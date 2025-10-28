// <copyright file="ITimeRangeQueryService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.TimeTracking.Storage;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// Interface for time-based filtering and querying of usage data.
    /// </summary>
    public interface ITimeRangeQueryService : IDisposable
    {
        /// <summary>
        /// Queries usage data within specific time ranges on given dates.
        /// </summary>
        /// <param name="date">The date to filter by.</param>
        /// <param name="fromTime">The start time within the date (optional).</param>
        /// <param name="toTime">The end time within the date (optional).</param>
        /// <returns>A collection of usage time records within the specified time range.</returns>
        Task<IEnumerable<UsageTime>> QueryTimeRangeAsync(DateTime date, TimeSpan? fromTime = null, TimeSpan? toTime = null);

        /// <summary>
        /// Queries usage data within specific time ranges across multiple dates.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <param name="fromTime">The start time within each date (optional).</param>
        /// <param name="toTime">The end time within each date (optional).</param>
        /// <returns>A collection of usage time records within the specified time ranges.</returns>
        Task<IEnumerable<UsageTime>> QueryTimeRangeAsync(DateTime fromDate, DateTime toDate, TimeSpan? fromTime = null, TimeSpan? toTime = null);

        /// <summary>
        /// Gets usage statistics grouped by hour for a specific date.
        /// </summary>
        /// <param name="date">The date to analyze.</param>
        /// <returns>A collection of hourly usage statistics.</returns>
        Task<IEnumerable<HourlyUsageStatistic>> GetHourlyUsageStatisticsAsync(DateTime date);

        /// <summary>
        /// Gets usage statistics grouped by day of week for a date range.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <returns>A collection of daily usage statistics.</returns>
        Task<IEnumerable<DailyUsageStatistic>> GetDailyUsageStatisticsAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Gets the most active time periods for a given date range.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <param name="topCount">The number of top periods to return.</param>
        /// <returns>A collection of the most active time periods.</returns>
        Task<IEnumerable<ActiveTimePeriod>> GetMostActiveTimePeriodsAsync(DateTime fromDate, DateTime toDate, int topCount = 10);
    }
}