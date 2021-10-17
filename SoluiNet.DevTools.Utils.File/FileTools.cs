// <copyright file="FileTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.File
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods for files.
    /// </summary>
    public static class FileTools
    {
        /// <summary>
        /// Extract lines which contains the search pattern.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>Returns every line which contains the search pattern.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Whatever happens should be logged instead of crashing the application")]
        public static IList<string> ExtractLinesContainingSearchPattern(string filePath, string searchPattern)
        {
            var result = new List<string>();

            try
            {
                foreach (var line in System.IO.File.ReadAllLines(filePath))
                {
                    if (line.Contains(searchPattern))
                    {
                        result.Add(line);
                    }
                }
            }
            catch (Exception exception)
            {
                result.Add(string.Format(CultureInfo.InvariantCulture, "##ERROR## Couldn't extract lines from file {0} - {1} ##ERROR##", filePath, exception.Message));
            }

            return result;
        }

        /// <summary>
        /// Extract lines which match the RegEx.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="regExPattern">The RegEx search pattern.</param>
        /// <returns>Returns every line which matches the RegEx search pattern.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Stay with 'Ex' for better readability.")]
        public static IList<string> ExtractLinesMatchingRegEx(string filePath, string regExPattern)
        {
            var result = new List<string>();
            var regEx = new Regex(regExPattern);

            foreach (var line in System.IO.File.ReadAllLines(filePath))
            {
                if (regEx.IsMatch(line))
                {
                    result.Add(line);
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate the hash of a file.
        /// </summary>
        /// <param name="hashType">The hash type which should be used to calculate the hash.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns the hash of a file into a string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "We want to use MD5 here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "We want to receive the result in lower case.")]
        public static string CalculateChecksum(string hashType, string filePath)
        {
            //// todo: get hash type from enumeration

            if (string.IsNullOrEmpty(hashType))
            {
                throw new ArgumentNullException(nameof(hashType));
            }

            if (!System.IO.File.Exists(filePath))
            {
                return string.Empty;
            }

            if (hashType.ToUpperInvariant() == "MD5")
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = System.IO.File.OpenRead(filePath))
                    {
                        var hash = md5.ComputeHash(stream);

                        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                    }
                }
            }

            return string.Empty;
        }
    }
}
