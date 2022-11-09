// <copyright file="PluginConfigurationAdapter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Common;

    /// <summary>
    /// The adapter for plugin configurations.
    /// </summary>
    public class PluginConfigurationAdapter : IConfiguration
    {
        /// <inheritdoc />
        /// <remarks>This adapter can only return if a plugin has been activated or not for the current environment.</remarks>
        public object GetByKey(string key, string plugin = "", string environment = "")
        {
            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = "General";
            }

            var scopeConfiguration = Plugin.Configuration.Configuration.GetConfigurationByScope(environment);

            return scopeConfiguration != null ? scopeConfiguration[key] : Plugin.Configuration.Configuration.Effective[key];
        }
    }
}
