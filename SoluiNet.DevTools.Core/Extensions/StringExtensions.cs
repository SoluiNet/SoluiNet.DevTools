// <copyright file="StringExtensions.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

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
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("select");
        }

        /// <summary>
        /// Check if the string variable is a SQL update.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL update.</returns>
        public static bool IsSqlUpdate(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("update");
        }

        /// <summary>
        /// Check if the string variable is a SQL insert.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL insert.</returns>
        public static bool IsSqlInsert(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("insert");
        }

        /// <summary>
        /// Check if the string variable is a SQL delete.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL delete.</returns>
        public static bool IsSqlDelete(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("delete");
        }

        /// <summary>
        /// Check if the string variable is a SQL execute.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL execute.</returns>
        public static bool IsSqlExecute(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("exec");
        }

        /// <summary>
        /// Check if the string variable is a SQL drop.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL drop.</returns>
        public static bool IsSqlDrop(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("drop");
        }

        /// <summary>
        /// Check if the string variable is a SQL create.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL create.</returns>
        public static bool IsSqlCreate(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("create");
        }

        /// <summary>
        /// Check if the string variable is a SQL alter.
        /// </summary>
        /// <param name="sqlCommand">The string variable.</param>
        /// <returns>Returns true if the string variable contains a SQL alter.</returns>
        public static bool IsSqlAlter(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("alter");
        }

        /// <summary>
        /// Replace environment placeholders in a string variable with the overgiven environment.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <param name="environment">The environment. If not provided it will default to "Default".</param>
        /// <returns>Returns a string with replaced environment placeholders.</returns>
        public static string SetEnvironment(this string originalString, string environment = "Default")
        {
            return originalString.Replace("[Environment]", environment);
        }

        /// <summary>
        /// Inject settings into a string variable.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <param name="settings">The settings object.</param>
        /// <returns>Returns a string with injected settings for variables (i. e. <[Settings.Default.abc]>).</returns>
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
                    settingsDictionary.Add(string.Format("Settings.{0}.{1}", environment.name, entry.name), entry.Value);
                }
            }

            return originalString.Inject(settingsDictionary);
        }

        /// <summary>
        /// Inject a dictionary into a string variable.
        /// </summary>
        /// <param name="originalString">The string variable.</param>
        /// <param name="injectionValues">The dictionary with possible variable to inject.</param>
        /// <returns>Returns a string with injected variables (i.e. <[variableName]>)</returns>
        public static string Inject(this string originalString, Dictionary<string, string> injectionValues)
        {
            var adjustedString = originalString;

            foreach (var injection in injectionValues)
            {
                var replaceRegex = new Regex(string.Format(VariableFormat, injection.Key));

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
                { "UtcDateTime", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss") },
                { "DateTime", DateTime.Now.ToString("yyyy-MM-dd\"T\"HH:mm:ss") },
                { "UserName", Environment.UserName },
                { "MachineName", Environment.MachineName },
                { "CurrentDirectory", Environment.CurrentDirectory },
                { "UserDomainName", Environment.UserDomainName },
                { "Timestamp", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString() },
                { "UtcTimestamp", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() },
            };

            return originalString.Inject(commonValueDictionary);
        }
    }
}
