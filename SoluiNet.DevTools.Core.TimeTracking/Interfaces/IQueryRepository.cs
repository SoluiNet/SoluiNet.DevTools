// <copyright file="IQueryRepository.cs" company="SoluiNet">
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
    /// Interface for advanced querying of time tracking data.
    /// </summary>
    public interface IQueryRepository : IDisposable
    {
        /// <summary>
        /// Queries usage data using regex pattern matching.
        /// </summary>
        /// <param name="pattern">The regex pattern to match.</param>
        /// <param name="field">The field to search in.</param>
        /// <returns>A collection of matching usage time records.</returns>
        Task<IEnumerable<UsageTime>> QueryWithRegexAsync(string pattern, QueryField field);

        /// <summary>
        /// Queries usage data using LIKE pattern matching with wildcards.
        /// </summary>
        /// <param name="pattern">The LIKE pattern to match (supports % and _ wildcards).</param>
        /// <param name="field">The field to search in.</param>
        /// <returns>A collection of matching usage time records.</returns>
        Task<IEnumerable<UsageTime>> QueryWithLikeAsync(string pattern, QueryField field);

        /// <summary>
        /// Queries usage data by date range.
        /// </summary>
        /// <param name="fromDate">The start date (inclusive).</param>
        /// <param name="toDate">The end date (inclusive).</param>
        /// <returns>A collection of usage time records within the date range.</returns>
        Task<IEnumerable<UsageTime>> QueryByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Queries usage data by application ID.
        /// </summary>
        /// <param name="applicationId">The application ID.</param>
        /// <returns>A collection of usage time records for the specified application.</returns>
        Task<IEnumerable<UsageTime>> QueryByApplicationAsync(int applicationId);

        /// <summary>
        /// Gets all applications with their usage statistics.
        /// </summary>
        /// <returns>A collection of applications with usage data.</returns>
        Task<IEnumerable<Application>> GetApplicationsWithUsageAsync();

        /// <summary>
        /// Gets all categories with their usage statistics.
        /// </summary>
        /// <returns>A collection of categories with usage data.</returns>
        Task<IEnumerable<Category>> GetCategoriesWithUsageAsync();
    }
}