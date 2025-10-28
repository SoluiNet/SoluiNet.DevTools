// <copyright file="QueryRepository.cs" company="SoluiNet">
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
    /// Repository for advanced querying of time tracking data with regex and pattern matching support.
    /// </summary>
    public class QueryRepository : IQueryRepository
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly CrossPlatformTimeTrackingContext context;

        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly ILogger<QueryRepository> logger;

        /// <summary>
        /// Indicates whether this instance has been disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public QueryRepository(CrossPlatformTimeTrackingContext context, ILogger<QueryRepository> logger = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger;
        }

        /// <summary>
        /// Queries usage data using regex pattern matching.
        /// </summary>
        /// <param name="pattern">The regex pattern to match.</param>
        /// <param name="field">The field to search in.</param>
        /// <returns>A collection of matching usage time records.</returns>
        public async Task<IEnumerable<UsageTime>> QueryWithRegexAsync(string pattern, QueryField field)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));
            }

            try
            {
                this.logger?.LogDebug("Executing regex query with pattern: {Pattern} on field: {Field}", pattern, field);

                var query = this.context.UsageTime.AsQueryable();

                switch (field)
                {
                    case QueryField.ApplicationIdentification:
                        // Use EF.Functions.Like with regex-like patterns converted to SQL LIKE
                        query = query.Where(u => EF.Functions.Like(u.ApplicationIdentification, ConvertRegexToLike(pattern)));
                        break;
                    case QueryField.WindowTitle:
                        query = query.Where(u => EF.Functions.Like(u.AdditionalInformation, ConvertRegexToLike(pattern)));
                        break;
                    case QueryField.ProcessName:
                        // For process name, we'll search in ApplicationIdentification
                        query = query.Where(u => EF.Functions.Like(u.ApplicationIdentification, ConvertRegexToLike(pattern)));
                        break;
                    case QueryField.All:
                        query = query.Where(u =>
                            EF.Functions.Like(u.ApplicationIdentification, ConvertRegexToLike(pattern)) ||
                            EF.Functions.Like(u.AdditionalInformation, ConvertRegexToLike(pattern)));
                        break;
                    default:
                        throw new ArgumentException($"Unsupported query field: {field}", nameof(field));
                }

                var results = await query
                    .Include(u => u.Application)
                    .Include(u => u.ApplicationArea)
                    .Include(u => u.CategoryUsageTime)
                        .ThenInclude(cu => cu.Category)
                    .OrderByDescending(u => u.StartTime)
                    .ToListAsync();

                this.logger?.LogDebug("Regex query returned {Count} results", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing regex query with pattern: {Pattern}", pattern);
                throw;
            }
        }

        /// <summary>
        /// Queries usage data using LIKE pattern matching with wildcards.
        /// </summary>
        /// <param name="pattern">The LIKE pattern to match (supports % and _ wildcards).</param>
        /// <param name="field">The field to search in.</param>
        /// <returns>A collection of matching usage time records.</returns>
        public async Task<IEnumerable<UsageTime>> QueryWithLikeAsync(string pattern, QueryField field)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));
            }

            try
            {
                this.logger?.LogDebug("Executing LIKE query with pattern: {Pattern} on field: {Field}", pattern, field);

                var query = this.context.UsageTime.AsQueryable();

                switch (field)
                {
                    case QueryField.ApplicationIdentification:
                        query = query.Where(u => EF.Functions.Like(u.ApplicationIdentification, pattern));
                        break;
                    case QueryField.WindowTitle:
                        query = query.Where(u => EF.Functions.Like(u.AdditionalInformation, pattern));
                        break;
                    case QueryField.ProcessName:
                        query = query.Where(u => EF.Functions.Like(u.ApplicationIdentification, pattern));
                        break;
                    case QueryField.All:
                        query = query.Where(u =>
                            EF.Functions.Like(u.ApplicationIdentification, pattern) ||
                            EF.Functions.Like(u.AdditionalInformation, pattern));
                        break;
                    default:
                        throw new ArgumentException($"Unsupported query field: {field}", nameof(field));
                }

                var results = await query
                    .Include(u => u.Application)
                    .Include(u => u.ApplicationArea)
                    .Include(u => u.CategoryUsageTime)
                        .ThenInclude(cu => cu.Category)
                    .OrderByDescending(u => u.StartTime)
                    .ToListAsync();

                this.logger?.LogDebug("LIKE query returned {Count} results", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing LIKE query with pattern: {Pattern}", pattern);
                throw;
            }
        }

        /// <summary>
        /// Queries usage data by date range.
        /// </summary>
        /// <param name="fromDate">The start date (inclusive).</param>
        /// <param name="toDate">The end date (inclusive).</param>
        /// <returns>A collection of usage time records within the date range.</returns>
        public async Task<IEnumerable<UsageTime>> QueryByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                this.logger?.LogDebug("Executing date range query from {FromDate} to {ToDate}", fromDate, toDate);

                var results = await this.context.UsageTime
                    .Where(u => u.StartTime >= fromDate && u.StartTime <= toDate)
                    .Include(u => u.Application)
                    .Include(u => u.ApplicationArea)
                    .Include(u => u.CategoryUsageTime)
                        .ThenInclude(cu => cu.Category)
                    .OrderByDescending(u => u.StartTime)
                    .ToListAsync();

                this.logger?.LogDebug("Date range query returned {Count} results", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing date range query from {FromDate} to {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <summary>
        /// Queries usage data by application ID.
        /// </summary>
        /// <param name="applicationId">The application ID.</param>
        /// <returns>A collection of usage time records for the specified application.</returns>
        public async Task<IEnumerable<UsageTime>> QueryByApplicationAsync(int applicationId)
        {
            try
            {
                this.logger?.LogDebug("Executing application query for ID: {ApplicationId}", applicationId);

                var results = await this.context.UsageTime
                    .Where(u => u.ApplicationId == applicationId)
                    .Include(u => u.Application)
                    .Include(u => u.ApplicationArea)
                    .Include(u => u.CategoryUsageTime)
                        .ThenInclude(cu => cu.Category)
                    .OrderByDescending(u => u.StartTime)
                    .ToListAsync();

                this.logger?.LogDebug("Application query returned {Count} results", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing application query for ID: {ApplicationId}", applicationId);
                throw;
            }
        }

        /// <summary>
        /// Gets all applications with their usage statistics.
        /// </summary>
        /// <returns>A collection of applications with usage data.</returns>
        public async Task<IEnumerable<Application>> GetApplicationsWithUsageAsync()
        {
            try
            {
                this.logger?.LogDebug("Retrieving applications with usage data");

                var results = await this.context.Application
                    .Include(a => a.UsageTime)
                    .Include(a => a.ApplicationArea)
                    .ToListAsync();

                this.logger?.LogDebug("Retrieved {Count} applications", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error retrieving applications with usage data");
                throw;
            }
        }

        /// <summary>
        /// Gets all categories with their usage statistics.
        /// </summary>
        /// <returns>A collection of categories with usage data.</returns>
        public async Task<IEnumerable<Category>> GetCategoriesWithUsageAsync()
        {
            try
            {
                this.logger?.LogDebug("Retrieving categories with usage data");

                var results = await this.context.Category
                    .Include(c => c.CategoryUsageTime)
                        .ThenInclude(cu => cu.UsageTime)
                    .ToListAsync();

                this.logger?.LogDebug("Retrieved {Count} categories", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error retrieving categories with usage data");
                throw;
            }
        }

        /// <summary>
        /// Converts a simple regex pattern to a SQL LIKE pattern.
        /// </summary>
        /// <param name="regexPattern">The regex pattern.</param>
        /// <returns>The equivalent SQL LIKE pattern.</returns>
        private static string ConvertRegexToLike(string regexPattern)
        {
            // Simple conversion for basic regex patterns
            // This is a simplified implementation - for full regex support,
            // we would need to use SQLite's REGEXP function with a custom extension

            var likePattern = regexPattern
                .Replace(".*", "%")  // .* becomes %
                .Replace(".+", "%")  // .+ becomes %
                .Replace(".", "_")   // . becomes _
                .Replace("^", "")    // Remove start anchor
                .Replace("$", "");   // Remove end anchor

            return likePattern;
        }

        /// <summary>
        /// Disposes the repository and its resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the repository and its resources.
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
    /// Enumeration of fields that can be queried.
    /// </summary>
    public enum QueryField
    {
        /// <summary>
        /// Application identification field.
        /// </summary>
        ApplicationIdentification,

        /// <summary>
        /// Window title field.
        /// </summary>
        WindowTitle,

        /// <summary>
        /// Process name field.
        /// </summary>
        ProcessName,

        /// <summary>
        /// Process path field.
        /// </summary>
        ProcessPath,

        /// <summary>
        /// Platform field.
        /// </summary>
        Platform,

        /// <summary>
        /// Search all fields.
        /// </summary>
        All
    }
}