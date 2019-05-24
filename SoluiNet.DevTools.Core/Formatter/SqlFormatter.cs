// <copyright file="SqlFormatter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// A formatter for SQL texts.
    /// </summary>
    public class SqlFormatter : IFormatter
    {
        private string indentationString = string.Empty;
        private char indentationCharacter = ' ';
        private int indentationLength = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlFormatter"/> class.
        /// </summary>
        public SqlFormatter()
        {
            this.CalculateIndentationString();
        }

        /// <summary>
        /// Gets or sets the indentation character.
        /// </summary>
        public char IndentationCharacter
        {
            get
            {
                return this.indentationCharacter;
            }

            set
            {
                this.indentationCharacter = value;
                this.CalculateIndentationString();
            }
        }

        /// <summary>
        /// Gets or sets the indentation length.
        /// </summary>
        public int IndentationLength
        {
            get
            {
                return this.indentationLength;
            }

            set
            {
                this.indentationLength = value;
                this.CalculateIndentationString();
            }
        }

        /// <summary>
        /// Gets the indentation string.
        /// </summary>
        public string IndentationString
        {
            get { return this.indentationString; }
        }

        /// <summary>
        /// Calculates the indentation string from indentation length and indentation character.
        /// </summary>
        public void CalculateIndentationString()
        {
            for (var i = 0; i < this.IndentationLength; i++)
            {
                this.indentationString += this.IndentationCharacter;
            }
        }

        /// <summary>
        /// Get the indentation string by indentation level.
        /// </summary>
        /// <param name="indentationLevel">The indentation level.</param>
        /// <returns>Returns a string which contains the indentation string as often as the overgiven indentation level.</returns>
        public string GetIndentation(int indentationLevel)
        {
            var indentation = string.Empty;

            for (var i = 0; i < indentationLevel; i++)
            {
                indentation += this.IndentationString;
            }

            return indentation;
        }

        /// <summary>
        /// Format a SQL string.
        /// </summary>
        /// <param name="originalString">The original SQL string.</param>
        /// <returns>Returns a formatted SQL string according to the properties which have been set up for this class.</returns>
        public string FormatString(string originalString)
        {
            var formatOptions = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatterOptions()
            {
                TrailingCommas = true,
                BreakJoinOnSections = true,
                IndentString = this.IndentationString,
                UppercaseKeywords = true,
                SpacesPerTab = 2,
            };

            var formatter = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter(formatOptions);

            var formattingManager = new PoorMansTSqlFormatterLib.SqlFormattingManager(formatter);

            return formattingManager.Format(originalString);
        }
    }
}
