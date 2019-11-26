// <copyright file="StringExtensions.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Provides a collection of methods to work with strings.
    /// </summary>
    public static class StringExtensions
    {
        private const string VariableFormat = "<\\[{0}\\]>";

        /// <summary>
        /// Check if the string variable is a SQL query.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL query.</returns>
        public static bool IsSqlQuery(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("SELECT", StringComparison.InvariantCulture) || sqlCommand.ToUpperInvariant().TrimStart().StartsWith("WITH", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL update.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL update.</returns>
        public static bool IsSqlUpdate(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("UPDATE", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL insert.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL insert.</returns>
        public static bool IsSqlInsert(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("INSERT", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL delete.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL delete.</returns>
        public static bool IsSqlDelete(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("DELETE", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL execute.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL execute.</returns>
        public static bool IsSqlExecute(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("EXEC", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL drop.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL drop.</returns>
        public static bool IsSqlDrop(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("DROP", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL create.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL create.</returns>
        public static bool IsSqlCreate(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("CREATE", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Check if the string variable is a SQL alter.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL alter.</returns>
        public static bool IsSqlAlter(this string sqlCommand)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentNullException(nameof(sqlCommand));
            }

            return sqlCommand.ToUpperInvariant().TrimStart().StartsWith("ALTER", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Replace environment placeholders in a string variable with the overgiven environment.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <param name="environment">The environment. If not provided it will default to "Default".</param>
        /// <returns>Returns a string with replaced environment placeholders.</returns>
        public static string SetEnvironment(this string originalString, string environment = "Default")
        {
            if (originalString == null)
            {
                throw new ArgumentNullException(nameof(originalString));
            }

            return originalString.Replace("[Environment]", environment);
        }

        /// <summary>
        /// Inject settings into a string variable.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <param name="settings">The settings object.</param>
        /// <returns>Returns a string with injected settings for variables (i. e. &lt;[Settings.Default.abc]&gt;).</returns>
        public static string InjectSettings(this string originalString, Settings.SoluiNetSettingType settings)
        {
            if (settings == null)
            {
                return originalString;
            }

            var settingsDictionary = new Dictionary<string, string>();

            foreach (var environment in settings.SoluiNetEnvironment)
            {
                foreach (var entry in environment.SoluiNetSettingEntry)
                {
                    settingsDictionary.Add(string.Format(CultureInfo.InvariantCulture, "Settings.{0}.{1}", environment.name, entry.name), entry.Value);
                }
            }

            return originalString.Inject(settingsDictionary);
        }

        /// <summary>
        /// Inject a dictionary into a string variable.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <param name="injectionValues">The dictionary with possible variable to inject.</param>
        /// <returns>Returns a string with injected variables (i.e. &lt;[variableName]&gt;).</returns>
        public static string Inject(this string originalString, Dictionary<string, string> injectionValues)
        {
            if (injectionValues == null)
            {
                throw new ArgumentNullException(nameof(injectionValues));
            }

            var adjustedString = originalString;

            foreach (var injection in injectionValues)
            {
                var replaceRegex = new Regex(string.Format(CultureInfo.InvariantCulture, VariableFormat, injection.Key));

                adjustedString = replaceRegex.Replace(adjustedString, injection.Value);
            }

            return adjustedString;
        }

        /// <summary>
        /// Inject common values into a string variable.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <returns>Returns a string with injected common values like UtcDateTime or UserName.</returns>
        public static string InjectCommonValues(this string originalString)
        {
            var commonValueDictionary = new Dictionary<string, string>()
            {
                { "UtcDateTime", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss", CultureInfo.InvariantCulture) },
                { "DateTime", DateTime.Now.ToString("yyyy-MM-dd\"T\"HH:mm:ss", CultureInfo.InvariantCulture) },
                { "UserName", Environment.UserName },
                { "MachineName", Environment.MachineName },
                { "CurrentDirectory", Environment.CurrentDirectory },
                { "UserDomainName", Environment.UserDomainName },
                { "Timestamp", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) },
                { "UtcTimestamp", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture) },
            };

            return originalString.Inject(commonValueDictionary);
        }
    }
}
