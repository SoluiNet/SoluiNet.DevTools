using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SoluiNet.DevTools.Core.ScriptEngine;

namespace SoluiNet.DevTools.Core.Tools
{
    public static class PluginHelper
    {
        public static IList<System.Type> GetEntityTypes(ISqlDevPlugin plugin)
        {
            if (plugin == null)
                return null;

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

        public static IList<string> GetEntityFields(ISqlDevPlugin plugin, string entityName)
        {
            var fieldList = new List<string>();

            if (plugin == null)
                return null;

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
                return fieldList;

            fieldList.AddRange(dataEntity.GetProperties().Select(x => x.Name));

            return fieldList;
        }

        public static IList<string> GetEmbeddedResources(ISqlDevPlugin plugin, string type = "Script")
        {
            if (plugin == null)
                return null;

            var pluginNamespace = plugin.GetType().Namespace;

            var embeddedResources = plugin.GetType().Assembly.GetManifestResourceNames().Where(x =>
                    x.StartsWith(string.Format("{0}.{1}", pluginNamespace, type)))
                .ToList();

            return embeddedResources;
        }

        public static IList<SqlScript> GetSqlScripts(ISqlDevPlugin plugin)
        {
            var scriptList = new List<SqlScript>();

            if (plugin == null)
                return null;

            var embeddedScripts = GetEmbeddedResources(plugin, "Script");
            var xmlSerializer = new XmlSerializer(typeof(SqlScripts));

            foreach (var scriptContainer in embeddedScripts)
            {
                var scriptsStream = plugin.GetType().Assembly.GetManifestResourceStream(scriptContainer);

                if (scriptsStream == null)
                    continue;

                var scripts = (SqlScripts)xmlSerializer.Deserialize(scriptsStream);

                scriptList.AddRange(scripts.SqlScript);
            }

            return scriptList;
        }

        public static string GetEnvironment(ISqlDevPlugin plugin)
        {
            return plugin.Environment;
        }

        public static string GetConnectionString(ISqlDevPlugin plugin, string defaultConnectionStringName)
        {
            var environment = GetEnvironment(plugin);

            return defaultConnectionStringName + (plugin.Environment == "Default" || string.IsNullOrEmpty(plugin.Environment) ? string.Empty : "." + plugin.Environment);
        }

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
    }
}
