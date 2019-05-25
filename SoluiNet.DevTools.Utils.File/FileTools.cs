// <copyright file="FileTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.File
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods for files.
    /// </summary>
    public class FileTools
    {
        /// <summary>
        /// Extract lines which contains the search pattern.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>Returns every line which contains the search pattern.</returns>
        public static List<string> ExtractLinesContainingSearchPattern(string filePath, string searchPattern)
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
                result.Add(string.Format("##ERROR## Couldn't extract lines from file {0} - {1} ##ERROR##", filePath, exception.Message));
            }

            return result;
        }

        /// <summary>
        /// Extract lines which match the RegEx.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="regExPattern">The RegEx search pattern.</param>
        /// <returns>Returns every line which matches the RegEx search pattern.</returns>
        public static List<string> ExtractLinesMatchingRegEx(string filePath, string regExPattern)
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
        public static string CalculateChecksum(string hashType, string filePath)
        {
            //// todo: get hash type from enumeration

            if (!System.IO.File.Exists(filePath))
            {
                return string.Empty;
            }

            if (hashType.ToLowerInvariant() == "md5")
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
