// <copyright file="WindowMonitor.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Service for monitoring window changes.
    /// </summary>
    public class WindowMonitor : IWindowMonitor
    {
        private readonly IPlatformProvider platformProvider;
        private readonly ILogger<WindowMonitor>? logger;
        private CancellationTokenSource? monitoringCancellationTokenSource;
        private Task? monitoringTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMonitor"/> class.
        /// </summary>
        /// <param name="platformProvider">The platform provider.</param>
        /// <param name="logger">The logger instance.</param>
        public WindowMonitor(IPlatformProvider platformProvider, ILogger<WindowMonitor>? logger = null)
        {
            this.platformProvider = platformProvider ?? throw new ArgumentNullException(nameof(platformProvider));
            this.logger = logger;
        }

        /// <inheritdoc />
        public event EventHandler<WindowChangedEventArgs>? WindowChanged;

        /// <inheritdoc />
        public bool IsMonitoring => this.monitoringTask != null && !this.monitoringTask.IsCompleted;

        /// <inheritdoc />
        public async Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.platformProvider.GetActiveWindowAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to get active window");
                return null;
            }
        }

        /// <inheritdoc />
        public Task StartMonitoringAsync(TimeSpan interval, CancellationToken cancellationToken = default)
        {
            if (this.IsMonitoring)
            {
                this.logger?.LogWarning("Window monitoring is already active");
                return Task.CompletedTask;
            }

            this.logger?.LogInformation("Starting window monitoring with interval: {Interval}", interval);

            this.monitoringCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this.monitoringTask = this.MonitorWindowsAsync(interval, this.monitoringCancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task StopMonitoringAsync(CancellationToken cancellationToken = default)
        {
            if (!this.IsMonitoring)
            {
                this.logger?.LogDebug("Window monitoring is not active");
                return;
            }

            this.logger?.LogInformation("Stopping window monitoring");

            this.monitoringCancellationTokenSource?.Cancel();

            if (this.monitoringTask != null)
            {
                try
                {
                    await this.monitoringTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                }
                catch (Exception ex)
                {
                    this.logger?.LogError(ex, "Error occurred while stopping window monitoring");
                }
            }

            this.monitoringCancellationTokenSource?.Dispose();
            this.monitoringCancellationTokenSource = null;
            this.monitoringTask = null;
        }

        /// <summary>
        /// Monitors windows for changes.
        /// </summary>
        /// <param name="interval">The monitoring interval.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task MonitorWindowsAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            WindowInfo? previousWindow = null;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var currentWindow = await this.GetActiveWindowAsync(cancellationToken).ConfigureAwait(false);

                        // Check if window has changed
                        var hasChanged = this.HasWindowChanged(previousWindow, currentWindow);

                        if (hasChanged)
                        {
                            this.logger?.LogDebug("Window changed from '{Previous}' to '{Current}'", 
                                previousWindow?.ToString() ?? "null", 
                                currentWindow?.ToString() ?? "null");

                            this.WindowChanged?.Invoke(this, new WindowChangedEventArgs(previousWindow, currentWindow));
                            previousWindow = currentWindow?.Clone();
                        }

                        await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        this.logger?.LogError(ex, "Error during window monitoring iteration");
                        
                        // Wait a bit before retrying to avoid tight error loops
                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Unexpected error in window monitoring loop");
            }

            this.logger?.LogDebug("Window monitoring loop ended");
        }

        /// <summary>
        /// Determines if the window has changed significantly.
        /// </summary>
        /// <param name="previous">The previous window information.</param>
        /// <param name="current">The current window information.</param>
        /// <returns>True if the window has changed, false otherwise.</returns>
        private bool HasWindowChanged(WindowInfo? previous, WindowInfo? current)
        {
            if (previous == null && current == null)
            {
                return false;
            }

            if (previous == null || current == null)
            {
                return true;
            }

            return previous.ProcessName != current.ProcessName ||
                   previous.Title != current.Title ||
                   previous.ProcessId != current.ProcessId;
        }
    }
}