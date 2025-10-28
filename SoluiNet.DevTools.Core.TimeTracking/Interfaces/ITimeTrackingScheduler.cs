// <copyright file="ITimeTrackingScheduler.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for time tracking scheduler operations.
    /// </summary>
    public interface ITimeTrackingScheduler : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the scheduler is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts the scheduler and schedules background tasks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the scheduler gracefully.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Schedules a job to run at specified intervals.
        /// </summary>
        /// <param name="jobType">The job type to schedule.</param>
        /// <param name="jobName">The name of the job.</param>
        /// <param name="groupName">The group name for the job.</param>
        /// <param name="intervalInSeconds">The interval in seconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ScheduleJobAsync(
            Type jobType,
            string jobName,
            string groupName,
            int intervalInSeconds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Unschedules a job.
        /// </summary>
        /// <param name="jobName">The name of the job.</param>
        /// <param name="groupName">The group name for the job.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns true if the job was unscheduled.</returns>
        Task<bool> UnscheduleJobAsync(
            string jobName,
            string groupName,
            CancellationToken cancellationToken = default);
    }
}