// <copyright file="IWindowMonitor.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Interface for window monitoring operations.
    /// </summary>
    public interface IWindowMonitor
    {
        /// <summary>
        /// Gets the currently active window information asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns the active window information.</returns>
        Task<WindowInfo?> GetActiveWindowAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts monitoring windows at the specified interval.
        /// </summary>
        /// <param name="interval">The monitoring interval.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartMonitoringAsync(TimeSpan interval, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops window monitoring.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StopMonitoringAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a value indicating whether monitoring is currently active.
        /// </summary>
        /// <returns>True if monitoring is active, false otherwise.</returns>
        bool IsMonitoring { get; }

        /// <summary>
        /// Occurs when a window change is detected.
        /// </summary>
        event EventHandler<WindowChangedEventArgs>? WindowChanged;
    }
}