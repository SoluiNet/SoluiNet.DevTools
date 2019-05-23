// <copyright file="IFormatter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Formatter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for formatting.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Format a string.
        /// </summary>
        /// <param name="originalString">The string which should be formatted.</param>
        /// <returns>Returns a formatted string.</returns>
        string FormatString(string originalString);
    }
}
