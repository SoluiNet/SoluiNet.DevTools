// <copyright file="CsvHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Csv
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using global::CsvHelper;
    using global::CsvHelper.Configuration;
    using SoluiNet.DevTools.Core.Tools.Stream;

    /// <summary>
    /// Provides a collection of methods to work with CSV data.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1724:Type names should not match namespaces", Justification = "We want to stay with the class name because other helper classes use the same name scheme.")]
    public static class CsvHelper
    {
        /// <summary>
        /// Generate a data table from CSV stream.
        /// </summary>
        /// <param name="csvStream">The CSV stream.</param>
        /// <param name="containsHeaders">A value indicating whether the CSV stream contains a header line.</param>
        /// <param name="delimiter">The delimiter character.</param>
        /// <param name="comment">The comment character.</param>
        /// <param name="quote">The quote character.</param>
        /// <param name="quotesNeeded">A value indicating whether quotes are needed.</param>
        /// <param name="lineSeparator">The line separator string.</param>
        /// <param name="addLineNumber">A value indicating whether line numbers should be added or not.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns a <see cref="DataTable"/> which holds the data contained in the CSV stream.</returns>
        /// <exception cref="ArgumentNullException">Returns ArgumentNullException if CSV stream is null or empty.</exception>
        /// <exception cref="NotImplementedException">Returns NotImplementedException if the implementation hasn't been finished until now.</exception>
        public static DataTable TableFromCsvStream(
            Stream csvStream,
            bool containsHeaders = false,
            char delimiter = ',',
            char comment = '#',
            char quote = '"',
            bool quotesNeeded = true,
            string lineSeparator = "\r\n",
            bool addLineNumber = false,
            Encoding encoding = null)
        {
            if (csvStream == null)
            {
                throw new ArgumentNullException(nameof(csvStream));
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var result = new DataTable();

            var csvConfig = new CsvConfiguration(new CultureInfo("de-DE"))
            {
                Comment = comment,
                Delimiter = delimiter.ToString(),
                Encoding = encoding,
                HasHeaderRecord = containsHeaders,
                NewLine = lineSeparator,
                ShouldQuote = (args) => { return quotesNeeded; },
                Quote = quote,
            };

            using (var csvReader = new CsvReader(new StreamReader(csvStream, encoding), csvConfig))
            {
                csvReader.Read();

                if (containsHeaders)
                {
                    csvReader.ReadHeader();
                }

                var lineNumber = 0;

                while (csvReader.Read())
                {
                    var row = result.Rows.Add();

                    if (addLineNumber)
                    {
                        if (result.Columns.Count < 1)
                        {
                            result.Columns.Add(new DataColumn("LineNo", typeof(int)));
                        }

                        row[0] = lineNumber;
                    }

                    // fix: csvReader.ColumnCount doesn't seem to be updated correctly - we will use the count of the parser.
                    var columnCount = csvReader.Parser.Count;

                    for (var i = 0; i < columnCount; i++)
                    {
                        if (columnCount + (addLineNumber ? 1 : 0) > result.Columns.Count + (addLineNumber ? 1 : 0))
                        {
                            if (containsHeaders)
                            {
                                result.Columns.Add(new DataColumn(csvReader.HeaderRecord[i]));
                            }
                            else
                            {
                                result.Columns.Add(new DataColumn());
                            }
                        }

                        row[i + (addLineNumber ? 1 : 0)] = csvReader.GetField(i);
                    }

                    lineNumber++;
                }
            }

            return result;
        }

        /// <summary>
        /// Generate a data table from CSV string.
        /// </summary>
        /// <param name="csvString">The CSV string.</param>
        /// <param name="containsHeaders">A value indicating whether the CSV string contains a header line.</param>
        /// <param name="delimiter">The delimiter character.</param>
        /// <param name="comment">The comment character.</param>
        /// <param name="quote">The quote character.</param>
        /// <param name="quotesNeeded">A value indicating whether quotes are needed.</param>
        /// <param name="lineSeparator">The line separator string.</param>
        /// <param name="addLineNumber">A value indicating whether line numbers should be added or not.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns a <see cref="DataTable"/> which holds the data contained in the CSV string.</returns>
        /// <exception cref="ArgumentNullException">Returns ArgumentNullException if CSV string is null or empty.</exception>
        public static DataTable TableFromCsvString(
            string csvString,
            bool containsHeaders = false,
            char delimiter = ',',
            char comment = '#',
            char quote = '"',
            bool quotesNeeded = true,
            string lineSeparator = "\r\n",
            bool addLineNumber = false,
            Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(csvString))
            {
                throw new ArgumentNullException(nameof(csvString));
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return TableFromCsvStream(
                csvString.AsStream(),
                containsHeaders,
                delimiter,
                comment,
                quote,
                quotesNeeded,
                lineSeparator,
                addLineNumber);
        }
    }
}
