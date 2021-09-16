// <copyright file="ITransformsData.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the interface for a plugin which can transform input data to a specific output format.
    /// </summary>
    public interface ITransformsData : IBasePlugin
    {
        /// <summary>
        /// Gets a list of all supported input formats.
        /// </summary>
        ICollection<string> SupportedInputFormats { get; }

        /// <summary>
        /// Gets a list of all supported output formats.
        /// </summary>
        ICollection<string> SupportedOutputFormats { get; }

        /// <summary>
        /// Transform an input file to the overgiven output format.
        /// </summary>
        /// <param name="inputFile">The input file path.</param>
        /// <param name="preferedOutputFormat">The output format to which the input file should be transformed to.</param>
        /// <returns>Returns an object which contains the data of the input file in the chosen output format.</returns>
        object Transform(string inputFile, string preferedOutputFormat);

        /// <summary>
        /// Transform a stream to the overgiven output format.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="inputFormat">The format in which the stream will be overgiven.</param>
        /// <param name="preferedOutputFormat">The output format to which the input stream should be transformed to.</param>
        /// <returns>Returns an object which contains the data of the input stream in the chosen output format.</returns>
        object Transform(string inputStream, string inputFormat, string preferedOutputFormat);
    }
}
