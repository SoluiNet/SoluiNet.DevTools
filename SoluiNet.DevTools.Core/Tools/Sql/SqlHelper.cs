// <copyright file="SqlHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of useful methods to work with SQL.
    /// </summary>
    public static class SqlHelper
    {
        private const string EXECUTE_SCRIPT_PART_KEYWORD = "GO";
        private const string VARIABLE_DECLARATION_KEYWORD = "DECLARE";

        /// <summary>
        /// Check if the string is a valid SQL script.
        /// </summary>
        /// <param name="sqlCommand">The string which should be checked.</param>
        /// <returns>A <see cref="bool"/> value which is true if the <see cref="string"/> is a SQL script.</returns>
        public static bool IsScript(this string sqlCommand)
        {
            if (sqlCommand.Contains(EXECUTE_SCRIPT_PART_KEYWORD))
            {
                return true;
            }

            if (sqlCommand.Contains(VARIABLE_DECLARATION_KEYWORD))
            {
                return true;
            }

            if (sqlCommand.ContainsDdlCommand())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get a list of all identifiable single SQL scripts.
        /// </summary>
        /// <param name="sqlCommand">The string which may contain multiple scripts.</param>
        /// <returns>A <see cref="List{T}"/> where each entry represents a single SQL script.</returns>
        public static List<string> GetSingleScripts(this string sqlCommand)
        {
            return sqlCommand.Split(new string[] { EXECUTE_SCRIPT_PART_KEYWORD }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart().TrimEnd()).ToList();
        }

        /// <summary>
        /// Check if the string contains a DDL command.
        /// </summary>
        /// <param name="sqlCommand">The string which should be checked.</param>
        /// <returns>A <see cref="bool"/> value which is true if the <see cref="string"/> contains a DDL command.</returns>
        public static bool ContainsDdlCommand(this string sqlCommand)
        {
            var ddlRegex = new Regex("(CREATE|ALTER|DROP) (VIEW|STORED PROCEDURE)", RegexOptions.IgnoreCase);

            if (ddlRegex.IsMatch(sqlCommand))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extract the table name for the overgiven alias from the SQL command.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="alias">The alias.</param>
        /// <returns>Returns the original table name for the overgiven alias.</returns>
        public static string GetTableByAlias(string sqlCommand, string alias)
        {
            // ignore SQL functions
            if (alias.Contains('('))
            {
                alias = alias.Split('(').Last();
            }

            Regex aliasDefinitionRegex = new Regex(string.Format(@"(FROM|JOIN)\s+(.*)\s+(AS\s+)?({0})", alias), RegexOptions.IgnoreCase);

            if (aliasDefinitionRegex.IsMatch(sqlCommand))
            {
                var aliasMatch = aliasDefinitionRegex.Match(sqlCommand);

                return aliasMatch.Groups[2].Value;
            }

            return alias;
        }

        /// <summary>
        /// Get the SQL command at the current cursor position.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="cursorPosition">The current cursor position.</param>
        /// <returns>Returns the SQL command at the current cursor position.</returns>
        public static string GetSqlCommandByPosition(string sqlCommand, int cursorPosition)
        {
            var beginningOfSqlRegex = new Regex(@"^(\r?\n)?[^\(]?SELECT", RegexOptions.IgnoreCase);
            var endOfSqlRegex = new Regex(@"((;)|(\r?\n\r?\n))$", RegexOptions.IgnoreCase);

            var sqlCmd = string.Empty;

            sqlCmd += sqlCommand[cursorPosition - 1];

            var beginningOfCommandFound = false;
            var countOfCharacters = 0;

            while (!beginningOfCommandFound)
            {
                if (cursorPosition - 1 <= countOfCharacters)
                {
                    return string.Empty;
                }

                sqlCmd = sqlCommand[cursorPosition - ++countOfCharacters - 1] + sqlCmd;

                if (beginningOfSqlRegex.IsMatch(sqlCmd))
                {
                    beginningOfCommandFound = true;
                }
            }

            var endOfCommandFound = false;
            countOfCharacters = 0;

            while (!endOfCommandFound)
            {
                if (sqlCommand.Length <= cursorPosition + countOfCharacters)
                {
                    return string.Empty;
                }

                sqlCmd += sqlCommand[cursorPosition + ++countOfCharacters - 1];

                if (endOfSqlRegex.IsMatch(sqlCmd))
                {
                    endOfCommandFound = true;
                }
            }

            return sqlCmd;
        }

        /// <summary>
        /// Get the alias for the current cursor positon.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="cursorPosition">The current cursor position.</param>
        /// <returns>Returns the alias at the current cursor position.</returns>
        public static string GetAliasByPosition(string sqlCommand, int cursorPosition)
        {
            var aliasRegex = new Regex("^\\s(([\\\"]?.+[\\\"]?)|(.+))\\.", RegexOptions.IgnoreCase);

            var alias = string.Empty;

            alias += sqlCommand[cursorPosition - 1];

            var aliasFound = false;
            var countOfCharacters = 0;

            while (!aliasFound)
            {
                if (cursorPosition - 1 <= countOfCharacters)
                {
                    return string.Empty;
                }

                alias = sqlCommand[cursorPosition - ++countOfCharacters - 1] + alias;

                if (aliasRegex.IsMatch(alias))
                {
                    aliasFound = true;
                }
            }

            return alias.Substring(1, alias.Length - 2);
        }
    }
}
