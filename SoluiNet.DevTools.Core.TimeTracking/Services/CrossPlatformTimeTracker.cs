// <copyright file="CrossPlatformTimeTracker.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;
    using SoluiNet.DevTools.Core.TimeTracking.Storage;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// Cross-platform implementation of the time tracker.
    /// </summary>
    public class CrossPlatformTimeTracker : ITimeTracker
    {
        private readonly IWindowMonitor windowMonitor;
        private readonly ITimeTrackingConfiguration configuration;
        private readonly IPlatformProvider platformProvider;
        private readonly ILogger<CrossPlatformTimeTracker>? logger;
        private readonly object statusLock = new object();

        private TimeTrackerStatus status;
        private WindowInfo? currentWindow;
        private DateTime? windowStartTime;
        private CrossPlatformTimeTrackingContext? dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossPlatformTimeTracker"/> class.
        /// </summary>
        /// <param name="windowMonitor">The window monitor service.</param>
        /// <param name="configuration">The time tracking configuration.</param>
        /// <param name="platformProvider">The platform provider for window monitoring.</param>
        /// <param name="logger">The logger instance.</param>
        public CrossPlatformTimeTracker(
            IWindowMonitor windowMonitor,
            ITimeTrackingConfiguration configuration,
            IPlatformProvider platformProvider,
            ILogger<CrossPlatformTimeTracker>? logger = null)
        {
            this.windowMonitor = windowMonitor ?? throw new ArgumentNullException(nameof(windowMonitor));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.platformProvider = platformProvider ?? throw new ArgumentNullException(nameof(platformProvider));
            this.logger = logger;

            this.status = new TimeTrackerStatus
            {
                IsRunning = false,
                MonitoringInterval = configuration.MonitoringInterval,
                PlatformProvider = platformProvider.PlatformName,
            };

            this.windowMonitor.WindowChanged += this.OnWindowChanged;
        }

        /// <inheritdoc />
        public event EventHandler<UsageDataEventArgs>? UsageDataCaptured;

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            lock (this.statusLock)
            {
                if (this.status.IsRunning)
                {
                    this.logger?.LogWarning("Time tracker is already running");
                    return;
                }

                this.status.IsRunning = true;
                this.status.StartedAt = DateTime.UtcNow;
                this.status.DataPointsCaptured = 0;
            }

            this.logger?.LogInformation("Starting cross-platform time tracker");

            try
            {
                // Initialize database context
                this.dbContext = new CrossPlatformTimeTrackingContext(logger: this.logger);

                await this.windowMonitor.StartMonitoringAsync(this.configuration.MonitoringInterval, cancellationToken)
                    .ConfigureAwait(false);

                this.logger?.LogInformation("Time tracker started successfully");
            }
            catch (Exception ex)
            {
                lock (this.statusLock)
                {
                    this.status.IsRunning = false;
                    this.status.StartedAt = null;
                }

                this.logger?.LogError(ex, "Failed to start time tracker");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            lock (this.statusLock)
            {
                if (!this.status.IsRunning)
                {
                    this.logger?.LogDebug("Time tracker is not running");
                    return;
                }
            }

            this.logger?.LogInformation("Stopping cross-platform time tracker");

            try
            {
                // Process any remaining window data
                await this.ProcessCurrentWindowDataAsync().ConfigureAwait(false);

                await this.windowMonitor.StopMonitoringAsync(cancellationToken).ConfigureAwait(false);

                // Dispose database context
                this.dbContext?.Dispose();
                this.dbContext = null;

                lock (this.statusLock)
                {
                    this.status.IsRunning = false;
                    this.status.StartedAt = null;
                }

                this.logger?.LogInformation("Time tracker stopped successfully");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error occurred while stopping time tracker");
                throw;
            }
        }

        /// <inheritdoc />
        public Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
        {
            lock (this.statusLock)
            {
                return Task.FromResult(this.status.IsRunning);
            }
        }

        /// <inheritdoc />
        public Task<TimeTrackerStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            lock (this.statusLock)
            {
                // Return a copy to avoid external modifications
                var statusCopy = new TimeTrackerStatus
                {
                    IsRunning = this.status.IsRunning,
                    StartedAt = this.status.StartedAt,
                    MonitoringInterval = this.status.MonitoringInterval,
                    DataPointsCaptured = this.status.DataPointsCaptured,
                    LastCapturedWindow = this.status.LastCapturedWindow?.Clone(),
                    PlatformProvider = this.status.PlatformProvider,
                };

                return Task.FromResult(statusCopy);
            }
        }

        /// <summary>
        /// Handles window change events from the window monitor.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The window change event arguments.</param>
        private async void OnWindowChanged(object? sender, WindowChangedEventArgs e)
        {
            try
            {
                await this.ProcessWindowChangeAsync(e).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error processing window change");
            }
        }

        /// <summary>
        /// Processes a window change event.
        /// </summary>
        /// <param name="e">The window change event arguments.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProcessWindowChangeAsync(WindowChangedEventArgs e)
        {
            // Process the previous window's usage data
            await this.ProcessCurrentWindowDataAsync().ConfigureAwait(false);

            // Update current window tracking
            this.currentWindow = e.CurrentWindow?.Clone();
            this.windowStartTime = e.CurrentWindow != null ? DateTime.UtcNow : null;

            lock (this.statusLock)
            {
                this.status.LastCapturedWindow = e.CurrentWindow?.Clone();
            }

            this.logger?.LogDebug("Window changed to: {Window}", e.CurrentWindow?.ToString() ?? "null");
        }

        /// <summary>
        /// Processes the current window's usage data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProcessCurrentWindowDataAsync()
        {
            if (this.currentWindow == null || this.windowStartTime == null)
            {
                return;
            }

            var duration = DateTime.UtcNow - this.windowStartTime.Value;

            // Only process if the duration is meaningful (at least 1 second)
            if (duration.TotalSeconds >= 1)
            {
                var usageData = new UsageDataEventArgs(this.currentWindow, duration);

                lock (this.statusLock)
                {
                    this.status.DataPointsCaptured++;
                }

                this.logger?.LogDebug("Captured usage data: {Window} for {Duration}",
                    this.currentWindow.ToString(), duration);

                // Store usage data in database
                await this.StoreUsageDataAsync(usageData).ConfigureAwait(false);

                // Raise the event for any listeners
                this.UsageDataCaptured?.Invoke(this, usageData);
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Stores usage data in the database.
        /// </summary>
        /// <param name="usageData">The usage data to store.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task StoreUsageDataAsync(UsageDataEventArgs usageData)
        {
            if (this.dbContext == null)
            {
                this.logger?.LogWarning("Database context is not initialized, cannot store usage data");
                return;
            }

            try
            {
                // Find or create application record
                var application = await this.GetOrCreateApplicationAsync(usageData.WindowInfo).ConfigureAwait(false);

                // Create usage time record
                var usageTime = new UsageTime
                {
                    ApplicationId = application.ApplicationId,
                    ApplicationIdentification = usageData.WindowInfo.ProcessName,
                    StartTime = usageData.CapturedAt - usageData.Duration,
                    Duration = (int)usageData.Duration.TotalSeconds,
                    AdditionalInformation = $"Title: {usageData.WindowInfo.Title}; Platform: {usageData.WindowInfo.Platform}; ProcessPath: {usageData.WindowInfo.ProcessPath}; WindowClass: {usageData.WindowInfo.WindowClass}",
                };

                this.dbContext.UsageTime.Add(usageTime);
                await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

                this.logger?.LogDebug("Stored usage data for {Application} - {Duration}s",
                    usageData.WindowInfo.ProcessName, usageData.Duration.TotalSeconds);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to store usage data for {Application}",
                    usageData.WindowInfo.ProcessName);
            }
        }

        /// <summary>
        /// Gets or creates an application record in the database.
        /// </summary>
        /// <param name="windowInfo">The window information.</param>
        /// <returns>The application record.</returns>
        private async Task<Application> GetOrCreateApplicationAsync(WindowInfo windowInfo)
        {
            if (this.dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized");
            }

            // Try to find existing application
            var existingApp = this.dbContext.Application
                .FirstOrDefault(a => a.ApplicationName == windowInfo.ProcessName);

            if (existingApp != null)
            {
                return existingApp;
            }

            // Create new application record
            var newApp = new Application
            {
                ApplicationName = windowInfo.ProcessName,
                ExtendedConfiguration = $"<config><platform>{windowInfo.Platform}</platform><processPath>{windowInfo.ProcessPath}</processPath></config>",
            };

            this.dbContext.Application.Add(newApp);
            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

            this.logger?.LogDebug("Created new application record for {Application}", windowInfo.ProcessName);
            return newApp;
        }
    }
}