﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Tools;
using SoluiNet.DevTools.Core.Tools.Json;

namespace SoluiNet.DevTools.UI
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        internal ICollection<IBasePlugin> Plugins { get; set; }

        internal ICollection<ISqlDevPlugin> SqlPlugins { get; set; }

        internal ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; set; }

        internal ICollection<IPluginWithBackgroundTask> BackgroundTaskPlugins { get; set; }

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
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }
                catch (BadImageFormatException exception)
                {
                    Debug.WriteLine(JsonTools.Serialize(exception));
                }
            }

            Type pluginType = typeof(IBasePlugin);
            Type sqlPluginType = typeof(ISqlDevPlugin);
            Type utilityPluginType = typeof(IUtilitiesDevPlugin);
            Type backgroundTaskType = typeof(IPluginWithBackgroundTask);

            IDictionary<Type, List<string>> pluginTypes = new Dictionary<Type, List<string>>();

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
                        if (pluginTypes.ContainsKey(type))
                        {
                            pluginTypes[type].Add("PluginDev");
                        }
                        else
                        {
                            pluginTypes.Add(type, new List<string> () { "PluginDev" });
                        }
                    }

                    if (type.GetInterface(sqlPluginType.FullName) != null)
                    {
                        if (pluginTypes.ContainsKey(type))
                        {
                            pluginTypes[type].Add("SqlDev");
                        }
                        else
                        {
                            pluginTypes.Add(type, new List<string>() { "SqlDev" });
                        }
                    }

                    if (type.GetInterface(utilityPluginType.FullName) != null)
                    {
                        if (pluginTypes.ContainsKey(type))
                        {
                            pluginTypes[type].Add("UtilityDev");
                        }
                        else
                        {
                            pluginTypes.Add(type, new List<string>() { "UtilityDev" });
                        }
                    }

                    if (type.GetInterface(backgroundTaskType.FullName) != null)
                    {
                        if (pluginTypes.ContainsKey(type))
                        {
                            pluginTypes[type].Add("BackgroundTask");
                        }
                        else
                        {
                            pluginTypes.Add(type, new List<string>() { "BackgroundTask" });
                        }
                    }
                }
            }

            Plugins = new List<IBasePlugin>();
            SqlPlugins = new List<ISqlDevPlugin>();
            UtilityPlugins = new List<IUtilitiesDevPlugin>();
            BackgroundTaskPlugins = new List<IPluginWithBackgroundTask>();

            foreach (var type in pluginTypes)
            {
                if (type.Value.Contains("PluginDev"))
                {
                    var plugin = (IBasePlugin)Activator.CreateInstance(type.Key);
                    Plugins.Add(plugin);
                }

                if (type.Value.Contains("SqlDev"))
                {
                    var plugin = (ISqlDevPlugin)Activator.CreateInstance(type.Key);
                    SqlPlugins.Add(plugin);
                }

                if (type.Value.Contains("UtilityDev"))
                {
                    var plugin = (IUtilitiesDevPlugin)Activator.CreateInstance(type.Key);
                    UtilityPlugins.Add(plugin);
                }

                if (type.Value.Contains("BackgroundTask"))
                {
                    var plugin = (IPluginWithBackgroundTask)Activator.CreateInstance(type.Key);
                    BackgroundTaskPlugins.Add(plugin);

                    Task.Run(() =>
                    {
                        (plugin as IPluginWithBackgroundTask).ExecuteBackgroundTask();
                    });
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(PluginHelper.LoadAssembly);
        }
    }
}
