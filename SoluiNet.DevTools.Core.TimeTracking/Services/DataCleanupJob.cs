// <copyright file="DataCleanupJob.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using SoluiNet.DevTools.Core.TimeTracking.Storage;

    /// <summary>
    /// Background job for cleaning up old time tracking data.
    /// </summary>
    [DisallowConcurrentExecution]
    public class DataCleanupJob : IJob
    {
        private readonly ILogger<DataCleanupJob>? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCleanupJob"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public DataCleanupJob(ILogger<DataCleanupJob>? logger = null)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Executes the data cleanup job.
        /// </summary>
        /// <param name="context">The job execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Execute(IJobExecutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.logger?.LogTrace("Starting data cleanup job execution");

            try
            {
                using var dbContext = new CrossPlatformTimeTrackingContext();

                // Check database connectivity
                var canConnect = await dbContext.Database.CanConnectAsync(context.CancellationToken).ConfigureAwait(false);

                if (!canConnect)
                {
                    this.logger?.LogWarning("Database connection check failed during data cleanup job");
                    return;
                }

                // Perform cleanup operations
                var totalCleaned = 0;

                // Clean up old usage time records (older than 2 years)
                totalCleaned += await this.CleanupOldUsageTimeRecordsAsync(dbContext, context).ConfigureAwait(false);

                // Clean up orphaned application records
                totalCleaned += await this.CleanupOrphanedApplicationsAsync(dbContext, context).ConfigureAwait(false);

                // Clean up old filter history (older than 6 months)
                totalCleaned += await this.CleanupOldFilterHistoryAsync(dbContext, context).ConfigureAwait(false);

                // Vacuum the database to reclaim space (SQLite specific)
                await this.VacuumDatabaseAsync(dbContext, context).ConfigureAwait(false);

                this.logger?.LogInformation("Data cleanup job completed successfully. Cleaned up {TotalCleaned} records", totalCleaned);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error occurred during data cleanup job execution");

                // Create a JobExecutionException to indicate the job failed
                throw new JobExecutionException(ex, refireImmediately: false);
            }
        }

        /// <summary>
        /// Cleans up old usage time records.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="context">The job execution context.</param>
        /// <returns>The number of records cleaned up.</returns>
        private async Task<int> CleanupOldUsageTimeRecordsAsync(
            CrossPlatformTimeTrackingContext dbContext,
            IJobExecutionContext context)
        {
            var cutoffDate = DateTime.UtcNow.AddYears(-2);

            var oldRecords = await dbContext.UsageTime
                .Where(ut => ut.StartTime < cutoffDate)
                .ToListAsync(context.CancellationToken)
                .ConfigureAwait(false);

            if (oldRecords.Count > 0)
            {
                dbContext.UsageTime.RemoveRange(oldRecords);
                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                this.logger?.LogInformation("Cleaned up {Count} old usage time records older than {CutoffDate}",
                    oldRecords.Count, cutoffDate);
            }

            return oldRecords.Count;
        }

        /// <summary>
        /// Cleans up orphaned application records.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="context">The job execution context.</param>
        /// <returns>The number of records cleaned up.</returns>
        private async Task<int> CleanupOrphanedApplicationsAsync(
            CrossPlatformTimeTrackingContext dbContext,
            IJobExecutionContext context)
        {
            var orphanedApps = await dbContext.Application
                .Where(app => !dbContext.UsageTime.Any(ut => ut.ApplicationId == app.ApplicationId))
                .ToListAsync(context.CancellationToken)
                .ConfigureAwait(false);

            if (orphanedApps.Count > 0)
            {
                dbContext.Application.RemoveRange(orphanedApps);
                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                this.logger?.LogInformation("Cleaned up {Count} orphaned application records", orphanedApps.Count);
            }

            return orphanedApps.Count;
        }

        /// <summary>
        /// Cleans up old filter history records.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="context">The job execution context.</param>
        /// <returns>The number of records cleaned up.</returns>
        private async Task<int> CleanupOldFilterHistoryAsync(
            CrossPlatformTimeTrackingContext dbContext,
            IJobExecutionContext context)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(-6);

            var oldFilterHistory = await dbContext.FilterHistory
                .Where(fh => fh.LastExecutionDateTime < cutoffDate)
                .ToListAsync(context.CancellationToken)
                .ConfigureAwait(false);

            if (oldFilterHistory.Count > 0)
            {
                dbContext.FilterHistory.RemoveRange(oldFilterHistory);
                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                this.logger?.LogInformation("Cleaned up {Count} old filter history records older than {CutoffDate}",
                    oldFilterHistory.Count, cutoffDate);
            }

            return oldFilterHistory.Count;
        }

        /// <summary>
        /// Vacuums the database to reclaim space.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="context">The job execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task VacuumDatabaseAsync(
            CrossPlatformTimeTrackingContext dbContext,
            IJobExecutionContext context)
        {
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("VACUUM;", context.CancellationToken).ConfigureAwait(false);
                this.logger?.LogDebug("Database vacuum completed successfully");
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Database vacuum failed, but continuing");
            }
        }
    }
}