// <copyright file="PluginHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.ScriptEngine;
    using SoluiNet.DevTools.Core.Tools.File;
    using SoluiNet.DevTools.Core.Tools.Stream;
    using SoluiNet.DevTools.Core.Tools.XML;

    /// <summary>
    /// Provides a collection of methods that will help to work with SoluiNet.DevTools-plugins.
    /// </summary>
    public static partial class PluginHelper
    {
        /// <summary>
        /// Get the embedded resource names of a plugin.
        /// </summary>
        /// <param name="plugin">The plugin in which the resources are contained.</param>
        /// <param name="type">The type of the resources. If not provided "Script" will be used.</param>
        /// <returns>A <see cref="IList{T}"/> which contains a list of all the embedded resource names in the overgiven plugin.</returns>
        public static IList<string> GetEmbeddedResources(IBasePlugin plugin, string type = "Script")
        {
            if (plugin == null)
            {
                return null;
            }

            var pluginNamespace = plugin.GetType().Namespace;

            var embeddedResources = plugin.GetType().Assembly.GetManifestResourceNames().Where(x =>
                    x.StartsWith(string.Format("{0}.{1}", pluginNamespace, type)))
                .ToList();

            return embeddedResources;
        }

        /// <summary>
        /// Get the embedded web client definition of a plugin.
        /// </summary>
        /// <param name="plugin">The plugin in which the web client definition is contained.</param>
        /// <returns>The <see cref="WebClientDefinition.SoluiNetWebClientDefinition"/> for the overgiven plugin.</returns>
        public static WebClientDefinition.SoluiNetWebClientDefinition GetWebClientDefinition(ISupportsWebClient plugin)
        {
            if (plugin == null)
            {
                return null;
            }

            var embeddedWebClientDefs = GetEmbeddedResources(plugin, "WebClientDefinition");

            var webClientDefContainer = embeddedWebClientDefs.FirstOrDefault();

            if (webClientDefContainer == null)
            {
                return null;
            }

            var webClientDefStream = plugin.GetType().Assembly.GetManifestResourceStream(webClientDefContainer);

            if (webClientDefStream == null)
            {
                return null;
            }

            var webClientDef = XmlHelper.Deserialize<WebClientDefinition.SoluiNetWebClientDefinition>(webClientDefStream);

            return webClientDef;
        }

        /// <summary>
        /// Get the effective settings for the overgiven plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which the settings should be provided.</param>
        /// <returns>An instance of <see cref="Settings.SoluiNetSettingType"/> which contains all effective settings for the plugin.</returns>
        public static Settings.SoluiNetSettingType GetSettings(IContainsSettings plugin)
        {
            if (plugin == null)
            {
                return null;
            }

            var settingsXml = string.Empty;

            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
            {
                return null;
            }

            var generalSettings = Path.Combine(folderPath, "Settings.xml");

            if (System.IO.File.Exists(generalSettings))
            {
                var generalSettingsXml = FileHelper.StringFromFile(generalSettings);

                settingsXml = generalSettingsXml;
            }

            var embeddedSettings = GetEmbeddedResources(plugin, "Settings");

            var settingsContainer = embeddedSettings.FirstOrDefault();

            if (settingsContainer != null)
            {
                var settingsStream = plugin.GetType().Assembly.GetManifestResourceStream(settingsContainer);

                if (settingsStream == null)
                {
                    return null;
                }

                var embeddedResourceXml = StreamHelper.StreamToString(settingsStream);
                settingsXml = XmlHelper.Merge(settingsXml, embeddedResourceXml, "SoluiNet.Settings");
            }

            var settingsPathForPlugin = Path.Combine(folderPath, "Plugins", string.Format("Settings.{0}.xml", plugin.Name));

            if (System.IO.File.Exists(settingsPathForPlugin))
            {
                var localSavedXml = FileHelper.StringFromFile(settingsPathForPlugin);

                settingsXml = XmlHelper.Merge(settingsXml, localSavedXml, "SoluiNet.Settings");
            }

            var settings = XmlHelper.Deserialize<Settings.SoluiNetSettingType>(settingsXml);

            return settings;
        }

        /// <summary>
        /// Get the effective settings for the overgiven plugin as dictionary.
        /// </summary>
        /// <param name="plugin">The plugin for which the settings should be provided.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> which contains all effective settings for the plugin.</returns>
        public static IDictionary<string, object> GetSettingsAsDictionary(IContainsSettings plugin)
        {
            var settings = GetSettings(plugin);

            if (settings == null)
            {
                return null;
            }

            var preparedSettings = settings.SoluiNetEnvironment
                .SelectMany(x => x.SoluiNetSettingEntry.Select(y => new { SettingName = string.Format("{0}.{1}", x.name, y.name), SettingValue = y.Value }));

            return preparedSettings.ToDictionary(x => x.SettingName, x => (object)x.SettingValue);
        }

        /// <summary>
        /// Provide a default implementation for the assembly resolve event.
        /// </summary>
        /// <param name="sender">The sender which triggered the event.</param>
        /// <param name="args">A <see cref="ResolveEventArgs"/> with additional arguments about the resolve event.</param>
        /// <returns>Returns an instance of <see cref="Assembly"/> which contains the assembly for which the event was looking for.</returns>
        public static Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
            {
                return null;
            }

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            var assemblyPluginPath = Path.Combine(folderPath, "Plugins", new AssemblyName(args.Name).Name + ".dll");

            if (!System.IO.File.Exists(assemblyPath) && !System.IO.File.Exists(assemblyPluginPath))
            {
                return null;
            }

            Assembly assembly = null;

            if (System.IO.File.Exists(assemblyPath))
            {
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            else if (System.IO.File.Exists(assemblyPluginPath))
            {
                assembly = Assembly.LoadFrom(assemblyPluginPath);
            }

            return assembly;
        }

        /// <summary>
        /// Get all plugins of the overgiven type.
        /// </summary>
        /// <typeparam name="T">The plugin type.</typeparam>
        /// <returns>Returns a <see cref="List{T}"/> of all available plugins which are castable to the overgiven plugin type.</returns>
        public static List<T> GetPlugins<T>()
        {
            string[] dllFileNames = null;
            if (Directory.Exists("Plugins"))
            {
                dllFileNames = Directory.GetFiles("Plugins", "*.dll");
            }

            if (dllFileNames == null)
            {
                return null;
            }

            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                var an = AssemblyName.GetAssemblyName(dllFile);
                var assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }

            Type pluginType = typeof(T);

            var pluginList = new List<T>();

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
                        var plugin = (T)Activator.CreateInstance(type);
                        pluginList.Add(plugin);
                    }

                    if (pluginType.IsInterface && pluginType.IsGenericType)
                    {
                        var typeInterfaces = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition().FullName == pluginType.GetGenericTypeDefinition().FullName);

                        if (typeInterfaces == null || typeInterfaces.Count() == 0)
                        {
                            continue;
                        }

                        foreach (var typeInterface in typeInterfaces)
                        {
                            var pluginGenericArguments = pluginType.GetGenericArguments();
                            var typeGenericArguments = typeInterface.GetGenericArguments();

                            if (pluginGenericArguments.Count() != typeGenericArguments.Count())
                            {
                                continue;
                            }

                            for (int i = 0; i < pluginGenericArguments.Count(); i++)
                            {
                                var genericArgument = pluginGenericArguments[i];

                                if (genericArgument.GetType() != typeGenericArguments[i].GetType())
                                {
                                    continue;
                                }
                            }

                            var plugin = (T)Activator.CreateInstance(type);
                            pluginList.Add(plugin);
                        }
                    }
                }
            }

            return pluginList;
        }

        /// <summary>
        /// Get a plugin with the overgiven name which implements the overgiven type.
        /// </summary>
        /// <typeparam name="T">The plugin type.</typeparam>
        /// <param name="name">The plugin name.</param>
        /// <returns>Returns the plugin which has the overgiven name and is castable to the overgiven plugin type.</returns>
        public static T GetPluginByName<T>(string name)
            where T : IBasePlugin
        {
            var pluginList = GetPlugins<T>();

            foreach (var plugin in pluginList)
            {
                if (plugin.Name.Equals(name))
                {
                    return plugin;
                }
            }

            return default(T);
        }
    }
}
