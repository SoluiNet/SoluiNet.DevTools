// <copyright file="LimitedSqlQueryProcessor.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;

    /// <summary>
    /// Processor for executing limited SQL queries with security restrictions.
    /// </summary>
    public class LimitedSqlQueryProcessor : ILimitedSqlQueryProcessor
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly CrossPlatformTimeTrackingContext context;

        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly ILogger<LimitedSqlQueryProcessor> logger;

        /// <summary>
        /// Set of allowed table names.
        /// </summary>
        private readonly HashSet<string> allowedTables = new(StringComparer.OrdinalIgnoreCase)
        {
            "UsageTime",
            "Application",
            "Category",
            "ApplicationArea",
            "CategoryUsageTime",
            "Category_UsageTime"
        };

        /// <summary>
        /// Set of allowed SQL operations.
        /// </summary>
        private readonly HashSet<string> allowedOperations = new(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT"
        };

        /// <summary>
        /// Set of allowed SQL functions.
        /// </summary>
        private readonly HashSet<string> allowedFunctions = new(StringComparer.OrdinalIgnoreCase)
        {
            "COUNT", "SUM", "AVG", "MIN", "MAX",
            "DATE", "TIME", "DATETIME", "STRFTIME",
            "UPPER", "LOWER", "LENGTH", "SUBSTR",
            "ROUND", "ABS", "COALESCE", "IFNULL"
        };

        /// <summary>
        /// Set of forbidden keywords that indicate dangerous operations.
        /// </summary>
        private readonly HashSet<string> forbiddenKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "DROP", "DELETE", "UPDATE", "INSERT", "ALTER", "CREATE", "TRUNCATE",
            "EXEC", "EXECUTE", "CALL", "DECLARE", "CURSOR", "FETCH",
            "GRANT", "REVOKE", "COMMIT", "ROLLBACK", "TRANSACTION",
            "PRAGMA", "ATTACH", "DETACH", "VACUUM", "REINDEX"
        };

        /// <summary>
        /// Maximum number of rows that can be returned by a query.
        /// </summary>
        private const int MaxResultRows = 10000;

        /// <summary>
        /// Maximum query execution timeout in seconds.
        /// </summary>
        private const int QueryTimeoutSeconds = 30;

        /// <summary>
        /// Indicates whether this instance has been disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedSqlQueryProcessor"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public LimitedSqlQueryProcessor(CrossPlatformTimeTrackingContext context, ILogger<LimitedSqlQueryProcessor> logger = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger;
        }

        /// <summary>
        /// Validates whether a SQL query is allowed to be executed.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to validate.</param>
        /// <returns>True if the query is valid and safe to execute.</returns>
        public bool IsValidQuery(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                return false;
            }

            try
            {
                // Remove comments and normalize whitespace
                var normalizedQuery = NormalizeQuery(sqlQuery);

                // Check for forbidden keywords
                if (ContainsForbiddenKeywords(normalizedQuery))
                {
                    this.logger?.LogWarning("Query contains forbidden keywords: {Query}", sqlQuery);
                    return false;
                }

                // Check if it starts with an allowed operation
                if (!StartsWithAllowedOperation(normalizedQuery))
                {
                    this.logger?.LogWarning("Query does not start with allowed operation: {Query}", sqlQuery);
                    return false;
                }

                // Validate table names
                if (!ContainsOnlyAllowedTables(normalizedQuery))
                {
                    this.logger?.LogWarning("Query contains disallowed tables: {Query}", sqlQuery);
                    return false;
                }

                // Validate functions
                if (!ContainsOnlyAllowedFunctions(normalizedQuery))
                {
                    this.logger?.LogWarning("Query contains disallowed functions: {Query}", sqlQuery);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error validating query: {Query}", sqlQuery);
                return false;
            }
        }

        /// <summary>
        /// Checks if a SQL query is safe to execute (additional security checks).
        /// </summary>
        /// <param name="sqlQuery">The SQL query to check.</param>
        /// <returns>True if the query is safe to execute.</returns>
        public bool IsSafeQuery(string sqlQuery)
        {
            if (!IsValidQuery(sqlQuery))
            {
                return false;
            }

            var normalizedQuery = NormalizeQuery(sqlQuery);

            // Check for potential SQL injection patterns
            var injectionPatterns = new[]
            {
                @";\s*(DROP|DELETE|UPDATE|INSERT|ALTER|CREATE)",
                @"UNION\s+SELECT",
                @"--\s*[^\r\n]*",
                @"/\*.*?\*/",
                @"'[^']*'[^']*'",
                @"EXEC\s*\(",
                @"EXECUTE\s*\(",
                @"sp_\w+",
                @"xp_\w+"
            };

            foreach (var pattern in injectionPatterns)
            {
                if (Regex.IsMatch(normalizedQuery, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    this.logger?.LogWarning("Query matches injection pattern {Pattern}: {Query}", pattern, sqlQuery);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parses and validates a SQL query, returning a sanitized version.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to parse and validate.</param>
        /// <returns>The sanitized SQL query.</returns>
        public string ParseAndValidateQuery(string sqlQuery)
        {
            if (!IsSafeQuery(sqlQuery))
            {
                throw new InvalidOperationException("Query is not safe to execute");
            }

            var normalizedQuery = NormalizeQuery(sqlQuery);

            // Add LIMIT clause if not present to prevent large result sets
            if (!Regex.IsMatch(normalizedQuery, @"\bLIMIT\s+\d+", RegexOptions.IgnoreCase))
            {
                normalizedQuery += $" LIMIT {MaxResultRows}";
            }
            else
            {
                // Ensure LIMIT doesn't exceed maximum
                normalizedQuery = Regex.Replace(normalizedQuery, @"\bLIMIT\s+(\d+)",
                    match =>
                    {
                        var limit = int.Parse(match.Groups[1].Value);
                        return $"LIMIT {Math.Min(limit, MaxResultRows)}";
                    }, RegexOptions.IgnoreCase);
            }

            return normalizedQuery;
        }

        /// <summary>
        /// Executes a limited SQL query and returns the results.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The query results.</returns>
        public async Task<QueryResult> ExecuteQueryAsync(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                throw new ArgumentException("Query cannot be null or empty", nameof(sqlQuery));
            }

            var sanitizedQuery = ParseAndValidateQuery(sqlQuery);

            try
            {
                this.logger?.LogInformation("Executing limited SQL query: {Query}", sanitizedQuery);

                var startTime = DateTime.UtcNow;

                // Execute the query with timeout
                using var command = this.context.Database.GetDbConnection().CreateCommand();
                command.CommandText = sanitizedQuery;
                command.CommandTimeout = QueryTimeoutSeconds;

                await this.context.Database.OpenConnectionAsync();

                using var reader = await command.ExecuteReaderAsync();

                var result = new QueryResult
                {
                    Columns = new List<string>(),
                    Rows = new List<Dictionary<string, object>>(),
                    ExecutionTime = DateTime.UtcNow - startTime,
                    Query = sanitizedQuery
                };

                // Get column names
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result.Columns.Add(reader.GetName(i));
                }

                // Read data rows
                var rowCount = 0;
                while (await reader.ReadAsync() && rowCount < MaxResultRows)
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[reader.GetName(i)] = value;
                    }
                    result.Rows.Add(row);
                    rowCount++;
                }

                result.RowCount = result.Rows.Count;
                result.WasTruncated = rowCount >= MaxResultRows;

                this.logger?.LogInformation("Query executed successfully. Returned {RowCount} rows in {ExecutionTime}ms",
                    result.RowCount, result.ExecutionTime.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error executing SQL query: {Query}", sanitizedQuery);
                throw new InvalidOperationException($"Query execution failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Normalizes a SQL query by removing comments and extra whitespace.
        /// </summary>
        /// <param name="query">The query to normalize.</param>
        /// <returns>The normalized query.</returns>
        private static string NormalizeQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return string.Empty;
            }

            // Remove single-line comments
            query = Regex.Replace(query, @"--[^\r\n]*", string.Empty, RegexOptions.Multiline);

            // Remove multi-line comments
            query = Regex.Replace(query, @"/\*.*?\*/", string.Empty, RegexOptions.Singleline);

            // Normalize whitespace
            query = Regex.Replace(query, @"\s+", " ", RegexOptions.Multiline);

            return query.Trim();
        }

        /// <summary>
        /// Checks if the query contains any forbidden keywords.
        /// </summary>
        /// <param name="query">The query to check.</param>
        /// <returns>True if forbidden keywords are found.</returns>
        private bool ContainsForbiddenKeywords(string query)
        {
            return this.forbiddenKeywords.Any(keyword =>
                Regex.IsMatch(query, $@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Checks if the query starts with an allowed operation.
        /// </summary>
        /// <param name="query">The query to check.</param>
        /// <returns>True if it starts with an allowed operation.</returns>
        private bool StartsWithAllowedOperation(string query)
        {
            return this.allowedOperations.Any(operation =>
                query.StartsWith(operation, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the query contains only allowed table names.
        /// </summary>
        /// <param name="query">The query to check.</param>
        /// <returns>True if only allowed tables are referenced.</returns>
        private bool ContainsOnlyAllowedTables(string query)
        {
            // Extract table names from FROM and JOIN clauses
            var tablePattern = @"\b(?:FROM|JOIN)\s+([a-zA-Z_][a-zA-Z0-9_]*)\b";
            var matches = Regex.Matches(query, tablePattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var tableName = match.Groups[1].Value;
                if (!this.allowedTables.Contains(tableName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the query contains only allowed functions.
        /// </summary>
        /// <param name="query">The query to check.</param>
        /// <returns>True if only allowed functions are used.</returns>
        private bool ContainsOnlyAllowedFunctions(string query)
        {
            // Extract function names
            var functionPattern = @"\b([a-zA-Z_][a-zA-Z0-9_]*)\s*\(";
            var matches = Regex.Matches(query, functionPattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var functionName = match.Groups[1].Value;
                if (!this.allowedFunctions.Contains(functionName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Disposes the processor and its resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the processor and its resources.
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
    /// Represents the result of a SQL query execution.
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Gets or sets the column names.
        /// </summary>
        public List<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the data rows.
        /// </summary>
        public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();

        /// <summary>
        /// Gets or sets the number of rows returned.
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the result was truncated due to row limits.
        /// </summary>
        public bool WasTruncated { get; set; }

        /// <summary>
        /// Gets or sets the query execution time.
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets the executed query.
        /// </summary>
        public string Query { get; set; }
    }
}