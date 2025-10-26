// <copyright file="ApplicationConfiguration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the main application configuration.
    /// </summary>
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Gets or sets the enabled plugins dictionary.
        /// </summary>
        public Dictionary<string, bool> EnabledPlugins { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets the logging configuration.
        /// </summary>
        public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();

        /// <summary>
        /// Gets or sets a value indicating whether to enable automatic plugin discovery.
        /// </summary>
        public bool EnableAutoPluginDiscovery { get; set; } = true;

        /// <summary>
        /// Gets or sets the plugin directories to search.
        /// </summary>
        public List<string> PluginDirectories { get; set; } = new List<string> { "plugins" };

        /// <summary>
        /// Gets or sets a value indicating whether to enable configuration migration.
        /// </summary>
        public bool EnableConfigurationMigration { get; set; } = true;

        /// <summary>
        /// Gets or sets the last migration version.
        /// </summary>
        public string LastMigrationVersion { get; set; }
    }
}