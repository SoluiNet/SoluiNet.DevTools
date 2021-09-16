// <copyright file="RunOptions.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Console.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CommandLine;

    /// <summary>
    /// The run options for the SoluiNet.DevTools.Console application.
    /// </summary>
    public class RunOptions
    {
        /// <summary>
        /// Gets or sets whether the application should be verbose or not.
        /// </summary>
        [Option(
            'v',
            "verbose",
            Default = false,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets whether the application should be showing the help or not.
        /// </summary>
        [Option(
            'h',
            "help",
            Default = false,
            HelpText = "Show help information.")]
        public bool Help { get; set; }
    }
}
