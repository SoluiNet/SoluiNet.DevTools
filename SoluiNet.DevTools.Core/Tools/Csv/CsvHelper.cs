// <copyright file="CsvHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    /// <summary>
    /// Provides a collection of methods to work with CSV data.
    /// </summary>
    public static class CsvHelper
    {
        public static DataTable TableFromCsvStream(
            IStream csvStream,
            bool containsHeaders = false,
            char delimiter = ',',
            char comment = '#',
            char quote = '"',
            string lineSeparator = "\r\n")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate a data table from CSV string.
        /// </summary>
        /// <param name="csvString">The CSV string.</param>
        /// <param name="containsHeaders">A value indicating whether the CSV string contains a header line.</param>
        /// <param name="delimiter">The delimiter character.</param>
        /// <param name="comment">The comment character.</param>
        /// <param name="quote">The quote character.</param>
        /// <param name="lineSeparator">The line separator string.</param>
        /// <param name="addLineNumber">A value indicating whether line numbers should be added or not.</param>
        /// <returns>Returns a <see cref="DataTable"/> which holds the data contained in the CSV string.</returns>
        /// <exception cref="ArgumentNullException">Returns ArgumentNullException if CSV string is null or empty.</exception>
        public static DataTable TableFromCsvString(
            string csvString,
            bool containsHeaders = false,
            char delimiter = ',',
            char comment = '#',
            char quote = '"',
            string lineSeparator = "\r\n",
            bool addLineNumber = false)
        {
            if (string.IsNullOrWhiteSpace(csvString))
            {
                throw new ArgumentNullException(nameof(csvString));
            }

            var result = new DataTable();

            var csvLines = csvString.Split(new[] { lineSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var lineNumber = 0;

            foreach (var line in csvLines)
            {
                var csvFields = line.Split(delimiter);

                if (csvFields.Length > 0)
                {
                    var row = result.Rows.Add();

                    for (var i = 0; i < csvFields.Length; i++)
                    {
                        if (result.Columns.Count < csvFields.Length)
                        {
                            if (lineNumber == 0 && containsHeaders)
                            {
                                result.Columns.Add(new DataColumn(csvFields[i]));
                            }
                            else
                            {
                                result.Columns.Add(new DataColumn());
                            }
                        }

                        row[i] = csvFields[i];
                    }
                }

                lineNumber++;
            }

            return result;
        }
    }
}
