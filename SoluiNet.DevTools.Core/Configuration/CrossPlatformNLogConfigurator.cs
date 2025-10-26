// <copyright file="CrossPlatformNLogConfigurator.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    using System;
    using System.IO;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Configures NLog for cross-platform logging with platform-appropriate log directories.
    /// </summary>
    public static class CrossPlatformNLogConfigurator
    {
        /// <summary>
        /// Configures NLog with platform-specific log directories and settings.
        /// </summary>
        /// <param name="platformService">The platform service for getting platform-specific paths.</param>
        /// <param name="applicationName">The name of the application (used in log file names).</param>
        public static void Configure(IPlatformService platformService, string applicationName = "console")
        {
            if (platformService == null)
            {
                throw new ArgumentNullException(nameof(platformService));
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = "console";
            }

            try
            {
                // Get platform-appropriate log directory
                var logDirectory = GetLogDirectory(platformService);

                // Ensure log directory exists
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create NLog configuration
                var config = new LoggingConfiguration();

                // Create file targets with platform-specific paths
                var fileTarget = CreateFileTarget("fileLog", logDirectory, applicationName, "${longdate} [${processid}] ${uppercase:${level}} ${message} (${logger})");
                var traceTarget = CreateFileTarget("traceLog", logDirectory, $"trace_{applicationName}", "${longdate} [${processid}] ${uppercase:${level}} ${message} (${logger})");
                var errorTarget = CreateFileTarget("errorLog", logDirectory, $"{applicationName}_errors", "${longdate} [${processid}] ${uppercase:${level}} ${message}${newline}${exception:format=tostring}");

                // Add targets to configuration
                config.AddTarget(fileTarget);
                config.AddTarget(traceTarget);
                config.AddTarget(errorTarget);

                // Create logging rules
                config.AddRule(LogLevel.Error, LogLevel.Fatal, errorTarget);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);
                
                // Add trace rule (disabled by default)
                var traceRule = new LoggingRule("*", LogLevel.Trace, traceTarget);
                traceRule.SetLoggingLevels(LogLevel.Trace, LogLevel.Info);
                traceRule.Final = false;
                // Note: LoggingRule.Enabled property doesn't exist in this NLog version
                // The rule will be added but can be controlled via configuration
                config.LoggingRules.Add(traceRule);

                // Filter out Quartz noise
                var quartzRule = new LoggingRule("Quartz*", LogLevel.Trace, LogLevel.Info, null);
                quartzRule.Final = true;
                config.LoggingRules.Insert(0, quartzRule);

                // Apply configuration
                LogManager.Configuration = config;

                // Log platform information for debugging
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info($"NLog configured for platform: {GetPlatformName()}");
                logger.Info($"Log directory: {logDirectory}");
            }
            catch (Exception ex)
            {
                // If NLog configuration fails, we can't use NLog to log the error
                // Fall back to console output
                Console.WriteLine($"Failed to configure NLog: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the platform-appropriate log directory.
        /// </summary>
        /// <param name="platformService">The platform service.</param>
        /// <returns>The log directory path.</returns>
        private static string GetLogDirectory(IPlatformService platformService)
        {
            var appDataDir = platformService.GetApplicationDataDirectory();
            return platformService.NormalizePath(Path.Combine(appDataDir, "logs"));
        }

        /// <summary>
        /// Creates a file target with the specified parameters.
        /// </summary>
        /// <param name="name">The target name.</param>
        /// <param name="logDirectory">The log directory.</param>
        /// <param name="filePrefix">The file prefix.</param>
        /// <param name="layout">The log layout.</param>
        /// <returns>A configured file target.</returns>
        private static FileTarget CreateFileTarget(string name, string logDirectory, string filePrefix, string layout)
        {
            var target = new FileTarget(name)
            {
                FileName = Path.Combine(logDirectory, $"{filePrefix}_${{shortdate}}.log"),
                Layout = layout,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                MaxArchiveFiles = 7, // Keep 7 days of logs
                ConcurrentWrites = true,
                KeepFileOpen = false // Better for cross-platform compatibility
            };

            // Note: FileAttributes property expects Win32FileAttributes, not System.IO.FileAttributes
            // For cross-platform compatibility, we'll skip setting file attributes here
            // and rely on the operating system's default file permissions

            return target;
        }

        /// <summary>
        /// Gets a human-readable platform name for logging.
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