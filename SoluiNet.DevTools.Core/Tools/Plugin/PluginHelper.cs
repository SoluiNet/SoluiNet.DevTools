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
    using SoluiNet.DevTools.Core.ScriptEngine;
    using SoluiNet.DevTools.Core.Tools.File;
    using SoluiNet.DevTools.Core.Tools.Stream;
    using SoluiNet.DevTools.Core.Tools.XML;

    /// <summary>
    /// Provides a collection of methods that will help to work with SoluiNet.DevTools-plugins.
    /// </summary>
    public static class PluginHelper
    {
        /// <summary>
        /// Get all defined entity types for a SQL development plugin.
        /// </summary>
        /// <param name="plugin">The SQL development plugin in which the entities are defined.</param>
        /// <returns>A <see cref="IList{System.Type}"/> which contains all defined entities in the plugin.</returns>
        public static IList<System.Type> GetEntityTypes(ISqlDevPlugin plugin)
        {
            if (plugin == null)
            {
                return null;
            }

            var pluginNamespace = plugin.GetType().Namespace;

            var dataEntities = plugin.GetType().Assembly.GetTypes().Where(x =>
                    x.IsClass
                    && !string.IsNullOrEmpty(x.Namespace)
                    && (x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Entities")) || x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Data.Entities"))))
                .ToList();

            if (!dataEntities.Any())
            {
                foreach (var assembly in plugin.GetType().Assembly.GetReferencedAssemblies())
                {
                    var assemblyType = Assembly.Load(assembly);

                    dataEntities.AddRange(assemblyType.GetTypes().Where(x =>
                            x.IsClass
                            && !string.IsNullOrEmpty(x.Namespace)
                            && (x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Entities")) || x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Data.Entities"))))
                        .ToList());
                }
            }

            return dataEntities;
        }

        /// <summary>
        /// Get the fields for the overgiven entity type of a SQL development plugin.
        /// </summary>
        /// <param name="plugin">The SQL development plugin in which the entity is defined.</param>
        /// <param name="entityName">The name of the entity for which the fields should be delivered.</param>
        /// <returns>A <see cref="IList{string}"/> which contains a list of all the field names for the overgiven entity.</returns>
        public static IList<string> GetEntityFields(ISqlDevPlugin plugin, string entityName)
        {
            var fieldList = new List<string>();

            if (plugin == null)
            {
                return null;
            }

            var pluginNamespace = plugin.GetType().Namespace;

            var dataEntities = plugin.GetType().Assembly.GetTypes().Where(x =>
                    x.IsClass
                    && x.Name == entityName
                    && !string.IsNullOrEmpty(x.Namespace)
                    && (x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Entities"))
                        || x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Data.Entities"))))
                .ToList();

            if (!dataEntities.Any())
            {
                foreach (var assembly in plugin.GetType().Assembly.GetReferencedAssemblies())
                {
                    var assemblyType = Assembly.Load(assembly);

                    dataEntities.AddRange(assemblyType.GetTypes().Where(x =>
                            x.IsClass
                            && x.Name == entityName
                            && !string.IsNullOrEmpty(x.Namespace)
                            && (x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Entities"))
                                || x.Namespace.StartsWith(string.Format("{0}.{1}", pluginNamespace, "Data.Entities"))))
                        .ToList());
                }
            }

            var dataEntity = dataEntities.FirstOrDefault();

            if (dataEntity == null)
            {
                return fieldList;
            }

            fieldList.AddRange(dataEntity.GetProperties().Select(x => x.Name));

            return fieldList;
        }

        /// <summary>
        /// Get the embedded resource names of a plugin.
        /// </summary>
        /// <param name="plugin">The plugin in which the resources are contained.</param>
        /// <param name="type">The type of the resources. If not provided "Script" will be used.</param>
        /// <returns>A <see cref="IList{string}"/> which contains a list of all the embedded resource names in the overgiven plugin.</returns>
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
        /// Get the embedded SQL scripts of a plugin.
        /// </summary>
        /// <param name="plugin">The plugin in which the SQL scripts are contained.</param>
        /// <returns>A <see cref="IList{SqlScript}"/> which contains all SQL scripts that are contained in the overgiven plugin.</returns>
        public static IList<SqlScript> GetSqlScripts(ISqlDevPlugin plugin)
        {
            var scriptList = new List<SqlScript>();

            if (plugin == null)
            {
                return null;
            }

            var embeddedScripts = GetEmbeddedResources(plugin, "Script");

            foreach (var scriptContainer in embeddedScripts)
            {
                var scriptsStream = plugin.GetType().Assembly.GetManifestResourceStream(scriptContainer);

                if (scriptsStream == null)
                {
                    continue;
                }

                var scripts = XmlHelper.Deserialize<SqlScripts>(scriptsStream);

                scriptList.AddRange(scripts.SqlScript);
            }

            return scriptList;
        }

        /// <summary>
        /// Get the embedded web client definition of a plugin.
        /// </summary>
        /// <param name="plugin">The plugin in which the web client definition is contained.</param>
        /// <returns>The <see cref="WebClientDefinition.SoluiNetWebClientDefinition"/> for the overgiven plugin.</returns>
        public static WebClientDefinition.SoluiNetWebClientDefinition GetWebClientDefinition(IWebClientSupportPlugin plugin)
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
        public static Settings.SoluiNetSettingType GetSettings(IPluginWithSettings plugin)
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
        /// <returns>A <see cref="Dictionary{string, object}"/> which contains all effective settings for the plugin.</returns>
        public static IDictionary<string, object> GetSettingsAsDictionary(IPluginWithSettings plugin)
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
        /// Get the currently active environment for the overgiven plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns>Returns the currently active environment for the overgiven plugin.</returns>
        public static string GetEnvironment(ISqlDevPlugin plugin)
        {
            return plugin.Environment;
        }

        /// <summary>
        /// Get the connection string for the overgiven default connection string name under the account of the active environment for the plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which the connection string should be provided.</param>
        /// <param name="defaultConnectionStringName">The default connection string name.</param>
        /// <returns>Returns the connection string which should be used for overgiven name and the environment which has been set up for the plugin.</returns>
        public static string GetConnectionString(ISqlDevPlugin plugin, string defaultConnectionStringName)
        {
            var environment = GetEnvironment(plugin);

            return defaultConnectionStringName + (plugin.Environment == "Default" || string.IsNullOrEmpty(plugin.Environment) ? string.Empty : "." + plugin.Environment);
        }

        /// <summary>
        /// Get a list of all available environments for the overgiven plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns>Returns a <see cref="List{string}"/> of all available environments for the overgiven plugin.</returns>
        public static List<string> GetEnvironments(ISqlDevPlugin plugin)
        {
            var environmentList = new List<string>();

            environmentList.AddRange(ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
                .Where(x => x.Name.StartsWith(plugin.DefaultConnectionStringName))
                .Select(x => x.Name == plugin.DefaultConnectionStringName ?
                    "Default" :
                    x.Name.Replace(plugin.DefaultConnectionStringName + ".", string.Empty)));

            return environmentList;
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
