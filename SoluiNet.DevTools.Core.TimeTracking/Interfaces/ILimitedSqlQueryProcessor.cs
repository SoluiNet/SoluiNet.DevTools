// <copyright file="ILimitedSqlQueryProcessor.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.TimeTracking.Storage;

    /// <summary>
    /// Interface for processing limited SQL queries with security restrictions.
    /// </summary>
    public interface ILimitedSqlQueryProcessor : IDisposable
    {
        /// <summary>
        /// Validates whether a SQL query is allowed to be executed.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to validate.</param>
        /// <returns>True if the query is valid and safe to execute.</returns>
        bool IsValidQuery(string sqlQuery);

        /// <summary>
        /// Checks if a SQL query is safe to execute (additional security checks).
        /// </summary>
        /// <param name="sqlQuery">The SQL query to check.</param>
        /// <returns>True if the query is safe to execute.</returns>
        bool IsSafeQuery(string sqlQuery);

        /// <summary>
        /// Parses and validates a SQL query, returning a sanitized version.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to parse and validate.</param>
        /// <returns>The sanitized SQL query.</returns>
        string ParseAndValidateQuery(string sqlQuery);

        /// <summary>
        /// Executes a limited SQL query and returns the results.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The query results.</returns>
        Task<QueryResult> ExecuteQueryAsync(string sqlQuery);
    }
}