// <copyright file="TimeTrackingScheduler.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using Quartz.Impl;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;

    /// <summary>
    /// Cross-platform scheduler for time tracking background tasks using Quartz.NET.
    /// </summary>
    public class TimeTrackingScheduler : ITimeTrackingScheduler
    {
        private readonly ILogger<TimeTrackingScheduler>? logger;
        private readonly ITimeTrackingConfiguration configuration;
        private IScheduler? scheduler;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingScheduler"/> class.
        /// </summary>
        /// <param name="configuration">The time tracking configuration.</param>
        /// <param name="logger">The logger instance.</param>
        public TimeTrackingScheduler(
            ITimeTrackingConfiguration configuration,
            ILogger<TimeTrackingScheduler>? logger = null)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger;
        }

        /// <summary>
        /// Gets a value indicating whether the scheduler is running.
        /// </summary>
        public bool IsRunning => this.scheduler?.IsStarted == true && !this.scheduler.IsShutdown;

        /// <summary>
        /// Starts the scheduler and schedules background tasks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (this.IsRunning)
            {
                this.logger?.LogWarning("Time tracking scheduler is already running");
                return;
            }

            this.logger?.LogInformation("Starting time tracking scheduler");

            try
            {
                // Create scheduler factory and get scheduler instance
                var factory = new StdSchedulerFactory();
                this.scheduler = await factory.GetScheduler(cancellationToken).ConfigureAwait(false);

                // Start the scheduler
                await this.scheduler.Start(cancellationToken).ConfigureAwait(false);

                // Schedule background tasks
                await this.ScheduleBackgroundTasksAsync(cancellationToken).ConfigureAwait(false);

                this.logger?.LogInformation("Time tracking scheduler started successfully");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to start time tracking scheduler");
                throw;
            }
        }

        /// <summary>
        /// Stops the scheduler gracefully.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!this.IsRunning)
            {
                this.logger?.LogDebug("Time tracking scheduler is not running");
                return;
            }

            this.logger?.LogInformation("Stopping time tracking scheduler");

            try
            {
                if (this.scheduler != null)
                {
                    // Gracefully shutdown the scheduler
                    await this.scheduler.Shutdown(waitForJobsToComplete: true, cancellationToken).ConfigureAwait(false);
                }

                this.logger?.LogInformation("Time tracking scheduler stopped successfully");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error occurred while stopping time tracking scheduler");
                throw;
            }
        }

        /// <summary>
        /// Schedules a job to run at specified intervals.
        /// </summary>
        /// <param name="jobType">The job type to schedule.</param>
        /// <param name="jobName">The name of the job.</param>
        /// <param name="groupName">The group name for the job.</param>
        /// <param name="intervalInSeconds">The interval in seconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ScheduleJobAsync(
            Type jobType,
            string jobName,
            string groupName,
            int intervalInSeconds,
            CancellationToken cancellationToken = default)
        {
            if (this.scheduler == null)
            {
                throw new InvalidOperationException("Scheduler is not initialized");
            }

            if (!typeof(IJob).IsAssignableFrom(jobType))
            {
                throw new ArgumentException("Job type must implement IJob interface", nameof(jobType));
            }

            this.logger?.LogDebug("Scheduling job {JobName} in group {GroupName} with interval {Interval}s",
                jobName, groupName, intervalInSeconds);

            // Create job detail
            var job = JobBuilder.Create(jobType)
                .WithIdentity(jobName, groupName)
                .Build();

            // Create trigger
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}_trigger", groupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(intervalInSeconds)
                    .RepeatForever())
                .Build();

            // Schedule the job
            await this.scheduler.ScheduleJob(job, trigger, cancellationToken).ConfigureAwait(false);

            this.logger?.LogInformation("Scheduled job {JobName} successfully", jobName);
        }

        /// <summary>
        /// Unschedules a job.
        /// </summary>
        /// <param name="jobName">The name of the job.</param>
        /// <param name="groupName">The group name for the job.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<bool> UnscheduleJobAsync(
            string jobName,
            string groupName,
            CancellationToken cancellationToken = default)
        {
            if (this.scheduler == null)
            {
                return false;
            }

            var jobKey = new JobKey(jobName, groupName);
            var result = await this.scheduler.DeleteJob(jobKey, cancellationToken).ConfigureAwait(false);

            if (result)
            {
                this.logger?.LogInformation("Unscheduled job {JobName} successfully", jobName);
            }
            else
            {
                this.logger?.LogWarning("Failed to unschedule job {JobName} - job may not exist", jobName);
            }

            return result;
        }

        /// <summary>
        /// Disposes the scheduler resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the scheduler resources.
        /// </summary>
        /// <param name="disposing">True if disposing, false if finalizing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                try
                {
                    if (this.IsRunning)
                    {
                        // Synchronous shutdown for dispose
                        this.scheduler?.Shutdown(waitForJobsToComplete: false).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    this.logger?.LogError(ex, "Error during scheduler disposal");
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Schedules the default background tasks for time tracking.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ScheduleBackgroundTasksAsync(CancellationToken cancellationToken)
        {
            // Schedule data persistence job every 30 seconds
            await this.ScheduleJobAsync(
                typeof(DataPersistenceJob),
                "DataPersistenceJob",
                "TimeTracking",
                30,
                cancellationToken).ConfigureAwait(false);

            // Schedule cleanup job every hour (3600 seconds)
            await this.ScheduleJobAsync(
                typeof(DataCleanupJob),
                "DataCleanupJob",
                "TimeTracking",
                3600,
                cancellationToken).ConfigureAwait(false);

            this.logger?.LogInformation("Scheduled default background tasks for time tracking");
        }
    }
}