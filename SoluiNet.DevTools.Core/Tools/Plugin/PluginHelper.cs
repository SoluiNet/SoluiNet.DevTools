using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SoluiNet.DevTools.Core.ScriptEngine;
using SoluiNet.DevTools.Core.Tools.XML;

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

        public static IList<string> GetEmbeddedResources(IBasePlugin plugin, string type = "Script")
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

            foreach (var scriptContainer in embeddedScripts)
            {
                var scriptsStream = plugin.GetType().Assembly.GetManifestResourceStream(scriptContainer);

                if (scriptsStream == null)
                    continue;

                var scripts = XmlHelper.Deserialize<SqlScripts>(scriptsStream);

                scriptList.AddRange(scripts.SqlScript);
            }

            return scriptList;
        }

        public static WebClientDefinition.SoluiNetWebClientDefinition GetWebClientDefinition(IWebClientSupportPlugin plugin)
        {
            if (plugin == null)
                return null;

            var embeddedWebClientDefs = GetEmbeddedResources(plugin, "WebClientDefinition");

            var webClientDefContainer = embeddedWebClientDefs.FirstOrDefault();

            if (webClientDefContainer == null)
                return null;

            var webClientDefStream = plugin.GetType().Assembly.GetManifestResourceStream(webClientDefContainer);

            if (webClientDefStream == null)
                return null;

            var webClientDef = XmlHelper.Deserialize<WebClientDefinition.SoluiNetWebClientDefinition>(webClientDefStream);

            return webClientDef;
        }

        public static Settings.SoluiNetSettingType GetSettings(IPluginWithSettings plugin)
        {
            if (plugin == null)
                return null;

            var embeddedSettings = GetEmbeddedResources(plugin, "Settings");

            var settingsContainer = embeddedSettings.FirstOrDefault();

            if (settingsContainer == null)
                return null;

            var settingsStream = plugin.GetType().Assembly.GetManifestResourceStream(settingsContainer);

            if (settingsStream == null)
                return null;

            var settings = XmlHelper.Deserialize<Settings.SoluiNetSettingType>(settingsStream);

            return settings;
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

        public static Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
                return null;

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            var assemblyPluginPath = Path.Combine(folderPath, "Plugins", new AssemblyName(args.Name).Name + ".dll");

            if (!System.IO.File.Exists(assemblyPath) && !System.IO.File.Exists(assemblyPluginPath))
                return null;

            Assembly assembly = null;

            if (System.IO.File.Exists(assemblyPath))
                assembly = Assembly.LoadFrom(assemblyPath);
            else if (System.IO.File.Exists(assemblyPluginPath))
                assembly = Assembly.LoadFrom(assemblyPluginPath);

            return assembly;
        }

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

        public static T GetPluginByName<T> (string name) where T : IBasePlugin
        {
            var pluginList = GetPlugins<T>();

            foreach(var plugin in pluginList)
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
