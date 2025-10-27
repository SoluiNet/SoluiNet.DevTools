// <copyright file="ITimeTracker.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.TimeTracking.Models;

    /// <summary>
    /// Interface for time tracking operations.
    /// </summary>
    public interface ITimeTracker
    {
        /// <summary>
        /// Occurs when usage data is captured.
        /// </summary>
        event EventHandler<UsageDataEventArgs>? UsageDataCaptured;

        /// <summary>
        /// Starts the time tracking asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the time tracking asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a value indicating whether the time tracker is currently running.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns true if running, false otherwise.</returns>
        Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current status of the time tracker.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns the current status.</returns>
        Task<TimeTrackerStatus> GetStatusAsync(CancellationToken cancellationToken = default);
    }
}