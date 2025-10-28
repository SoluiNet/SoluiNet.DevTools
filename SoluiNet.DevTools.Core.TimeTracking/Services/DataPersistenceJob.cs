// <copyright file="DataPersistenceJob.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using SoluiNet.DevTools.Core.TimeTracking.Storage;

    /// <summary>
    /// Background job for persisting time tracking data.
    /// </summary>
    [DisallowConcurrentExecution]
    public class DataPersistenceJob : IJob
    {
        private readonly ILogger<DataPersistenceJob>? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPersistenceJob"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public DataPersistenceJob(ILogger<DataPersistenceJob>? logger = null)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Executes the data persistence job.
        /// </summary>
        /// <param name="context">The job execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(IJobExecutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.logger?.LogTrace("Starting data persistence job execution");

            try
            {
                // Ensure database connection is healthy and perform any pending operations
                using var dbContext = new CrossPlatformTimeTrackingContext();

                // Check database connectivity
                var canConnect = await dbContext.Database.CanConnectAsync(context.CancellationToken).ConfigureAwait(false);

                if (!canConnect)
                {
                    this.logger?.LogWarning("Database connection check failed during data persistence job");
                    return;
                }

                // Perform any pending database maintenance operations
                await this.PerformMaintenanceOperationsAsync(dbContext, context).ConfigureAwait(false);

                this.logger?.LogTrace("Data persistence job completed successfully");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error occurred during data persistence job execution");

                // Create a JobExecutionException to indicate the job failed
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }

        /// <summary>
        /// Performs maintenance operations on the database.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="context">The job execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task PerformMaintenanceOperationsAsync(
            CrossPlatformTimeTrackingContext dbContext,
            IJobExecutionContext context)
        {
            // Ensure all pending changes are saved
            var changesSaved = await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

            if (changesSaved > 0)
            {
                this.logger?.LogDebug("Saved {ChangesSaved} pending changes to database", changesSaved);
            }

            // Perform database optimization (SQLite specific)
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("PRAGMA optimize;", context.CancellationToken).ConfigureAwait(false);
                this.logger?.LogTrace("Database optimization completed");
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Database optimization failed, but continuing");
            }
        }
    }
}