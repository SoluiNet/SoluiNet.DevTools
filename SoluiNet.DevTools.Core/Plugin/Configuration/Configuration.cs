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
    using NLog;
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
                return GetConfigurationForExecutingPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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

        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
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

        /// <summary>
        /// Get configuration by scope.
        /// </summary>
        /// <param name="configurationScope">The configuration scope.</param>
        /// <returns>Returns the configuration for the overgiven scope.</returns>
        public static Dictionary<string, bool> GetConfigurationByScope(string configurationScope = "General")
        {
            if (configurationScope != "General")
            {
                return GetConfigurationForExecutingPath(configurationScope);
            }
            else
            {
                var configuration = new Dictionary<string, bool>();

                if (Current != null && Current.SoluiNetConfigurationEntry != null && Current.SoluiNetConfigurationEntry.Count() > 0)
                {
                    foreach (var entry in Current.SoluiNetConfigurationEntry)
                    {
                        if (entry.Item is SoluiNetPluginEntryType)
                        {
                            var pluginEntry = entry.Item as SoluiNetPluginEntryType;

                            if (!configuration.ContainsKey(pluginEntry.name))
                            {
                                configuration.Add(pluginEntry.name, pluginEntry.enabledSpecified && pluginEntry.enabled);
                            }
                        }
                    }
                }

                return configuration;
            }
        }

        private static Dictionary<string, bool> GetConfigurationForExecutingPath(string executingPath)
        {
            var effectiveConfiguration = new Dictionary<string, List<SoluiNetPluginEntryType>>();

            if (Current != null && Current.SoluiNetConfigurationEntry != null && Current.SoluiNetConfigurationEntry.Count() > 0)
            {
                foreach (var entry in Current.SoluiNetConfigurationEntry)
                {
                    if (entry.Item is SoluiNetInstallationType)
                    {
                        if (executingPath == (entry.Item as SoluiNetInstallationType).path)
                        {
                            if ((entry.Item as SoluiNetInstallationType).SoluiNetPlugin != null && (entry.Item as SoluiNetInstallationType).SoluiNetPlugin.Count() > 0)
                            {
                                foreach (var pluginEntry in (entry.Item as SoluiNetInstallationType).SoluiNetPlugin)
                                {
                                    pluginEntry.Scope = Enums.ConfigurationScopeEnum.PerInstallation;

                                    if (!effectiveConfiguration.ContainsKey(pluginEntry.name))
                                    {
                                        effectiveConfiguration.Add(pluginEntry.name, new List<SoluiNetPluginEntryType>() { pluginEntry });
                                    }
                                    else
                                    {
                                        effectiveConfiguration[pluginEntry.name].Add(pluginEntry);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (entry.Item is SoluiNetPluginEntryType)
                        {
                            var pluginEntry = entry.Item as SoluiNetPluginEntryType;
                            pluginEntry.Scope = Enums.ConfigurationScopeEnum.General;

                            if (!effectiveConfiguration.ContainsKey(pluginEntry.name))
                            {
                                effectiveConfiguration.Add(pluginEntry.name, new List<SoluiNetPluginEntryType>() { pluginEntry });
                            }
                            else
                            {
                                effectiveConfiguration[pluginEntry.name].Add(pluginEntry);
                            }
                        }
                    }
                }
            }

            return effectiveConfiguration.ToDictionary(x => x.Key, x =>
            {
                var highestScopePriorityEntry = x.Value.Aggregate((currentMin, y) => (currentMin == null || y.Scope < currentMin.Scope ? y : currentMin));

                return highestScopePriorityEntry.enabledSpecified && highestScopePriorityEntry.enabled;
            });
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
                    path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                };

                var pluginList = new List<SoluiNetPluginEntryType>();

                string[] dllFileNames = null;
                if (Directory.Exists("Plugins"))
                {
                    dllFileNames = Directory.GetFiles("Plugins", "*.dll");
                }

                if (dllFileNames != null)
                {
                    ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                    foreach (string dllFile in dllFileNames)
                    {
                        try
                        {
                            var an = AssemblyName.GetAssemblyName(dllFile);
                            var assembly = Assembly.Load(an);
                            assemblies.Add(assembly);
                        }
                        catch (BadImageFormatException exception)
                        {
                            Logger.Warn(exception, "Couldn't load assembly while iterating Plugins directory");
                        }
                    }

                    Type pluginType = typeof(IBasePlugin);

                    foreach (var assembly in assemblies)
                    {
                        if (assembly == null)
                        {
                            continue;
                        }

                        var types = assembly.GetTypes();
                        foreach (var type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }

                            if (type.GetInterface(pluginType.FullName) != null)
                            {
                                pluginList.Add(new SoluiNetPluginEntryType()
                                {
                                    name = assembly.GetName().Name,
                                    enabledSpecified = true,
                                    enabled = true,
                                });
                            }
                        }
                    }
                }

                (pluginConfiguration.SoluiNetConfigurationEntry[0].Item as SoluiNetInstallationType).SoluiNetPlugin = pluginList.ToArray();

                Current = pluginConfiguration;

                Save();
            }
        }
    }
}
