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
            Logger logger = null;

            try
            {
                // Conditional debugger launch only on Windows in debug mode
#if DEBUG
                if (PlatformHelper.IsWindows)
                {
                    Debugger.Launch();
                }
#endif

                // Initialize platform services
                platformService = PlatformServiceFactory.Create();

                // Configure cross-platform logging
                CrossPlatformNLogConfigurator.Configure(platformService, "console");

                logger = LogManager.GetCurrentClassLogger();
                
                // Log detailed platform information for troubleshooting
                LogPlatformInformation(logger);

                // Initialize configuration manager
                configurationManager = new CrossPlatformConfigurationManager(platformService);

                // Perform configuration migration if needed
                PerformConfigurationMigration(configurationManager, platformService, logger);

                // Load or create application configuration
                LoadApplicationConfiguration(configurationManager, logger);

                ApplicationContext.Application = new ConsoleApplication();

                (ApplicationContext.Application as BaseSoluiNetApp).Initialize();

                // Enhanced command-line parsing with cross-platform error handling
                var parseResult = CommandLine.Parser.Default.ParseArguments<RunOptions>(args);
                parseResult
                    .WithParsed(options => Run(options, logger))
                    .WithNotParsed(errors => Error(errors, logger));
            }
            catch (PlatformNotSupportedException platformException)
            {
                HandlePlatformException(platformException, logger);
            }
            catch (UnauthorizedAccessException accessException)
            {
                HandleAccessException(accessException, logger);
            }
            catch (System.IO.DirectoryNotFoundException dirException)
            {
                HandleDirectoryException(dirException, logger, platformService);
            }
            catch (Exception exception)
            {
                HandleGeneralException(exception, logger, platformService);
            }
        }

        /// <summary>
        /// Run with parsed options.
        /// </summary>
        /// <param name="options">The parsed options.</param>
        /// <param name="logger">The logger instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We want to provide a neutral console tool. So there won't be any localizations for now.")]
        internal static void Run(RunOptions options, Logger logger = null)
        {
            try
            {
                var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown";
                var platformInfo = PlatformHelper.GetDetailedPlatformInfo();
                
                Console.WriteLine($@"SoluiNet.DevTools.Console v{version} ({platformInfo})");
                Console.WriteLine($@"Current Arguments: -v {options.Verbose} -h {options.Help}");

                logger?.Info($"Application started with arguments: verbose={options.Verbose}, help={options.Help}");

                if (options.Help)
                {
                    Console.WriteLine(@"You can use the following options:");
                    Console.WriteLine(@"v, verbose   Use verbose output");
                    Console.WriteLine(@"h, help      Open additional information about the usage of this application");

                    try
                    {
                        var consoleApp = ApplicationContext.Application.Plugins as ConsoleApplication;
                        if (consoleApp?.CommandLinePlugins != null)
                        {
                            foreach (var plugin in consoleApp.CommandLinePlugins)
                            {
                                Console.WriteLine(plugin.HelpText);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.Warn(ex, "Failed to load plugin help information");
                        Console.WriteLine("Warning: Some plugin help information could not be loaded.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error in Run method");
                Console.WriteLine($"Error during execution: {ex.Message}");
            }
        }

        /// <summary>
        /// Couldn't identify the options.
        /// </summary>
        /// <param name="errors">A enumerable which holds the errors.</param>
        /// <param name="logger">The logger instance.</param>
        internal static void Error(IEnumerable<Error> errors, Logger logger = null)
        {
            logger ??= LogManager.GetCurrentClassLogger();

            Console.WriteLine("Command line parsing errors occurred:");
            
            foreach (var error in errors)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Command line error: {0} (stops processing: {1})",
                    error.Tag.ToString(),
                    error.StopsProcessing);
                
                logger.Error(errorMessage);
                Console.WriteLine($"  - {error.Tag}");
            }
            
            Console.WriteLine("Use --help for usage information.");
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
        private static void LoadApplicationConfiguration(IConfigurationManager configurationManager, Logger logger)
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
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to load application configuration, using defaults.");
            }
        }

        /// <summary>
        /// Logs detailed platform information for troubleshooting.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        private static void LogPlatformInformation(Logger logger)
        {
            logger.Info($"Starting SoluiNet.DevTools.Console on {PlatformHelper.PlatformName} - {PlatformHelper.GetDetailedPlatformInfo()}");
            logger.Info($"Runtime: {PlatformHelper.FrameworkDescription} | OS: {PlatformHelper.OSDescription}");
            logger.Debug($"Architecture Details: {PlatformHelper.Architecture} ({PlatformHelper.RuntimeIdentifier}) | ARM64: {PlatformHelper.IsArm64} | x64: {PlatformHelper.IsX64}");
            logger.Debug($"Platform Features: Windows={PlatformHelper.SupportsWindowsFeatures()}, Unix={PlatformHelper.SupportsUnixFeatures()}");
        }

        /// <summary>
        /// Handles platform not supported exceptions with appropriate messaging.
        /// </summary>
        /// <param name="exception">The platform exception.</param>
        /// <param name="logger">The logger instance.</param>
        private static void HandlePlatformException(PlatformNotSupportedException exception, Logger logger)
        {
            var message = $"Platform not supported: {PlatformHelper.GetDetailedPlatformInfo()}. Error: {exception.Message}";
            
            logger?.Error(exception, message);
            Console.WriteLine($"ERROR: {message}");
            Console.WriteLine("This application requires Windows, Linux, or macOS.");
            
            Environment.Exit(1);
        }

        /// <summary>
        /// Handles unauthorized access exceptions with platform-specific guidance.
        /// </summary>
        /// <param name="exception">The access exception.</param>
        /// <param name="logger">The logger instance.</param>
        private static void HandleAccessException(UnauthorizedAccessException exception, Logger logger)
        {
            var message = $"Access denied on {PlatformHelper.PlatformName}: {exception.Message}";
            
            logger?.Error(exception, message);
            Console.WriteLine($"ERROR: {message}");
            
            if (PlatformHelper.IsUnixLike)
            {
                Console.WriteLine("On Unix-like systems, you may need to:");
                Console.WriteLine("  - Check file permissions with 'ls -la'");
                Console.WriteLine("  - Run with appropriate user permissions");
                Console.WriteLine("  - Ensure the application directory is writable");
            }
            else if (PlatformHelper.IsWindows)
            {
                Console.WriteLine("On Windows, you may need to:");
                Console.WriteLine("  - Run as Administrator");
                Console.WriteLine("  - Check folder permissions");
                Console.WriteLine("  - Ensure antivirus is not blocking the application");
            }
            
            Environment.Exit(2);
        }

        /// <summary>
        /// Handles directory not found exceptions with platform-specific paths.
        /// </summary>
        /// <param name="exception">The directory exception.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="platformService">The platform service.</param>
        private static void HandleDirectoryException(System.IO.DirectoryNotFoundException exception, Logger logger, IPlatformService platformService)
        {
            var message = $"Directory not found on {PlatformHelper.PlatformName}: {exception.Message}";
            
            logger?.Error(exception, message);
            Console.WriteLine($"ERROR: {message}");
            
            if (platformService != null)
            {
                Console.WriteLine($"Expected configuration directory: {platformService.GetConfigurationDirectory()}");
                Console.WriteLine($"Expected application data directory: {platformService.GetApplicationDataDirectory()}");
            }
            
            Console.WriteLine("The application will attempt to create necessary directories on next run.");
            Environment.Exit(3);
        }

        /// <summary>
        /// Handles general exceptions with platform context.
        /// </summary>
        /// <param name="exception">The general exception.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="platformService">The platform service.</param>
        private static void HandleGeneralException(Exception exception, Logger logger, IPlatformService platformService)
        {
            var platformContext = $"Platform: {PlatformHelper.GetDetailedPlatformInfo()}";
            var message = $"Unexpected error on {PlatformHelper.PlatformName}: {exception.Message}";
            
            logger?.Error(exception, $"{message} | {platformContext}");
            
            Console.WriteLine($"ERROR: {message}");
            Console.WriteLine($"Platform Information: {platformContext}");
            Console.WriteLine($"Exception Type: {exception.GetType().Name}");
            
            if (logger != null)
            {
                Console.WriteLine("Check the application logs for detailed error information.");
                if (platformService != null)
                {
                    Console.WriteLine($"Log location may be in: {platformService.GetApplicationDataDirectory()}");
                }
            }
            
            Environment.Exit(4);
        }
    }
}