// <copyright file="App.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI
{
    using System;
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
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.Tools.UI;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.UIElement;

    /// <summary>
    /// Interaction logic for "App.xaml".
    /// </summary>
    public partial class App : Application, ISoluiNetApp
    {
        /// <summary>
        /// Gets or sets all available plugins.
        /// </summary>
        public ICollection<IBasePlugin> Plugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that provide database connectivity functions.
        /// </summary>
        public ICollection<ISqlDevPlugin> SqlPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that provide utility functions.
        /// </summary>
        public ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that will run in the background.
        /// </summary>
        public ICollection<IPluginWithBackgroundTask> BackgroundTaskPlugins { get; set; }

        /// <inheritdoc/>
        public ICollection<ISoluiNetUIElement> UiElements { get; set; }

        /// <summary>
        /// Event handling for start up.
        /// </summary>
        /// <param name="e">The start up event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.LoadPlugins();
            this.LoadUiElements();
        }

        private void LoadPlugins()
        {
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
                            pluginTypes.Add(type, new List<string>() { "PluginDev" });
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

            this.Plugins = new List<IBasePlugin>();
            this.SqlPlugins = new List<ISqlDevPlugin>();
            this.UtilityPlugins = new List<IUtilitiesDevPlugin>();
            this.BackgroundTaskPlugins = new List<IPluginWithBackgroundTask>();

            foreach (var type in pluginTypes)
            {
                if (type.Value.Contains("PluginDev"))
                {
                    var plugin = (IBasePlugin)Activator.CreateInstance(type.Key);
                    this.Plugins.Add(plugin);
                }

                if (type.Value.Contains("SqlDev"))
                {
                    var plugin = (ISqlDevPlugin)Activator.CreateInstance(type.Key);
                    this.SqlPlugins.Add(plugin);
                }

                if (type.Value.Contains("UtilityDev"))
                {
                    var plugin = (IUtilitiesDevPlugin)Activator.CreateInstance(type.Key);
                    this.UtilityPlugins.Add(plugin);
                }

                if (type.Value.Contains("BackgroundTask"))
                {
                    var plugin = (IPluginWithBackgroundTask)Activator.CreateInstance(type.Key);
                    this.BackgroundTaskPlugins.Add(plugin);

                    Task.Run(() =>
                    {
                        (plugin as IPluginWithBackgroundTask).ExecuteBackgroundTask();
                    });
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(PluginHelper.LoadAssembly);
        }

        private void LoadUiElements()
        {
            string[] dllFileNames = null;
            if (Directory.Exists("UI"))
            {
                dllFileNames = Directory.GetFiles("UI", "*.dll");
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

            Type uiElementType = typeof(ISoluiNetUIElement);

            IDictionary<Type, List<string>> uiElementTypes = new Dictionary<Type, List<string>>();

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

                    if (type.GetInterface(uiElementType.FullName) != null)
                    {
                        if (uiElementTypes.ContainsKey(type))
                        {
                            uiElementTypes[type].Add("UiElement");
                        }
                        else
                        {
                            uiElementTypes.Add(type, new List<string>() { "UiElement" });
                        }
                    }
                }
            }

            this.UiElements = new List<ISoluiNetUIElement>();

            foreach (var type in uiElementTypes)
            {
                if (type.Value.Contains("UiElement"))
                {
                    var uiElement = (ISoluiNetUIElement)Activator.CreateInstance(type.Key);
                    this.UiElements.Add(uiElement);
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(UIHelper.LoadUiElementAssembly);
        }
    }
}
