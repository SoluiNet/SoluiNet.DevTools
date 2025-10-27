// <copyright file="CrossPlatformNLogConfigurator.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Configures NLog for cross-platform logging with platform-appropriate log directories and settings.
    /// </summary>
    public static class CrossPlatformNLogConfigurator
    {
        /// <summary>
        /// The buffer size for log file operations (32KB).
        /// </summary>
        private const int LogFileBufferSize = 32768;

        /// <summary>
        /// The enhanced log layout with platform information.
        /// </summary>
        private const string EnhancedLogLayout = "${longdate} [${proce  ssid}] ${uppercase:${level}} [${platform-info}] ${message} (${logger})";
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
                // Register custom layout renderer for platform information
                RegisterCustomLayoutRenderers();

                // Get platform-appropriate log directory
                var logDirectory = GetLogDirectory(platformService);

                // Ensure log directory exists with proper permissions
                CreateLogDirectoryWithPermissions(logDirectory);

                // Create NLog configuration
                var config = new LoggingConfiguration();

                // Create enhanced layout with platform information
                var enhancedLayout = CreateEnhancedLayout();

                // Create file targets with platform-specific settings
                var fileTarget = CreateFileTarget("fileLog", logDirectory, applicationName, enhancedLayout);
                var traceTarget = CreateFileTarget("traceLog", logDirectory, $"trace_{applicationName}", enhancedLayout);
                var errorTarget = CreateFileTarget("errorLog", logDirectory, $"{applicationName}_errors", 
                    "${longdate} [${processid}] ${uppercase:${level}} [${platform-info}] ${message}${newline}${exception:format=tostring}");

                // Configure console target with platform information
                var consoleTarget = CreateConsoleTarget("console");

                // Add targets to configuration
                config.AddTarget(fileTarget);
                config.AddTarget(traceTarget);
                config.AddTarget(errorTarget);
                config.AddTarget(consoleTarget);

                // Create logging rules with platform-appropriate levels
                CreateLoggingRules(config, fileTarget, traceTarget, errorTarget, consoleTarget);

                // Apply configuration
                LogManager.Configuration = config;

                // Set up log file permissions on Unix-like systems
                SetLogFilePermissions(logDirectory);

                // Log platform information for debugging
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info($"NLog configured for {GetPlatformName()} | Log directory: {logDirectory}");
                logger.Debug($"Platform details: {GetDetailedPlatformInfo()}");
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
        /// Registers custom layout renderers for enhanced logging.
        /// </summary>
        private static void RegisterCustomLayoutRenderers()
        {
            try
            {
                // Register the platform info layout renderer using the modern NLog API
                LogManager.Setup().SetupExtensions(ext => ext.RegisterLayoutRenderer<PlatformInfoLayoutRenderer>("platform-info"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not register custom layout renderers: {ex.Message}");
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
        /// Creates a file target with platform-specific settings.
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
                ConcurrentWrites = true,
                KeepFileOpen = false, // Better for cross-platform compatibility
                CreateDirs = true,
                BufferSize = LogFileBufferSize, // 32KB buffer for better performance
            };

            // Configure platform-specific archiving policies
            ConfigurePlatformSpecificArchiving(target);

            return target;
        }

        /// <summary>
        /// Creates a console target with platform information.
        /// </summary>
        /// <param name="name">The target name.</param>
        /// <returns>A configured console target.</returns>
        private static ColoredConsoleTarget CreateConsoleTarget(string name)
        {
            var target = new ColoredConsoleTarget(name)
            {
                Layout = "${longdate} ${uppercase:${level}} [${platform-info}] ${message} ${exception:format=tostring}",
                UseDefaultRowHighlightingRules = true,
            };

            return target;
        }

        /// <summary>
        /// Configures platform-specific archiving policies.
        /// </summary>
        /// <param name="target">The file target to configure.</param>
        private static void ConfigurePlatformSpecificArchiving(FileTarget target)
        {
            if (OperatingSystem.IsWindows())
            {
                // Windows: More aggressive archiving due to potential disk space constraints
                target.ArchiveEvery = FileArchivePeriod.Day;
                target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
                target.MaxArchiveFiles = 14; // Keep 2 weeks of logs
                target.ArchiveOldFileOnStartup = true;
            }
            else if (OperatingSystem.IsLinux())
            {
                // Linux: Follow typical log rotation patterns
                target.ArchiveEvery = FileArchivePeriod.Day;
                target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
                target.MaxArchiveFiles = 30; // Keep 1 month of logs
                target.ArchiveOldFileOnStartup = false; // Let logrotate handle this if configured
            }
            else if (OperatingSystem.IsMacOS())
            {
                // macOS: Moderate archiving policy
                target.ArchiveEvery = FileArchivePeriod.Day;
                target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
                target.MaxArchiveFiles = 21; // Keep 3 weeks of logs
                target.ArchiveOldFileOnStartup = true;
            }
            else
            {
                // Default/fallback policy
                target.ArchiveEvery = FileArchivePeriod.Day;
                target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
                target.MaxArchiveFiles = 7; // Keep 1 week of logs
            }
        }

        /// <summary>
        /// Creates an enhanced layout with platform information.
        /// </summary>
        /// <returns>The enhanced layout string.</returns>
        private static string CreateEnhancedLayout()
        {
            return EnhancedLogLayout;
        }

        /// <summary>
        /// Creates logging rules with platform-appropriate settings.
        /// </summary>
        /// <param name="config">The logging configuration.</param>
        /// <param name="fileTarget">The file target.</param>
        /// <param name="traceTarget">The trace target.</param>
        /// <param name="errorTarget">The error target.</param>
        /// <param name="consoleTarget">The console target.</param>
        private static void CreateLoggingRules(LoggingConfiguration config, FileTarget fileTarget, FileTarget traceTarget, FileTarget errorTarget, ColoredConsoleTarget consoleTarget)
        {
            // Error logging - always to error file and console
            config.AddRule(LogLevel.Error, LogLevel.Fatal, errorTarget);
            config.AddRule(LogLevel.Error, LogLevel.Fatal, consoleTarget);

            // General file logging - platform-specific levels
            if (OperatingSystem.IsWindows())
            {
                // Windows: More verbose logging for debugging Windows-specific issues
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);
            }
            else
            {
                // Unix-like: Standard logging level
                config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            }

            // Console logging - warnings and above
            config.AddRule(LogLevel.Warn, LogLevel.Fatal, consoleTarget);

            // Trace logging - only when explicitly enabled
            var traceRule = new LoggingRule("*", LogLevel.Trace, traceTarget);
            traceRule.SetLoggingLevels(LogLevel.Trace, LogLevel.Debug);
            traceRule.Final = false;
            config.LoggingRules.Add(traceRule);

            // Filter out noisy loggers
            var quartzRule = new LoggingRule("Quartz*", LogLevel.Trace, LogLevel.Info, null);
            quartzRule.Final = true;
            config.LoggingRules.Insert(0, quartzRule);

            // Filter out Microsoft.Extensions noise on non-Windows platforms
            if (!OperatingSystem.IsWindows())
            {
                var microsoftRule = new LoggingRule("Microsoft.Extensions.*", LogLevel.Trace, LogLevel.Info, null);
                microsoftRule.Final = true;
                config.LoggingRules.Insert(0, microsoftRule);
            }
        }

        /// <summary>
        /// Creates the log directory with proper permissions.
        /// </summary>
        /// <param name="logDirectory">The log directory path.</param>
        private static void CreateLogDirectoryWithPermissions(string logDirectory)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);

                // Set appropriate permissions on Unix-like systems
                if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                {
                    try
                    {
                        // Set directory permissions: owner read/write/execute, group read/execute, others read/execute
                        File.SetUnixFileMode(logDirectory, 
                            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                            UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                            UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not set log directory permissions: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Sets appropriate permissions for log files on Unix-like systems.
        /// </summary>
        /// <param name="logDirectory">The log directory path.</param>
        private static void SetLogFilePermissions(string logDirectory)
        {
            if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
            {
                return; // Only applicable to Unix-like systems
            }

            try
            {
                // Set permissions for existing log files
                var logFiles = Directory.GetFiles(logDirectory, "*.log", SearchOption.TopDirectoryOnly);
                foreach (var logFile in logFiles)
                {
                    try
                    {
                        // Set file permissions: owner read/write, group read, others read
                        File.SetUnixFileMode(logFile,
                            UnixFileMode.UserRead | UnixFileMode.UserWrite |
                            UnixFileMode.GroupRead |
                            UnixFileMode.OtherRead);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not set permissions for log file {logFile}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not set log file permissions: {ex.Message}");
            }
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

        /// <summary>
        /// Gets detailed platform information for logging.
        /// </summary>
        /// <returns>Detailed platform information string.</returns>
        private static string GetDetailedPlatformInfo()
        {
            var platformName = GetPlatformName();
            var architecture = RuntimeInformation.ProcessArchitecture.ToString();
            var framework = RuntimeInformation.FrameworkDescription;
            var osDescription = RuntimeInformation.OSDescription;

            return $"{platformName} {architecture} | {framework} | {osDescription}";
        }
    }
}