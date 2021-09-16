// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System.Reflection;

namespace SoluiNet.DevTools.Console
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using CommandLine;
    using SoluiNet.DevTools.Console.Options;

    /// <summary>
    /// The main entrance point for the SoluiNet.DevTools.Console application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main method which should be called when executing this application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<RunOptions>(args)
                .WithParsed(Run)
                .WithNotParsed(Error);
        }

        /// <summary>
        /// Run with parsed options.
        /// </summary>
        /// <param name="options">The parsed options.</param>
        internal static void Run(RunOptions options)
        {
            Console.WriteLine($@"SoluiNet.DevTools.Console v{Assembly.GetEntryAssembly()?.GetName().Version.ToString()}");
            Console.WriteLine($@"Current Arguments: -v {options.Verbose} -h {options.Help}");

            if (options.Help)
            {
                Console.WriteLine(@"You can use the following options:");
                Console.WriteLine(@"v, verbose   Use verbose output");
                Console.WriteLine(@"h, help      Open additional information about the usage of this application");
            }
        }

        /// <summary>
        /// Couldn't identify the options.
        /// </summary>
        /// <param name="errors">A enumerable which holds the errors.</param>
        internal static void Error(IEnumerable<Error> errors)
        {

        }
    }
}
