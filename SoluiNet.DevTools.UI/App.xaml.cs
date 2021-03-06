﻿// <copyright file="App.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using NLog;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.Events;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.UI.UIElement;
    using SoluiNet.DevTools.Core.UI.WPF.Application;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Tools.UI;

    /// <summary>
    /// Interaction logic for "App.xaml".
    /// </summary>
    public partial class App : Application, ISoluiNetUiWpfApp
    {
        /// <summary>
        /// Gets all available plugins.
        /// </summary>
        public ICollection<IBasePlugin> Plugins { get; private set; }

        /// <summary>
        /// Gets all available plugins that provide database connectivity functions.
        /// </summary>
        public ICollection<ISqlUiPlugin> SqlPlugins { get; private set; }

        /// <summary>
        /// Gets all available plugins that provide utility functions.
        /// </summary>
        public ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; private set; }

        /// <summary>
        /// Gets all available plugins that will run in the background.
        /// </summary>
        public ICollection<IRunsBackgroundTask> BackgroundTaskPlugins { get; private set; }

        /// <inheritdoc/>
        public ICollection<ISoluiNetUIElement> UiElements { get; private set; }

        private Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <summary>
        /// Event handling for start up.
        /// </summary>
        /// <param name="e">The start up event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                this.Logger.Info(string.Format(CultureInfo.InvariantCulture, "Start SoluiNet.DevTools from '{0}'", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));

                this.LoadPlugins();
                this.LoadUiElements();

                App.CallStartupEvent();
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error in SoluiNet.DevTools.UI");
            }
        }

        /// <summary>
        /// Event handling for exit.
        /// </summary>
        /// <param name="e">The exit event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                base.OnExit(e);

                this.Logger.Info(string.Format(CultureInfo.InvariantCulture, "Stop SoluiNet.DevTools from '{0}'", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));

                App.CallShutdownEvent();
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error in SoluiNet.DevTools.UI");
            }
        }

        private static void CallStartupEvent()
        {
            foreach (var plugin in PluginHelper.GetPlugins<IHandlesEvent<IStartupEvent>>())
            {
                plugin.HandleEvent<IStartupEvent>(new Dictionary<string, object>());
            }
        }

        private static void CallShutdownEvent()
        {
            foreach (var plugin in PluginHelper.GetPlugins<IHandlesEvent<IShutdownEvent>>())
            {
                plugin.HandleEvent<IShutdownEvent>(new Dictionary<string, object>());
            }
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

                    this.Logger.Debug(exception, string.Format(CultureInfo.InvariantCulture, "Couldn't load assembly '{0}'", dllFile));
                }
            }

            Type pluginType = typeof(IBasePlugin);
            Type sqlPluginType = typeof(ISqlUiPlugin);
            Type utilityPluginType = typeof(IUtilitiesDevPlugin);
            Type backgroundTaskType = typeof(IRunsBackgroundTask);

            IDictionary<Type, List<string>> pluginTypes = new Dictionary<Type, List<string>>();

            try
            {
                foreach (var assembly in assemblies)
                {
                    if (assembly == null)
                    {
                        continue;
                    }

                    try
                    {
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
                    catch (Exception assignmentException)
                    {
                        this.Logger.Fatal(assignmentException, "Error while assigning plugin types for assembly '{0}'", assembly.FullName);
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.Fatal(exception, "Error while assigning plugin types");
            }

            this.Plugins = new List<IBasePlugin>();
            this.SqlPlugins = new List<ISqlUiPlugin>();
            this.UtilityPlugins = new List<IUtilitiesDevPlugin>();
            this.BackgroundTaskPlugins = new List<IRunsBackgroundTask>();

            var enabledPlugins = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Effective;

            foreach (var type in pluginTypes)
            {
                var assemblyName = type.Key.Assembly.GetName().Name;

                if (!enabledPlugins.ContainsKey(assemblyName) || !enabledPlugins[assemblyName])
                {
                    this.Logger.Info(string.Format(CultureInfo.InvariantCulture, "Found plugin '{0}' but it will be ignored because it isn't configured as enabled plugin.", assemblyName));

                    continue;
                }

                this.Logger.Info(string.Format(CultureInfo.InvariantCulture, "Load plugin '{0}'.", assemblyName));

                if (type.Value.Contains("PluginDev"))
                {
                    var plugin = (IBasePlugin)Activator.CreateInstance(type.Key);
                    this.Plugins.Add(plugin);
                }

                if (type.Value.Contains("SqlDev"))
                {
                    var plugin = (ISqlUiPlugin)Activator.CreateInstance(type.Key);
                    this.SqlPlugins.Add(plugin);
                }

                if (type.Value.Contains("UtilityDev"))
                {
                    var plugin = (IUtilitiesDevPlugin)Activator.CreateInstance(type.Key);
                    this.UtilityPlugins.Add(plugin);
                }

                if (type.Value.Contains("BackgroundTask"))
                {
                    var plugin = (IRunsBackgroundTask)Activator.CreateInstance(type.Key);
                    this.BackgroundTaskPlugins.Add(plugin);

                    Task.Run(() =>
                    {
                        plugin.ExecuteBackgroundTask();
                    });
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += PluginHelper.LoadAssembly;
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
            currentDomain.AssemblyResolve += UIHelper.LoadUiElementAssembly;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            this.Logger.Fatal(e.Exception, "Unhandled Exception while executing SoluiNet.DevTools.UI");
        }
    }
}
