﻿// <copyright file="ApplicationConfigurationAdapter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Common;

    /// <summary>
    /// The adapter for application wide configurations.
    /// </summary>
    public class ApplicationConfigurationAdapter : IConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigurationAdapter"/> class.
        /// </summary>
        public ApplicationConfigurationAdapter()
        {
            this.PluginConfiguration = new PluginConfigurationAdapter();
            this.Settings = new SettingsAdapter();
        }

        /// <summary>
        /// Gets the plugin configuration.
        /// </summary>
        public PluginConfigurationAdapter PluginConfiguration { get; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public SettingsAdapter Settings { get; }

        /// <inheritdoc />
        public object GetByKey(string key, string plugin = "", string environment = "")
        {
            if (string.IsNullOrWhiteSpace(plugin))
            {
                plugin = "Core";
            }

            return Configuration.Configuration.GetConfigurationEntry(key, plugin, environment);
        }
    }
}
