// <copyright file="LinuxPlatformProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Platform
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Linux-specific platform provider for window and process information.
    /// </summary>
    public class LinuxPlatformProvider : IPlatformProvider
    {
        private readonly ILogger? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinuxPlatformProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LinuxPlatformProvider(ILogger? logger = null)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string PlatformName => "Linux";

        /// <inheritdoc />
        public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <inheritdoc />
        public Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Linux-specific window detection using X11/Wayland APIs
            // This will be implemented in task 2.3
            this.logger?.LogDebug("Getting active window on Linux (placeholder implementation)");
            
            var windowInfo = new WindowInfo
            {
                Title = "Placeholder Linux Window",
                ProcessName = "placeholder",
                ProcessId = 5678,
                Platform = this.PlatformName,
                CapturedAt = DateTime.UtcNow,
            };

            return Task.FromResult<WindowInfo?>(windowInfo);
        }

        /// <inheritdoc />
        public Task<ProcessInfo?> GetActiveProcessAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Linux-specific process detection
            // This will be implemented in task 2.3
            this.logger?.LogDebug("Getting active process on Linux (placeholder implementation)");
            
            var processInfo = new ProcessInfo
            {
                ProcessId = 5678,
                ProcessName = "placeholder",
                Platform = this.PlatformName,
                CapturedAt = DateTime.UtcNow,
            };

            return Task.FromResult<ProcessInfo?>(processInfo);
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Initializing Linux platform provider");
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DisposeAsync(CancellationToken cancellationToken = default)
        {
            this.logger?.LogDebug("Disposing Linux platform provider");
            return Task.CompletedTask;
        }
    }
}