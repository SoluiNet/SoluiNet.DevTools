// <copyright file="StringHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.String
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
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
            if (headerLabel == null)
            {
                throw new ArgumentNullException(nameof(headerLabel));
            }

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
            if (originalString == null)
            {
                throw new ArgumentNullException(nameof(originalString));
            }

            var temporaryString = string.Empty;
            var lineNumber = 1;

            foreach (var line in originalString.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None))
            {
                temporaryString += string.Format(CultureInfo.InvariantCulture, "{0}: {1}\r\n", lineNumber++.ToString("D" + numberOfDigits.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), line);
            }

            return temporaryString.Remove(temporaryString.Length - 2, 2);
        }

        /// <summary>
        /// Convert a string to a base64 value.
        /// </summary>
        /// <param name="originalString">The string which should be casted to base64.</param>
        /// <returns>A <see cref="string"/> which represents the base64 value for <paramref name="originalString"/>.</returns>
        public static string ToBase64(this string originalString)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(originalString);
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
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Get the number of seconds which are represented by the duration string.
        /// </summary>
        /// <param name="durationString">The duration string.</param>
        /// <returns>Returns the number of seconds which the duration string represents.</returns>
        public static long GetSecondsFromDurationString(this string durationString)
        {
            long duration = 0;

            var durationRegEx = new Regex(@"((\d+)w)?\s*((\d+)d)?\s*((\d+)h)?\s*((\d+)m)?\s*((\d+)s?)");

            var match = durationRegEx.Match(durationString);

            if (match.Success)
            {
                if (match.Groups[2].Success)
                {
                    duration += Convert.ToInt32(match.Groups[2].Value, CultureInfo.InvariantCulture) * 7 * 24 * 60 * 60;
                }

                if (match.Groups[4].Success)
                {
                    duration += Convert.ToInt32(match.Groups[4].Value, CultureInfo.InvariantCulture) * 24 * 60 * 60;
                }

                if (match.Groups[6].Success)
                {
                    duration += Convert.ToInt32(match.Groups[6].Value, CultureInfo.InvariantCulture) * 60 * 60;
                }

                if (match.Groups[8].Success)
                {
                    duration += Convert.ToInt32(match.Groups[8].Value, CultureInfo.InvariantCulture) * 60;
                }

                if (match.Groups[10].Success)
                {
                    duration += Convert.ToInt32(match.Groups[10].Value, CultureInfo.InvariantCulture);
                }
            }

            return duration;
        }

        /// <summary>
        /// Check if a regular expression matches an overgiven string.
        /// </summary>
        /// <param name="regExPattern">The regular expression.</param>
        /// <param name="searchString">The string which should be checked.</param>
        /// <returns>Returns true if the search string matches the regular expression.</returns>
        public static bool RegExMatch(this string regExPattern, string searchString)
        {
            if (string.IsNullOrEmpty(regExPattern) || string.IsNullOrEmpty(searchString))
            {
                return false;
            }

            var regEx = new Regex(regExPattern);

            return regEx.IsMatch(searchString);
        }

        /// <summary>
        /// Check if a regular expression matches an overgiven string.
        /// </summary>
        /// <param name="searchString">The string which should be checked.</param>
        /// <param name="regExPattern">The regular expression.</param>
        /// <returns>Returns true if the search string matches the regular expression.</returns>
        public static bool MatchesRegEx(this string searchString, string regExPattern)
        {
            if (string.IsNullOrEmpty(regExPattern) || string.IsNullOrEmpty(searchString))
            {
                return false;
            }

            var regEx = new Regex(regExPattern);

            return regEx.IsMatch(searchString);
        }

        /// <summary>
        /// Check if a regular expression matches an overgiven string.
        /// </summary>
        /// <param name="originalString">The string which should be replaced.</param>
        /// <param name="regExPattern">The regular expression.</param>
        /// <param name="replacementString">The replacement string.</param>
        /// <returns>The string with replacement.</returns>
        public static string ReplaceRegEx(this string originalString, string regExPattern, string replacementString = "")
        {
            if (string.IsNullOrEmpty(regExPattern) || string.IsNullOrEmpty(originalString))
            {
                return originalString;
            }

            var regEx = new Regex(regExPattern);

            return regEx.Replace(originalString, replacementString);
        }

        /// <summary>
        /// Returns true if the string has an affirmative value (1 / true / wahr / y / yes).
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns>If the string represents an affirmative value return true, otherwise false.</returns>
        public static bool IsAffirmative(this string stringValue)
        {
            if (stringValue == null)
            {
                throw new ArgumentNullException(nameof(stringValue));
            }

            if (stringValue.ToUpperInvariant() == "1")
            {
                return true;
            }

            if (stringValue.ToUpperInvariant() == "TRUE")
            {
                return true;
            }

            if (stringValue.ToUpperInvariant() == "WAHR")
            {
                return true;
            }

            if (stringValue.ToUpperInvariant() == "Y")
            {
                return true;
            }

            if (stringValue.ToUpperInvariant() == "YES")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generate a random string (taken from https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings).
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <returns>Returns a random string.</returns>
        public static string GetRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Get stream for given string (taken from https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string).
        /// The stream must be used via using (i. e. using (var stream = exampleString.GetStreamForString()){ // do something }).
        /// </summary>
        /// <param name="originalText">The original text.</param>
        /// <returns>Returns a stream from the given string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The purpose of this method is that the objects will be disposed outside")]
        public static Stream GetStreamForString(this string originalText)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(originalText);
            writer.Flush();

            stream.Position = 0;

            return stream;
        }
    }
}
