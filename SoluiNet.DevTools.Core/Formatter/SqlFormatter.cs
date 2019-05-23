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
        private string _indentationString = string.Empty;
        private char _indentationCharacter = ' ';
        private int _indentationLength = 2;

        public char IndentationCharacter
        {
            get
            {
                return this._indentationCharacter;
            }

            set
            {
                this._indentationCharacter = value;
                this.CalculateIndentationString();
            }
        }

        public int IndentationLength
        {
            get
            {
                return this._indentationLength;
            }

            set
            {
                this._indentationLength = value;
                this.CalculateIndentationString();
            }
        }

        public string IndentationString
        {
            get { return this._indentationString; }
        }

        public void CalculateIndentationString()
        {
            for (var i = 0; i < this.IndentationLength; i++)
            {
                this._indentationString += this.IndentationCharacter;
            }
        }

        public string GetIndentation(int indentationLevel)
        {
            var indentation = string.Empty;

            for (var i = 0; i < indentationLevel; i++)
            {
                indentation += this.IndentationString;
            }

            return indentation;
        }

        public SqlFormatter()
        {
            this.CalculateIndentationString();
        }

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
