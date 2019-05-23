// <copyright file="Configuration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides a class which allows working with the configuration files.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Get the value of an configuration entry.
        /// </summary>
        /// <param name="entryName">The entry name.</param>
        /// <param name="pluginName">The plugin name. If not provided "Core" will be used.</param>
        /// <param name="alternativeConfigKey">An alternative configuration in which can be searched for the configuration entry.</param>
        /// <returns>The value of the configuration entry as a <see cref="string"/>.</returns>
        public static string GetConfigurationEntry(string entryName, string pluginName = "Core", string alternativeConfigKey = "")
        {
            var config = ReadXmlConfiguration(!string.IsNullOrEmpty(alternativeConfigKey) ? string.Format("Configuration.{0}", alternativeConfigKey) : string.Empty);

            if (config == null)
            {
                return string.Empty;
            }

            var pluginArea = config.SoluiNetPlugin.FirstOrDefault(x => x.name == pluginName);

            if (pluginArea == null)
            {
                return string.Empty;
            }

            var configurationEntry = pluginArea.SoluiNetConfigurationEntry.FirstOrDefault(x => x.name == entryName);

            return configurationEntry != null ? configurationEntry.Value : string.Empty;
        }

        /// <summary>
        /// Read the XML configuration.
        /// </summary>
        /// <param name="configPath">The configuration path. If not provided an empty string will be used.</param>
        /// <returns>The deserialized <see cref="SoluiNetConfigurationType"/> which could be read from the XML configuration.</returns>
        private static SoluiNetConfigurationType ReadXmlConfiguration(string configPath = "")
        {
            var configFilePath = configPath;

            if (string.IsNullOrEmpty(configPath))
            {
                configFilePath = "Configuration.xml";
            }

            if (!File.Exists(configFilePath))
            {
                return null;
            }

            var xmlReader = XmlReader.Create(configFilePath);

            var xmlSerializer = new XmlSerializer(typeof(SoluiNetConfigurationType));

            return xmlSerializer.Deserialize(xmlReader) as SoluiNetConfigurationType;
        }
    }
}
