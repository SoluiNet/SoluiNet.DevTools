// <copyright file="Configuration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Tools.File;
    using SoluiNet.DevTools.Core.Tools.XML;

    /// <summary>
    /// The plugin configuration.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Initializes static members of the <see cref="Configuration"/> class.
        /// </summary>
        static Configuration()
        {
            ReadOrCreateConfiguration();
        }

        /// <summary>
        /// Gets or sets the current configuration.
        /// </summary>
        public static SoluiNetPluginConfigurationType Current { get; set; }

        /// <summary>
        /// Gets the effective configuration.
        /// </summary>
        public static Dictionary<string, bool> Effective
        {
            get
            {
                var effectiveConfiguration = new Dictionary<string, bool>();

                if (Current != null && Current.SoluiNetConfigurationEntry != null && Current.SoluiNetConfigurationEntry.Count() > 0)
                {
                    foreach (var entry in Current.SoluiNetConfigurationEntry)
                    {
                        if (entry.Item is SoluiNetInstallationType)
                        {
                            if (Assembly.GetExecutingAssembly().Location == (entry.Item as SoluiNetInstallationType).path)
                            {
                                if ((entry.Item as SoluiNetInstallationType).SoluiNetPlugin != null && (entry.Item as SoluiNetInstallationType).SoluiNetPlugin.Count() > 0)
                                {
                                    foreach (var pluginEntry in (entry.Item as SoluiNetInstallationType).SoluiNetPlugin)
                                    {
                                        effectiveConfiguration.Add(pluginEntry.name, pluginEntry.enabledSpecified && pluginEntry.enabled);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.Item is SoluiNetPluginEntryType)
                            {
                                effectiveConfiguration.Add((entry.Item as SoluiNetPluginEntryType).name, (entry.Item as SoluiNetPluginEntryType).enabledSpecified && (entry.Item as SoluiNetPluginEntryType).enabled);
                            }
                        }
                    }
                }

                return effectiveConfiguration;
            }
        }

        /// <summary>
        /// Gets the local configuration path.
        /// </summary>
        private static string LocalConfigurationPath
        {
            get
            {
                return string.Format("{0}\\SoluiNet.DevTools\\pluginConfiguration.xml", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            }
        }

        /// <summary>
        /// Save the configuration.
        /// </summary>
        public static void Save()
        {
            var streamWriter = File.CreateText(LocalConfigurationPath);

            streamWriter.Write(XmlHelper.Serialize<SoluiNetPluginConfigurationType>(Current));

            streamWriter.Flush();
            streamWriter.Close();
        }

        private static void ReadOrCreateConfiguration()
        {
            if (File.Exists(LocalConfigurationPath))
            {
                Current = XmlHelper.DeserializeString<SoluiNetPluginConfigurationType>(FileHelper.StringFromFile(LocalConfigurationPath));
            }
            else
            {
                var pluginConfiguration = new SoluiNetPluginConfigurationType();

                pluginConfiguration.SoluiNetConfigurationEntry = new SoluiNetConfigurationEntryType[1];
                pluginConfiguration.SoluiNetConfigurationEntry[0] = new SoluiNetConfigurationEntryType();
                pluginConfiguration.SoluiNetConfigurationEntry[0].Item = new SoluiNetInstallationType()
                {
                    path = Assembly.GetExecutingAssembly().Location,
                };

                Current = pluginConfiguration;

                Save();
            }
        }
    }
}
