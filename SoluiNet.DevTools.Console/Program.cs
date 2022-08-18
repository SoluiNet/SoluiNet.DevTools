// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Console
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using CommandLine;
    using NLog;
    using SoluiNet.DevTools.Console.Options;
    using SoluiNet.DevTools.Core.Application;

    /// <summary>
    /// The main entrance point for the SoluiNet.DevTools.Console application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main method which should be called when executing this application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        public static void Main(string[] args)
        {
            try
            {
#if DEBUG
                Debugger.Launch();
#endif
                ApplicationContext.Application = new ConsoleApplication();

                (ApplicationContext.Application as BaseSoluiNetApp).Initialize();

                CommandLine.Parser.Default.ParseArguments<RunOptions>(args)
                    .WithParsed(Run)
                    .WithNotParsed(Error);
            }
            catch (Exception exception)
            {
                var logger = LogManager.GetCurrentClassLogger();

                logger.Error(string.Format(
                    CultureInfo.InvariantCulture,
                    "Error while executing SoluiNet.DevTools.Console - {0}",
                    exception.ToString()));

                Console.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "Error while executing SoluiNet.DevTools.Console - {0}",
                    exception.ToString()));
            }
        }

        /// <summary>
        /// Run with parsed options.
        /// </summary>
        /// <param name="options">The parsed options.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We want to provide a neutral console tool. So there won't be any localizations for now.")]
        internal static void Run(RunOptions options)
        {
            Console.WriteLine($@"SoluiNet.DevTools.Console v{Assembly.GetEntryAssembly()?.GetName().Version.ToString()}");
            Console.WriteLine($@"Current Arguments: -v {options.Verbose} -h {options.Help}");

            if (options.Help)
            {
                Console.WriteLine(@"You can use the following options:");
                Console.WriteLine(@"v, verbose   Use verbose output");
                Console.WriteLine(@"h, help      Open additional information about the usage of this application");

                foreach (var plugin in (ApplicationContext.Application.Plugins as ConsoleApplication).CommandLinePlugins)
                {
                    Console.WriteLine(plugin.HelpText);
                }
            }
        }

        /// <summary>
        /// Couldn't identify the options.
        /// </summary>
        /// <param name="errors">A enumerable which holds the errors.</param>
        internal static void Error(IEnumerable<Error> errors)
        {
            var logger = LogManager.GetCurrentClassLogger();

            foreach (var error in errors)
            {
                logger.Error(string.Format(
                    CultureInfo.InvariantCulture,
                    "Error while executing SoluiNet.DevTools.Console - Run - {0} (stops processing: {1})",
                    error.Tag.ToString(),
                    error.StopsProcessing));
            }
        }
    }
}