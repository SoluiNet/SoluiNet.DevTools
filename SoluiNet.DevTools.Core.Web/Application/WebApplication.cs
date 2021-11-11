// <copyright file="WebApplication.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using NLog;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.Web.Plugin;

    /// <summary>
    /// The web application.
    /// </summary>
    public class WebApplication : ISoluiNetWebApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplication"/> class.
        /// </summary>
        public WebApplication()
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

                    Logger.Debug(exception, string.Format(CultureInfo.InvariantCulture, "Couldn't load assembly '{0}'", dllFile));
                }
            }

            Type pluginType = typeof(IBasePlugin);
            Type backgroundTaskType = typeof(IRunsBackgroundTask);
            Type webPluginType = typeof(IProvidesWebCommunication);

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

                    if (type.GetInterface(webPluginType.FullName) != null)
                    {
                        if (pluginTypes.ContainsKey(type))
                        {
                            pluginTypes[type].Add("Web");
                        }
                        else
                        {
                            pluginTypes.Add(type, new List<string>() { "Web" });
                        }
                    }
                }
            }

            this.Plugins = new List<IBasePlugin>();
            this.BackgroundTaskPlugins = new List<IRunsBackgroundTask>();
            this.WebPlugins = new List<IProvidesWebCommunication>();

            var enabledPlugins = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Effective;

            foreach (var type in pluginTypes)
            {
                var assemblyName = type.Key.Assembly.GetName().Name;

                if (!enabledPlugins.ContainsKey(assemblyName) || !enabledPlugins[assemblyName])
                {
                    Logger.Info(string.Format(CultureInfo.InvariantCulture, "Found plugin '{0}' but it will be ignored because it isn't configured as enabled plugin.", assemblyName));

                    continue;
                }

                Logger.Info(string.Format(CultureInfo.InvariantCulture, "Load plugin '{0}'.", assemblyName));

                if (type.Value.Contains("PluginDev"))
                {
                    var plugin = (IBasePlugin)Activator.CreateInstance(type.Key);
                    this.Plugins.Add(plugin);
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

                if (type.Value.Contains("PluginDev"))
                {
                    var plugin = (IProvidesWebCommunication)Activator.CreateInstance(type.Key);
                    this.WebPlugins.Add(plugin);
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += PluginHelper.LoadAssembly;
        }

        /// <inheritdoc/>
        public ICollection<IProvidesWebCommunication> WebPlugins { get; }

        /// <inheritdoc/>
        public ICollection<IBasePlugin> Plugins { get; }

        /// <inheritdoc/>
        public ICollection<IRunsBackgroundTask> BackgroundTaskPlugins { get; }

        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }
    }
}
