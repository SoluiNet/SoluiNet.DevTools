using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using SoluiNet.DevTools.Core;

namespace SoluiNet.DevTools.UI
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        internal ICollection<ISqlDevPlugin> Plugins { get; set; }

        internal ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; set; }

        static Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
                return null;

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            var assemblyPluginPath = Path.Combine(folderPath, "Plugins", new AssemblyName(args.Name).Name + ".dll");

            if (!File.Exists(assemblyPath) && !File.Exists(assemblyPluginPath))
                return null;

            Assembly assembly = null;

            if (File.Exists(assemblyPath))
                assembly = Assembly.LoadFrom(assemblyPath);
            else if (File.Exists(assemblyPluginPath))
                assembly = Assembly.LoadFrom(assemblyPluginPath);

            return assembly;
        }

        /// <summary>
        /// Event handling for start up
        /// </summary>
        /// <param name="e">The start up event</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string[] dllFileNames = null;
            if (Directory.Exists("Plugins"))
            {
                dllFileNames = Directory.GetFiles("Plugins", "*.dll");
            }

            if (dllFileNames == null)
            {
                return;
            }

            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                var an = AssemblyName.GetAssemblyName(dllFile);
                var assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }

            Type pluginType = typeof(ISqlDevPlugin);
            Type utilityPluginType = typeof(IUtilitiesDevPlugin);
            IDictionary<Type, string> pluginTypes = new Dictionary<Type, string>();
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
                        pluginTypes.Add(type, "SqlDev");
                    }
                    else if (type.GetInterface(utilityPluginType.FullName) != null)
                    {
                        pluginTypes.Add(type, "UtilityDev");
                    }
                }
            }

            Plugins = new List<ISqlDevPlugin>();
            UtilityPlugins = new List<IUtilitiesDevPlugin>();

            foreach (var type in pluginTypes)
            {
                if (type.Value == "SqlDev")
                {
                    var plugin = (ISqlDevPlugin)Activator.CreateInstance(type.Key);
                    Plugins.Add(plugin);
                }
                else if (type.Value == "UtilityDev")
                {
                    var plugin = (IUtilitiesDevPlugin)Activator.CreateInstance(type.Key);
                    UtilityPlugins.Add(plugin);
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadAssembly);
        }
    }
}
