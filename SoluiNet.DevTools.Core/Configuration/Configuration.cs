using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SoluiNet.DevTools.Core.Configuration
{
    public class Configuration
    {
        private static SoluiNetConfigurationType ReadXmlConfiguration(string configPath = "")
        {
            var configFilePath = configPath;

            if (string.IsNullOrEmpty(configPath))
                configFilePath = "Configuration.xml";

            if (!File.Exists(configFilePath))
                return null;

            var xmlReader = XmlReader.Create(configFilePath);

            var xmlSerializer = new XmlSerializer(typeof(SoluiNetConfigurationType));

            return xmlSerializer.Deserialize(xmlReader) as SoluiNetConfigurationType;
        }

        public static string GetConfigurationEntry(string entryName, string pluginName = "Core", string alternativeConfigKey = "")
        {
            var config = ReadXmlConfiguration(!string.IsNullOrEmpty(alternativeConfigKey) ? string.Format("Configuration.{0}", alternativeConfigKey) : string.Empty);

            if (config == null)
                return string.Empty;

            var pluginArea = config.SoluiNetPlugin.FirstOrDefault(x => x.name == pluginName);

            if (pluginArea == null)
                return string.Empty;

            var configurationEntry = pluginArea.SoluiNetConfigurationEntry.FirstOrDefault(x => x.name == entryName);

            return configurationEntry != null ? configurationEntry.Value : string.Empty;
        }
    }
}
