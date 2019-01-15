using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Formatter
{
    public class SqlFormatter : IFormatter
    {
        private string _indentationString = string.Empty;
        private char _indentationCharacter = ' ';
        private int _indentationLength = 2;

        public char IndentationCharacter
        {
            get { return _indentationCharacter; }
            set
            {
                _indentationCharacter = value;
                CalculateIndentationString();
            }
        }

        public int IndentationLength
        {
            get { return _indentationLength; }
            set
            {
                _indentationLength = value;
                CalculateIndentationString();
            }
        }

        public string IndentationString
        {
            get { return _indentationString; }
        }

        public void CalculateIndentationString()
        {
            for (var i = 0; i < IndentationLength; i++)
            {
                _indentationString += IndentationCharacter;
            }
        }

        public string GetIndentation(int indentationLevel)
        {
            var indentation = string.Empty;

            for (var i = 0; i < indentationLevel; i++)
            {
                indentation += IndentationString;
            }

            return indentation;
        }

        public SqlFormatter()
        {
            CalculateIndentationString();
        }

        public string FormatString(string originalString)
        {
            var formatOptions = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatterOptions()
            {
                TrailingCommas = true,
                BreakJoinOnSections = true
            };

            var formatter = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter(formatOptions);

            var formattingManager = new PoorMansTSqlFormatterLib.SqlFormattingManager(formatter);

            return formattingManager.Format(originalString);
        }
    }
}
