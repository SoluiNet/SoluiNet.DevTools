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
    using SoluiNet.DevTools.Core.Configuration;
    using SoluiNet.DevTools.Core.Configuration.Models;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// The main entrance point for the SoluiNet.DevTools.Console application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main method which should be called when executing this application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        public static void Main(string[] args)
        {
            IPlatformService platformService = null;
            IConfigurationManager configurationManager = null;
            
            try
            {
#if DEBUG && WINDOWS
                Debugger.Launch();
#endif
                // Initialize platform services
                platformService = PlatformServiceFactory.Create();
                
                // Configure cross-platform logging
                CrossPlatformNLogConfigurator.Configure(platformService, "console");
                
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info($"Starting SoluiNet.DevTools.Console on {GetPlatformName()}");
                
                // Initialize configuration manager
                configurationManager = new CrossPlatformConfigurationManager(platformService);
                
                // Perform configuration migration if needed
                PerformConfigurationMigration(configurationManager, platformService, logger);
                
                // Load or create application configuration
                LoadApplicationConfiguration(configurationManager, logger);
                
                ApplicationContext.Application = new ConsoleApplication();

                (ApplicationContext.Application as BaseSoluiNetApp).Initialize();

                CommandLine.Parser.Default.ParseArguments<RunOptions>(args)
                    .WithParsed(Run)
                    .WithNotParsed(Error);
            }
            catch (Exception exception)
            {
                var logger = LogManager.GetCurrentClassLogger();

                logger.Error(exception, string.Format(
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

        /// <summary>
        /// Performs configuration migration from Windows-specific locations if needed.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="platformService">The platform service.</param>
        /// <param name="logger">The logger.</param>
        private static void PerformConfigurationMigration(IConfigurationManager configurationManager, IPlatformService platformService, Logger logger)
        {
            try
            {
                var migrationService = new ConfigurationMigrationService(configurationManager, platformService);
                
                // Only attempt migration on non-Windows platforms or if no configuration exists
                if (!OperatingSystem.IsWindows() || !configurationManager.ConfigurationExists("application"))
                {
                    var migrated = migrationService.MigrateFromWindowsLocations();
                    if (migrated)
                    {
                        logger.Info("Successfully migrated configuration from Windows locations.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Configuration migration failed, but application will continue with default settings.");
            }
        }

        /// <summary>
        /// Loads or creates the application configuration.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The application configuration.</returns>
        private static ApplicationConfiguration LoadApplicationConfiguration(IConfigurationManager configurationManager, Logger logger)
        {
            try
            {
                var config = configurationManager.GetConfiguration<ApplicationConfiguration>("application");
                if (config == null)
                {
                    logger.Info("No existing application configuration found, creating default configuration.");
                    config = new ApplicationConfiguration();
                    configurationManager.SaveConfiguration("application", config);
                }
                else
                {
                    logger.Debug("Loaded existing application configuration.");
                }

                return config;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to load application configuration, using defaults.");
                return new ApplicationConfiguration();
            }
        }

        /// <summary>
        /// Gets a human-readable platform name.
        /// </summary>
        /// <returns>The platform name.</returns>
        private static string GetPlatformName()
        {
            if (OperatingSystem.IsWindows())
            {
                return "Windows";
            }

            if (OperatingSystem.IsLinux())
            {
                return "Linux";
            }

            if (OperatingSystem.IsMacOS())
            {
                return "macOS";
            }

            return "Unknown";
        }
    }
}