// <copyright file="StringHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.String
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods which support working with strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Replace the first occurence of a RegEx-pattern.
        /// </summary>
        /// <param name="inputString">The string in which should be searched.</param>
        /// <param name="searchPattern">The search pattern (RegEx).</param>
        /// <param name="replacementValue">The replacement value. If not overgiven an empty string will be used.</param>
        /// <returns>The <paramref name="inputString"/> in which the first occurence of <paramref name="searchPattern"/> has been replaced with <paramref name="replacementValue"/>.</returns>
        public static string ReplaceFirstOccurence(string inputString, string searchPattern, string replacementValue = "")
        {
            var regEx = new Regex(Regex.Escape(searchPattern));

            return regEx.Replace(inputString, replacementValue, 1);
        }

        /// <summary>
        /// Prepare the header label of visual components.
        /// </summary>
        /// <param name="headerLabel">The header label.</param>
        /// <param name="removeEntityStructure">A value which indicated if the entity structure should be removed. Only the last entity name will be used.</param>
        /// <returns>Returns a <see cref="string"/> which will be displayed correctly in the header of a visual component.</returns>
        public static string PrepareHeaderLabel(string headerLabel, bool removeEntityStructure = true)
        {
            var headerLabelString = headerLabel;

            if (headerLabelString.Contains('.') && removeEntityStructure)
            {
                var positionOfLastDot = headerLabelString.LastIndexOf('.');

                headerLabelString = headerLabelString.Substring(positionOfLastDot + 1, headerLabelString.Length - positionOfLastDot - 1);
            }

            return headerLabelString.Replace("_", "__");
        }

        /// <summary>
        /// Add line numbers to a string.
        /// </summary>
        /// <param name="originalString">The string which should be extended with line numbers.</param>
        /// <param name="numberOfDigits">The number of digits which should be used for the line number.</param>
        /// <returns>Returns a <see cref="string"/> which represents the <paramref name="originalString"/> with line numbers at the beginning of each line.</returns>
        public static string AddLineNumbers(this string originalString, int numberOfDigits = 2)
        {
            var temporaryString = string.Empty;
            var lineNumber = 1;

            foreach (var line in originalString.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None))
            {
                temporaryString += string.Format("{0}: {1}\r\n", lineNumber++.ToString("D" + numberOfDigits.ToString()), line);
            }

            return temporaryString;
        }

        /// <summary>
        /// Convert a string to a base64 value.
        /// </summary>
        /// <param name="originalString">The string which should be casted to base64.</param>
        /// <returns>A <see cref="string"/> which represents the base64 value for <paramref name="originalString"/>.</returns>
        public static string ToBase64(this string originalString)
        {
            var plainTextBytes = Encoding.Unicode.GetBytes(originalString);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Convert a string from base64 to its' original value.
        /// </summary>
        /// <param name="encodedString">The string which should be converted from base64.</param>
        /// <returns>A <see cref="string"/> which represents the original value for <paramref name="encodedString"/>.</returns>
        public static string FromBase64(this string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.Unicode.GetString(data);
        }
    }
}
