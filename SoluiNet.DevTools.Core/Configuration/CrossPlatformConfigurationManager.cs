// <copyright file="CrossPlatformConfigurationManager.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using NLog;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Cross-platform configuration manager that handles configuration loading, saving, and migration.
    /// </summary>
    public class CrossPlatformConfigurationManager : IConfigurationManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPlatformService platformService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossPlatformConfigurationManager"/> class.
        /// </summary>
        /// <param name="platformService">The platform service for platform-specific operations.</param>
        public CrossPlatformConfigurationManager(IPlatformService platformService)
        {
            this.platformService = platformService ?? throw new ArgumentNullException(nameof(platformService));
        }

        /// <inheritdoc/>
        public T GetConfiguration<T>(string configurationName) where T : class
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                throw new ArgumentException("Configuration name cannot be null or empty.", nameof(configurationName));
            }

            var configPath = this.GetConfigurationPath(configurationName);
            
            if (!File.Exists(configPath))
            {
                Logger.Debug($"Configuration file not found: {configPath}");
                return null;
            }

            try
            {
                var json = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<T>(json);
                Logger.Debug($"Successfully loaded configuration: {configurationName}");
                return config;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to load configuration: {configurationName}");
                return null;
            }
        }

        /// <inheritdoc/>
        public void SaveConfiguration<T>(string configurationName, T configuration) where T : class
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                throw new ArgumentException("Configuration name cannot be null or empty.", nameof(configurationName));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configPath = this.GetConfigurationPath(configurationName);
            var configDir = Path.GetDirectoryName(configPath);

            try
            {
                // Ensure the configuration directory exists
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                    Logger.Debug($"Created configuration directory: {configDir}");
                }

                var json = JsonConvert.SerializeObject(configuration, Formatting.Indented);
                File.WriteAllText(configPath, json);
                Logger.Debug($"Successfully saved configuration: {configurationName}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to save configuration: {configurationName}");
                throw new InvalidOperationException($"Failed to save configuration: {configurationName}", ex);
            }
        }

        /// <inheritdoc/>
        public string GetConfigurationPath(string configurationName)
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                throw new ArgumentException("Configuration name cannot be null or empty.", nameof(configurationName));
            }

            var configDir = this.platformService.GetConfigurationDirectory();
            var fileName = configurationName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) 
                ? configurationName 
                : $"{configurationName}.json";
            
            return this.platformService.NormalizePath(Path.Combine(configDir, fileName));
        }

        /// <inheritdoc/>
        public bool ConfigurationExists(string configurationName)
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                return false;
            }

            var configPath = this.GetConfigurationPath(configurationName);
            return File.Exists(configPath);
        }

        /// <inheritdoc/>
        public bool MigrateConfiguration(string oldConfigurationPath, string configurationName)
        {
            if (string.IsNullOrEmpty(oldConfigurationPath) || string.IsNullOrEmpty(configurationName))
            {
                return false;
            }

            if (!File.Exists(oldConfigurationPath))
            {
                Logger.Debug($"Old configuration file not found for migration: {oldConfigurationPath}");
                return false;
            }

            var newConfigPath = this.GetConfigurationPath(configurationName);
            
            // Don't overwrite existing configuration
            if (File.Exists(newConfigPath))
            {
                Logger.Debug($"New configuration already exists, skipping migration: {newConfigPath}");
                return false;
            }

            try
            {
                var configDir = Path.GetDirectoryName(newConfigPath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                File.Copy(oldConfigurationPath, newConfigPath);
                Logger.Info($"Successfully migrated configuration from {oldConfigurationPath} to {newConfigPath}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to migrate configuration from {oldConfigurationPath} to {newConfigPath}");
                return false;
            }
        }

        /// <inheritdoc/>
        public string GetConfigurationDirectory()
        {
            return this.platformService.GetConfigurationDirectory();
        }
    }
}