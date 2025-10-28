// <copyright file="TimeTrackingStorageTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the time tracking storage layer functionality.
    /// </summary>
    [TestClass]
    public class TimeTrackingStorageTest
    {
        /// <summary>
        /// Test SQL query validation for security restrictions.
        /// </summary>
        [TestMethod]
        public void SqlQueryValidation_ShouldRejectDangerousQueries()
        {
            // Arrange
            var validator = new MockSqlQueryValidator();

            // Act & Assert - Test forbidden operations
            Assert.IsFalse(validator.IsValidQuery("DELETE FROM UsageTime"));
            Assert.IsFalse(validator.IsValidQuery("UPDATE UsageTime SET Duration = 0"));
            Assert.IsFalse(validator.IsValidQuery("DROP TABLE UsageTime"));
            Assert.IsFalse(validator.IsValidQuery("INSERT INTO UsageTime VALUES (1, 'test', '2023-01-01', 100)"));
            Assert.IsFalse(validator.IsValidQuery("ALTER TABLE UsageTime ADD COLUMN NewCol TEXT"));
            Assert.IsFalse(validator.IsValidQuery("CREATE TABLE NewTable (Id INTEGER)"));
            Assert.IsFalse(validator.IsValidQuery("TRUNCATE TABLE UsageTime"));
        }

        /// <summary>
        /// Test SQL query validation for allowed operations.
        /// </summary>
        [TestMethod]
        public void SqlQueryValidation_ShouldAllowSafeQueries()
        {
            // Arrange
            var validator = new MockSqlQueryValidator();

            // Act & Assert - Test allowed operations
            Assert.IsTrue(validator.IsValidQuery("SELECT * FROM UsageTime"));
            Assert.IsTrue(validator.IsValidQuery("SELECT COUNT(*) FROM UsageTime"));
            Assert.IsTrue(validator.IsValidQuery("SELECT ApplicationIdentification, SUM(Duration) FROM UsageTime GROUP BY ApplicationIdentification"));
            Assert.IsTrue(validator.IsValidQuery("SELECT * FROM Application"));
            Assert.IsTrue(validator.IsValidQuery("SELECT * FROM Category"));
            Assert.IsTrue(validator.IsValidQuery("SELECT * FROM ApplicationArea"));
        }

        /// <summary>
        /// Test SQL query validation for injection attempts.
        /// </summary>
        [TestMethod]
        public void SqlQueryValidation_ShouldRejectInjectionAttempts()
        {
            // Arrange
            var validator = new MockSqlQueryValidator();

            // Act & Assert - Test SQL injection patterns
            Assert.IsFalse(validator.IsSafeQuery("SELECT * FROM UsageTime; DROP TABLE UsageTime;"));
            Assert.IsFalse(validator.IsSafeQuery("SELECT * FROM UsageTime UNION SELECT * FROM sqlite_master"));
            Assert.IsFalse(validator.IsSafeQuery("SELECT * FROM UsageTime WHERE 1=1 OR 1=1"));
            Assert.IsFalse(validator.IsSafeQuery("SELECT * FROM UsageTime /* comment */ WHERE Duration > 0"));
        }

        /// <summary>
        /// Test query field enumeration values.
        /// </summary>
        [TestMethod]
        public void QueryField_ShouldHaveExpectedValues()
        {
            // Act & Assert
            var fields = Enum.GetValues(typeof(MockQueryField));
            Assert.IsTrue(fields.Length >= 5);
            Assert.IsTrue(Enum.IsDefined(typeof(MockQueryField), "ApplicationIdentification"));
            Assert.IsTrue(Enum.IsDefined(typeof(MockQueryField), "WindowTitle"));
            Assert.IsTrue(Enum.IsDefined(typeof(MockQueryField), "ProcessName"));
            Assert.IsTrue(Enum.IsDefined(typeof(MockQueryField), "All"));
        }

        /// <summary>
        /// Test LIKE pattern conversion.
        /// </summary>
        [TestMethod]
        public void LikePatternConversion_ShouldWorkCorrectly()
        {
            // Arrange
            var converter = new MockPatternConverter();

            // Act & Assert
            Assert.AreEqual("%Visual%", converter.ConvertToLikePattern("*Visual*"));
            Assert.AreEqual("Visual%", converter.ConvertToLikePattern("Visual*"));
            Assert.AreEqual("%Visual", converter.ConvertToLikePattern("*Visual"));
            Assert.AreEqual("Visual_Studio", converter.ConvertToLikePattern("Visual?Studio"));
        }

        /// <summary>
        /// Test regex pattern validation.
        /// </summary>
        [TestMethod]
        public void RegexPatternValidation_ShouldWorkCorrectly()
        {
            // Arrange
            var validator = new MockPatternValidator();

            // Act & Assert
            Assert.IsTrue(validator.IsValidRegexPattern("Visual.*"));
            Assert.IsTrue(validator.IsValidRegexPattern("^Chrome$"));
            Assert.IsTrue(validator.IsValidRegexPattern("[Nn]otepad"));
            Assert.IsFalse(validator.IsValidRegexPattern("[invalid"));
            Assert.IsFalse(validator.IsValidRegexPattern("*invalid"));
        }

        /// <summary>
        /// Test cross-platform path handling.
        /// </summary>
        [TestMethod]
        public void CrossPlatformPaths_ShouldHandleCorrectly()
        {
            // Arrange
            var pathProvider = new MockStoragePathProvider();

            // Act
            var databasePath = pathProvider.GetDatabasePath();
            var configPath = pathProvider.GetConfigurationPath();

            // Assert
            Assert.IsNotNull(databasePath);
            Assert.IsNotNull(configPath);
            Assert.IsTrue(Path.IsPathRooted(databasePath));
            Assert.IsTrue(Path.IsPathRooted(configPath));
            Assert.IsTrue(databasePath.EndsWith(".db") || databasePath.EndsWith(".sqlite"));
        }

        /// <summary>
        /// Test query result structure.
        /// </summary>
        [TestMethod]
        public void QueryResult_ShouldHaveCorrectStructure()
        {
            // Arrange
            var result = new MockQueryResult();

            // Act
            result.Columns = new List<string> { "ApplicationIdentification", "Duration", "StartTime" };
            result.Rows = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "ApplicationIdentification", "Visual Studio" },
                    { "Duration", 3600 },
                    { "StartTime", DateTime.Now }
                }
            };
            result.RowCount = 1;
            result.ExecutionTime = TimeSpan.FromMilliseconds(50);

            // Assert
            Assert.AreEqual(3, result.Columns.Count);
            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(1, result.RowCount);
            Assert.IsTrue(result.ExecutionTime.TotalMilliseconds > 0);
            Assert.AreEqual("Visual Studio", result.Rows[0]["ApplicationIdentification"]);
        }
    }

    /// <summary>
    /// Mock SQL query validator for testing.
    /// </summary>
    public class MockSqlQueryValidator
    {
        private readonly HashSet<string> forbiddenKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "DROP", "DELETE", "UPDATE", "INSERT", "ALTER", "CREATE", "TRUNCATE",
            "EXEC", "EXECUTE", "CALL", "DECLARE", "CURSOR", "FETCH",
            "GRANT", "REVOKE", "COMMIT", "ROLLBACK", "TRANSACTION",
            "PRAGMA", "ATTACH", "DETACH", "VACUUM", "REINDEX"
        };

        private readonly HashSet<string> allowedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "UsageTime", "Application", "Category", "ApplicationArea", "CategoryUsageTime"
        };

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

            var normalizedQuery = sqlQuery.Trim().ToUpperInvariant();

            // Check for forbidden keywords
            if (this.forbiddenKeywords.Any(keyword => normalizedQuery.Contains(keyword)))
            {
                return false;
            }

            // Must start with SELECT
            if (!normalizedQuery.StartsWith("SELECT"))
            {
                return false;
            }

            // Check for allowed tables
            var hasAllowedTable = this.allowedTables.Any(table => normalizedQuery.Contains(table.ToUpperInvariant()));
            return hasAllowedTable;
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

            // Check for SQL injection patterns
            var injectionPatterns = new[]
            {
                @";\s*(DROP|DELETE|UPDATE|INSERT|ALTER|CREATE)",
                @"UNION\s+SELECT",
                @"/\*.*?\*/",
                @"--\s*[^\r\n]*"
            };

            return !injectionPatterns.Any(pattern => Regex.IsMatch(sqlQuery, pattern, RegexOptions.IgnoreCase));
        }
    }

    /// <summary>
    /// Mock query field enumeration for testing.
    /// </summary>
    public enum MockQueryField
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

    /// <summary>
    /// Mock pattern converter for testing.
    /// </summary>
    public class MockPatternConverter
    {
        /// <summary>
        /// Converts a wildcard pattern to a SQL LIKE pattern.
        /// </summary>
        /// <param name="wildcardPattern">The wildcard pattern.</param>
        /// <returns>The SQL LIKE pattern.</returns>
        public string ConvertToLikePattern(string wildcardPattern)
        {
            if (string.IsNullOrEmpty(wildcardPattern))
            {
                return wildcardPattern;
            }

            return wildcardPattern.Replace("*", "%").Replace("?", "_");
        }
    }

    /// <summary>
    /// Mock pattern validator for testing.
    /// </summary>
    public class MockPatternValidator
    {
        /// <summary>
        /// Validates if a regex pattern is valid.
        /// </summary>
        /// <param name="pattern">The regex pattern to validate.</param>
        /// <returns>True if the pattern is valid.</returns>
        public bool IsValidRegexPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            try
            {
                Regex.IsMatch("test", pattern);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Mock storage path provider for testing.
    /// </summary>
    public class MockStoragePathProvider
    {
        /// <summary>
        /// Gets the database file path.
        /// </summary>
        /// <returns>The database file path.</returns>
        public string GetDatabasePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, "SoluiNet", "DevTools", "TimeTracking", "timetracking.db");
        }

        /// <summary>
        /// Gets the configuration file path.
        /// </summary>
        /// <returns>The configuration file path.</returns>
        public string GetConfigurationPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, "SoluiNet", "DevTools", "TimeTracking", "config.json");
        }
    }

    /// <summary>
    /// Mock query result for testing.
    /// </summary>
    public class MockQueryResult
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