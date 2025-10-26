// <copyright file="ConfigurationMigrationService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NLog;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Service for migrating configuration files from old Windows-specific locations to cross-platform locations.
    /// </summary>
    public class ConfigurationMigrationService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfigurationManager configurationManager;

        // Constants for configuration paths
        private const string SoluiNetFolderName = "SoluiNet";
        private const string DevToolsFolderName = "DevTools";
        private const string LegacyFolderName = "SoluiNet.DevTools";
        private const string SettingsFileName = "settings.json";
        private const string PluginsFileName = "plugins.json";
        private const string ApplicationFileName = "application.json";
        private const string SettingsConfigName = "settings";
        private const string PluginsConfigName = "plugins";
        private const string ApplicationConfigName = "application";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationMigrationService"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="platformService">The platform service.</param>
        public ConfigurationMigrationService(IConfigurationManager configurationManager, IPlatformService platformService)
        {
            this.configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _ = platformService ?? throw new ArgumentNullException(nameof(platformService));
        }

        /// <summary>
        /// Migrates configuration files from Windows-specific locations to the current platform's appropriate location.
        /// </summary>
        /// <returns>True if any configurations were migrated, false otherwise.</returns>
        public bool MigrateFromWindowsLocations()
        {
            var migrated = false;

            try
            {
                // Get potential Windows configuration paths
                var windowsConfigPaths = GetWindowsConfigurationPaths();

                foreach (var (oldPath, configName) in windowsConfigPaths)
                {
                    if (this.configurationManager.MigrateConfiguration(oldPath, configName))
                    {
                        migrated = true;
                    }
                }

                if (migrated)
                {
                    Logger.Info("Configuration migration from Windows locations completed successfully.");
                }
                else
                {
                    Logger.Debug("No Windows configurations found to migrate.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred during configuration migration from Windows locations.");
            }

            return migrated;
        }

        /// <summary>
        /// Migrates a specific configuration file from a custom location.
        /// </summary>
        /// <param name="oldConfigurationPath">The path to the old configuration file.</param>
        /// <param name="configurationName">The name of the configuration.</param>
        /// <returns>True if migration was successful, false otherwise.</returns>
        public bool MigrateConfiguration(string oldConfigurationPath, string configurationName)
        {
            return this.configurationManager.MigrateConfiguration(oldConfigurationPath, configurationName);
        }

        /// <summary>
        /// Gets a list of potential Windows configuration paths and their corresponding configuration names.
        /// </summary>
        /// <returns>A list of tuples containing old paths and configuration names.</returns>
        private static List<(string OldPath, string ConfigName)> GetWindowsConfigurationPaths()
        {
            var paths = new List<(string, string)>();

            try
            {
                // Common Windows configuration locations
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // Add potential configuration file paths
                var potentialPaths = new[]
                {
                    (Path.Combine(appDataPath, SoluiNetFolderName, DevToolsFolderName, SettingsFileName), SettingsConfigName),
                    (Path.Combine(appDataPath, SoluiNetFolderName, DevToolsFolderName, PluginsFileName), PluginsConfigName),
                    (Path.Combine(appDataPath, SoluiNetFolderName, DevToolsFolderName, ApplicationFileName), ApplicationConfigName),
                    (Path.Combine(localAppDataPath, SoluiNetFolderName, DevToolsFolderName, SettingsFileName), SettingsConfigName),
                    (Path.Combine(localAppDataPath, SoluiNetFolderName, DevToolsFolderName, PluginsFileName), PluginsConfigName),
                    (Path.Combine(localAppDataPath, SoluiNetFolderName, DevToolsFolderName, ApplicationFileName), ApplicationConfigName),

                    // Legacy paths (if any existed)
                    (Path.Combine(appDataPath, LegacyFolderName, SettingsFileName), SettingsConfigName),
                    (Path.Combine(appDataPath, LegacyFolderName, PluginsFileName), PluginsConfigName),
                    (Path.Combine(localAppDataPath, LegacyFolderName, SettingsFileName), SettingsConfigName),
                    (Path.Combine(localAppDataPath, LegacyFolderName, PluginsFileName), PluginsConfigName),
                };

                foreach (var (path, configName) in potentialPaths)
                {
                    if (File.Exists(path))
                    {
                        paths.Add((path, configName));
                        Logger.Debug($"Found existing Windows configuration: {path}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while searching for Windows configuration paths.");
            }

            return paths;
        }
    }
}