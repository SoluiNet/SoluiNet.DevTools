// <copyright file="IConfigurationManager.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    /// <summary>
    /// Interface for managing application configuration across different platforms.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets a configuration object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of configuration to retrieve.</typeparam>
        /// <param name="configurationName">The name of the configuration.</param>
        /// <returns>The configuration object, or null if not found.</returns>
        T GetConfiguration<T>(string configurationName) where T : class;

        /// <summary>
        /// Saves a configuration object.
        /// </summary>
        /// <typeparam name="T">The type of configuration to save.</typeparam>
        /// <param name="configurationName">The name of the configuration.</param>
        /// <param name="configuration">The configuration object to save.</param>
        void SaveConfiguration<T>(string configurationName, T configuration) where T : class;

        /// <summary>
        /// Gets the full path to a configuration file.
        /// </summary>
        /// <param name="configurationName">The name of the configuration.</param>
        /// <returns>The full path to the configuration file.</returns>
        string GetConfigurationPath(string configurationName);

        /// <summary>
        /// Checks if a configuration exists.
        /// </summary>
        /// <param name="configurationName">The name of the configuration.</param>
        /// <returns>True if the configuration exists, false otherwise.</returns>
        bool ConfigurationExists(string configurationName);

        /// <summary>
        /// Migrates configuration from an old location to the current platform-appropriate location.
        /// </summary>
        /// <param name="oldConfigurationPath">The path to the old configuration file.</param>
        /// <param name="configurationName">The name of the configuration to migrate to.</param>
        /// <returns>True if migration was successful, false otherwise.</returns>
        bool MigrateConfiguration(string oldConfigurationPath, string configurationName);

        /// <summary>
        /// Gets the configuration directory path for the current platform.
        /// </summary>
        /// <returns>The configuration directory path.</returns>
        string GetConfigurationDirectory();
    }
}