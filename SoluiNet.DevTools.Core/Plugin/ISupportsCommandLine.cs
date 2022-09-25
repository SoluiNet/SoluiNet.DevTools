// <copyright file="ISupportsCommandLine.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Enums;

    /// <summary>
    /// Provides an interface for a plugin which supports command line parameters.
    /// </summary>
    public interface ISupportsCommandLine : IBasePlugin
    {
        /// <summary>
        /// Gets the help text.
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// Parse and run the command.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>Returns a integer-based code for the execution result of the command.</returns>
        int RunCommandLine(IDictionary<string, string> arguments);
    }
}
