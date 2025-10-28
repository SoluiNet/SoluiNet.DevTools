// <copyright file="TimeRangeQueryService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// Service for time-based filtering and querying of usage data.
    /// </summary>
    public class TimeRangeQueryService : ITimeRangeQueryService
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly CrossPlatformTimeTrackingContext context;

        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly ILogger<TimeRangeQueryService> logger;

        /// <summary>
        /// Indicates whether this instance has been disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRangeQueryService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public TimeRangeQueryService(CrossPlatformTimeTrackingContext context, ILogger<TimeRangeQueryService> logger = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger;
        }

        /// <summary>
        /// Queries usage data within specific time ranges on given dates.
        /// </summary>
        /// <param name="date">The date to filter by.</param>
        /// <param name="fromTime">The start time within the date (optional).</param>
        /// <param name="toTime">The end time within the date (optional).</param>
        /// <returns>A collection of usage time records within the specified time range.</returns>
        public async Task<IEnumerable<UsageTime>> QueryTimeRangeAsync(DateTime date, TimeSpan? fromTime = null, TimeSpan? toTime = null)
        {
            try
            {
                this.logger?.LogDebug("Executing time range query for date: {Date}, from: {FromTime}, to: {ToTime}",
                    date.Date, fromTime, toTime);

                var startDateTime = date.Date;
                var endDateTime = date.Date.AddDays(1).AddTicks(-1); // End of day

                if (fromTime.HasValue)
                {
                    startDateTime = date.Date.Add(fromTime.Value);
                }

                if (toTime.HasValue)
                {
                    endDateTime = date.Date.Add(toTime.Value);

                    // Handle case where toTime is before fromTime (spans midnight)
                    if (fromTime.HasValue && toTime.Value < fromTime.Value)
                    {
                        endDateTime = endDateTime.AddDays(1);
                    }
                }

                var results = await this.context.UsageTime
                    .Where(u => u.StartTime >= startDateTime && u.StartTime <= endDateTime)
                    .Include(u => u.Application)
                    .Include(u => u.ApplicationArea)
                    .Include(u => u.CategoryUsageTime)
                        .ThenInclude(cu => cu.Category)
                    .OrderBy(u => u.StartTime)
                    .ToListAsync();

                this.logger?.LogDebug("Time range query returned {Count} results", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing time range query for date: {Date}", date);
                throw;
            }
        }

        /// <summary>
        /// Queries usage data within specific time ranges across multiple dates.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <param name="fromTime">The start time within each date (optional).</param>
        /// <param name="toTime">The end time within each date (optional).</param>
        /// <returns>A collection of usage time records within the specified time ranges.</returns>
        public async Task<IEnumerable<UsageTime>> QueryTimeRangeAsync(DateTime fromDate, DateTime toDate, TimeSpan? fromTime = null, TimeSpan? toTime = null)
        {
            try
            {
                this.logger?.LogDebug("Executing multi-date time range query from {FromDate} to {ToDate}, time: {FromTime}-{ToTime}",
                    fromDate.Date, toDate.Date, fromTime, toTime);

                var query = this.context.UsageTime.AsQueryable();

                if (fromTime.HasValue && toTime.HasValue)
                {
                    // Filter by time of day across the date range
                    if (toTime.Value >= fromTime.Value)
                    {
                        // Normal time range (doesn't span midnight)
                        query = query.Where(u =>
                            u.StartTime.Date >= fromDate.Date &&
                            u.StartTime.Date <= toDate.Date &&
                            u.StartTime.TimeOfDay >= fromTime.Value &&
                            u.StartTime.TimeOfDay <= toTime.Value);
                    }
                    else
                    {
                        // Time range spans midnight
                        query = query.Where(u =>
                            u.StartTime.Date >= fromDate.Date &&
                            u.StartTime.Date <= toDate.Date &&
                            (u.StartTime.TimeOfDay >= fromTime.Value || u.StartTime.TimeOfDay <= toTime.Value));
                    }
                }
                else if (fromTime.HasValue)
                {
                    // Only start time specified
                    query = query.Where(u =>
                        u.StartTime.Date >= fromDate.Date &&
                        u.StartTime.Date <= toDate.Date &&
                        u.StartTime.TimeOfDay >= fromTime.Value);
                }
                else if (toTime.HasValue)
                {
                    // Only end time specified
                    query = query.Where(u =>
                        u.StartTime.Date >= fromDate.Date &&
                        u.StartTime.Date <= toDate.Date &&
                        u.StartTime.TimeOfDay <= toTime.Value);
                }
                else
                {
                    // No time filtering, just date range
                    query = query.Where(u =>
                        u.StartTime.Date >= fromDate.Date &&
                        u.StartTime.Date <= toDate.Date);
                }

                var results = await query
                    .Include(u => u.Application)
                    .Include(u => u.ApplicationArea)
                    .Include(u => u.CategoryUsageTime)
                        .ThenInclude(cu => cu.Category)
                    .OrderBy(u => u.StartTime)
                    .ToListAsync();

                this.logger?.LogDebug("Multi-date time range query returned {Count} results", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing multi-date time range query from {FromDate} to {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <summary>
        /// Gets usage statistics grouped by hour for a specific date.
        /// </summary>
        /// <param name="date">The date to analyze.</param>
        /// <returns>A collection of hourly usage statistics.</returns>
        public async Task<IEnumerable<HourlyUsageStatistic>> GetHourlyUsageStatisticsAsync(DateTime date)
        {
            try
            {
                this.logger?.LogDebug("Getting hourly usage statistics for date: {Date}", date.Date);

                var startDate = date.Date;
                var endDate = date.Date.AddDays(1);

                var results = await this.context.UsageTime
                    .Where(u => u.StartTime >= startDate && u.StartTime < endDate)
                    .GroupBy(u => u.StartTime.Hour)
                    .Select(g => new HourlyUsageStatistic
                    {
                        Hour = g.Key,
                        TotalDuration = g.Sum(u => u.Duration),
                        SessionCount = g.Count(),
                        AverageDuration = g.Average(u => u.Duration)
                    })
                    .OrderBy(h => h.Hour)
                    .ToListAsync();

                this.logger?.LogDebug("Retrieved hourly statistics for {Count} hours", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error getting hourly usage statistics for date: {Date}", date);
                throw;
            }
        }

        /// <summary>
        /// Gets usage statistics grouped by day of week for a date range.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <returns>A collection of daily usage statistics.</returns>
        public async Task<IEnumerable<DailyUsageStatistic>> GetDailyUsageStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                this.logger?.LogDebug("Getting daily usage statistics from {FromDate} to {ToDate}", fromDate.Date, toDate.Date);

                var results = await this.context.UsageTime
                    .Where(u => u.StartTime.Date >= fromDate.Date && u.StartTime.Date <= toDate.Date)
                    .GroupBy(u => u.StartTime.Date)
                    .Select(g => new DailyUsageStatistic
                    {
                        Date = g.Key,
                        DayOfWeek = g.Key.DayOfWeek,
                        TotalDuration = g.Sum(u => u.Duration),
                        SessionCount = g.Count(),
                        AverageDuration = g.Average(u => u.Duration)
                    })
                    .OrderBy(d => d.Date)
                    .ToListAsync();

                this.logger?.LogDebug("Retrieved daily statistics for {Count} days", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error getting daily usage statistics from {FromDate} to {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <summary>
        /// Gets the most active time periods for a given date range.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <param name="topCount">The number of top periods to return.</param>
        /// <returns>A collection of the most active time periods.</returns>
        public async Task<IEnumerable<ActiveTimePeriod>> GetMostActiveTimePeriodsAsync(DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            try
            {
                this.logger?.LogDebug("Getting most active time periods from {FromDate} to {ToDate}, top {TopCount}",
                    fromDate.Date, toDate.Date, topCount);

                var results = await this.context.UsageTime
                    .Where(u => u.StartTime.Date >= fromDate.Date && u.StartTime.Date <= toDate.Date)
                    .GroupBy(u => u.StartTime.Hour)
                    .Select(g => new ActiveTimePeriod
                    {
                        Hour = g.Key,
                        TotalDuration = g.Sum(u => u.Duration),
                        SessionCount = g.Count(),
                        AverageDuration = g.Average(u => u.Duration),
                        MostUsedApplication = g.GroupBy(u => u.ApplicationIdentification)
                            .OrderByDescending(ag => ag.Sum(au => au.Duration))
                            .Select(ag => ag.Key)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(p => p.TotalDuration)
                    .Take(topCount)
                    .ToListAsync();

                this.logger?.LogDebug("Retrieved {Count} most active time periods", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error getting most active time periods from {FromDate} to {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <summary>
        /// Disposes the service and its resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the service and its resources.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                // Context is managed externally, don't dispose it here
                this.disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents hourly usage statistics.
    /// </summary>
    public class HourlyUsageStatistic
    {
        /// <summary>
        /// Gets or sets the hour (0-23).
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Gets or sets the total duration in seconds.
        /// </summary>
        public long TotalDuration { get; set; }

        /// <summary>
        /// Gets or sets the number of sessions.
        /// </summary>
        public int SessionCount { get; set; }

        /// <summary>
        /// Gets or sets the average duration in seconds.
        /// </summary>
        public double AverageDuration { get; set; }
    }

    /// <summary>
    /// Represents daily usage statistics.
    /// </summary>
    public class DailyUsageStatistic
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the day of week.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets the total duration in seconds.
        /// </summary>
        public long TotalDuration { get; set; }

        /// <summary>
        /// Gets or sets the number of sessions.
        /// </summary>
        public int SessionCount { get; set; }

        /// <summary>
        /// Gets or sets the average duration in seconds.
        /// </summary>
        public double AverageDuration { get; set; }
    }

    /// <summary>
    /// Represents an active time period with usage statistics.
    /// </summary>
    public class ActiveTimePeriod
    {
        /// <summary>
        /// Gets or sets the hour (0-23).
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Gets or sets the total duration in seconds.
        /// </summary>
        public long TotalDuration { get; set; }

        /// <summary>
        /// Gets or sets the number of sessions.
        /// </summary>
        public int SessionCount { get; set; }

        /// <summary>
        /// Gets or sets the average duration in seconds.
        /// </summary>
        public double AverageDuration { get; set; }

        /// <summary>
        /// Gets or sets the most used application during this time period.
        /// </summary>
        public string MostUsedApplication { get; set; }
    }
}