// <copyright file="SettingsAdapter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SoluiNet.DevTools.Core.Common;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// The adapter for settings.
    /// </summary>
    public class SettingsAdapter : IConfiguration
    {
        /// <inheritdoc />
        public object GetByKey(string key, string plugin = "", string environment = "")
        {
            IContainsSettings pluginObject = null;

            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = "Default";
            }

            if (!string.IsNullOrWhiteSpace(plugin))
            {
                pluginObject = ApplicationContext.Application?.Plugins
                    .FirstOrDefault(x => x.Name == plugin && x is IContainsSettings) as IContainsSettings;
            }

            if (pluginObject == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            var settings = PluginHelper.GetSettings(pluginObject);

            return settings?.SoluiNetEnvironment
                    .FirstOrDefault(x => x.name == environment)?
                    .SoluiNetSettingEntry
                    .FirstOrDefault(x => x.name == key)?
                    .Value;
        }
    }
}
