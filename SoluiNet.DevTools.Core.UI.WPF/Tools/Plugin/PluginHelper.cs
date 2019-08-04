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
        /// Get all defined entity types for a SQL development plugin.
        /// </summary>
        /// <param name="plugin">The SQL development plugin in which the entities are defined.</param>
        /// <returns>A <see cref="IList{T}"/> which contains all defined entities in the plugin.</returns>
        public static IList<System.Type> GetEntityTypes(IProvidesDatabaseConnectivity plugin)
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
        /// <returns>A <see cref="IList{T}"/> which contains a list of all the field names for the overgiven entity.</returns>
        public static IList<string> GetEntityFields(IProvidesDatabaseConnectivity plugin, string entityName)
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
        /// Get the embedded SQL scripts of a plugin.
        /// </summary>
        /// <param name="plugin">The plugin in which the SQL scripts are contained.</param>
        /// <returns>A <see cref="IList{SqlScript}"/> which contains all SQL scripts that are contained in the overgiven plugin.</returns>
        public static IList<SqlScript> GetSqlScripts(IProvidesDatabaseConnectivity plugin)
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
        /// Get the currently active environment for the overgiven plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns>Returns the currently active environment for the overgiven plugin.</returns>
        public static string GetEnvironment(IProvidesDatabaseConnectivity plugin)
        {
            return plugin.Environment;
        }

        /// <summary>
        /// Get the connection string for the overgiven default connection string name under the account of the active environment for the plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which the connection string should be provided.</param>
        /// <param name="defaultConnectionStringName">The default connection string name.</param>
        /// <returns>Returns the connection string which should be used for overgiven name and the environment which has been set up for the plugin.</returns>
        public static string GetConnectionString(IProvidesDatabaseConnectivity plugin, string defaultConnectionStringName)
        {
            var environment = GetEnvironment(plugin);

            return defaultConnectionStringName + (plugin.Environment == "Default" || string.IsNullOrEmpty(plugin.Environment) ? string.Empty : "." + plugin.Environment);
        }

        /// <summary>
        /// Get a list of all available environments for the overgiven plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns>Returns a <see cref="List{T}"/> of all available environments for the overgiven plugin.</returns>
        public static List<string> GetEnvironments(IProvidesDatabaseConnectivity plugin)
        {
            var environmentList = new List<string>();

            environmentList.AddRange(ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
                .Where(x => x.Name.StartsWith(plugin.DefaultConnectionStringName))
                .Select(x => x.Name == plugin.DefaultConnectionStringName ?
                    "Default" :
                    x.Name.Replace(plugin.DefaultConnectionStringName + ".", string.Empty)));

            return environmentList;
        }
    }
}
