﻿// <copyright file="BaseSoluiNetApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using NLog;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.Events;
    using SoluiNet.DevTools.Core.Services;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// The base implementation of a solui.net Application.
    /// </summary>
    public abstract class BaseSoluiNetApp : ISoluiNetApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSoluiNetApp"/> class.
        /// </summary>
        protected BaseSoluiNetApp()
        {
            this.Plugins = new List<IBasePlugin>();
            this.BackgroundTaskPlugins = new List<IRunsBackgroundTask>();
            this.Services = new List<ISoluiNetService>();
        }

        /// <inheritdoc />
        public ICollection<IBasePlugin> Plugins { get; }

        /// <inheritdoc />
        public ICollection<IRunsBackgroundTask> BackgroundTaskPlugins { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public ICollection<ISoluiNetService> Services { get; }

        /// <inheritdoc/>
        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        /// <summary>
        /// Initialize the application object.
        /// </summary>
        public void Initialize()
        {
            this.LoadServices();
        }

        /// <summary>
        /// Calls the startup event of the application.
        /// </summary>
        /// <param name="eventArguments">The event arguments.</param>
        public virtual void Startup(Dictionary<string, object> eventArguments = null)
        {
            if (eventArguments == null)
            {
                eventArguments = new Dictionary<string, object>();
            }

            foreach (var plugin in PluginHelper.GetPlugins<IHandlesEvent<IStartupEvent>>())
            {
                plugin.HandleEvent<IStartupEvent>(eventArguments);
            }
        }

        /// <summary>
        /// Calls the initialized event of the application.
        /// </summary>
        /// <param name="eventArguments">The event arguments.</param>
        public virtual void Initialized(Dictionary<string, object> eventArguments = null)
        {
            if (eventArguments == null)
            {
                eventArguments = new Dictionary<string, object>();
            }

            foreach (var plugin in PluginHelper.GetPlugins<IHandlesEvent<IInitializedEvent>>())
            {
                plugin.HandleEvent<IInitializedEvent>(eventArguments);
            }
        }

        /// <summary>
        /// Calls the shut down event of the application.
        /// </summary>
        /// <param name="eventArguments">The event arguments.</param>
        public virtual void Shutdown(Dictionary<string, object> eventArguments = null)
        {
            if (eventArguments == null)
            {
                eventArguments = new Dictionary<string, object>();
            }

            foreach (var plugin in PluginHelper.GetPlugins<IHandlesEvent<IShutdownEvent>>())
            {
                plugin.HandleEvent<IShutdownEvent>(eventArguments);
            }
        }

        /// <summary>
        /// Initialize the plugins.
        /// </summary>
        /// <typeparam name="TPlugin">The plugin type.</typeparam>
        /// <param name="pluginList">The plugin list.</param>
        /// <param name="alreadyInitializedList">The list of plugins that have already been initialized.</param>
        /// <param name="correspondingAssemblies">The list of corresponding assemblies which will be filled once a plugin has been added.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "We want to catch any exception that occurs during plugin load. Therefore the base exception type will be catched.")]
        protected static void InitializePlugins<TPlugin>(
            ICollection<TPlugin> pluginList,
            ICollection<IBasePlugin> alreadyInitializedList = null,
            ICollection<Assembly> correspondingAssemblies = null)
            where TPlugin : IBasePlugin
        {
            if (pluginList == null)
            {
                throw new ArgumentNullException(nameof(pluginList));
            }

            /*if (alreadyInitializedList == null)
            {
                throw new ArgumentNullException(nameof(alreadyInitializedList));
            }*/

            string[] dllFileNames = null;
            dllFileNames = Directory.GetFiles(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "*.dll",
                SearchOption.AllDirectories);

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
                    var assembly = Assembly.LoadFrom(dllFile);
                    assemblies.Add(assembly);
                }
                catch (BadImageFormatException exception)
                {
                    Debug.WriteLine(JsonTools.Serialize(exception));
                    Logger.Warn(exception, string.Format(CultureInfo.InvariantCulture, "Couldn't load '{0}'", dllFile));
                }
            }

            Type pluginType = typeof(TPlugin);

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
                            Logger.Debug("Found class '{0}' in assembly '{1}'", type.FullName, assembly.FullName);

                            if (alreadyInitializedList == null || !alreadyInitializedList.Any(x => x.GetType() == type))
                            {
                                Logger.Debug("Create new instance of '{0}' from assembly '{1}'", type.FullName, assembly.FullName);

                                var pluginObject = (TPlugin)Activator.CreateInstance(type);
                                alreadyInitializedList?.Add(pluginObject);

                                pluginList.Add(pluginObject);

                                if (correspondingAssemblies != null && !correspondingAssemblies.Contains(type.Assembly))
                                {
                                    correspondingAssemblies.Add(type.Assembly);
                                }
                            }
                            else
                            {
                                Logger.Debug("Add already initialized instance of '{0}' from assembly '{1}'", type.FullName, assembly.FullName);

                                pluginList.Add((TPlugin)alreadyInitializedList.First(x => x.GetType() == type));

                                if (correspondingAssemblies != null && !correspondingAssemblies.Contains(type.Assembly))
                                {
                                    correspondingAssemblies.Add(type.Assembly);
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException loadException)
                {
                    Logger.Fatal(
                        loadException,
                        "Error (Load Exception) while assigning plugin types for assembly '{0}': {1}",
                        assembly.FullName,
                        loadException.LoaderExceptions.Select(x => x.Message).Aggregate((x, y) => x + "\r\n" + y));
                }
                catch (Exception assignmentException)
                {
                    Logger.Fatal(assignmentException, "Error while assigning plugin types for assembly '{0}'", assembly.FullName);
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += BaseSoluiNetApp.LoadAssembly;
        }

        /// <summary>
        /// Provide a default implementation for the assembly resolve event.
        /// </summary>
        /// <param name="sender">The sender which triggered the event.</param>
        /// <param name="args">A <see cref="ResolveEventArgs"/> with additional arguments about the resolve event.</param>
        /// <returns>Returns an instance of <see cref="Assembly"/> which contains the assembly for which the event was looking for.</returns>
        /// <exception cref="ArgumentNullException">Throws <see cref="ArgumentNullException"/> if one or more arguments are null.</exception>
        private static Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
            {
                return null;
            }

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            var assemblyPluginPath = Path.Combine(folderPath, "Plugins", new AssemblyName(args.Name).Name + ".dll");
            var assemblyUiPath = Path.Combine(folderPath, "UI", new AssemblyName(args.Name).Name + ".dll");

            if (!System.IO.File.Exists(assemblyPath)
                && !System.IO.File.Exists(assemblyPluginPath)
                && !System.IO.File.Exists(assemblyUiPath))
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
            else if (System.IO.File.Exists(assemblyUiPath))
            {
                assembly = Assembly.LoadFrom(assemblyUiPath);
            }

            return assembly;
        }

        /// <summary>
        /// Load services for the application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "We want to catch any exception that occurs during plugin load. Therefore the base exception type will be catched.")]
        private void LoadServices()
        {
            string[] dllFileNames = null;
            dllFileNames = Directory.GetFiles(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "*.dll",
                SearchOption.AllDirectories);

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
                    Logger.Warn(exception, string.Format(CultureInfo.InvariantCulture, "Couldn't load '{0}'", dllFile));
                }
            }

            Type serviceType = typeof(ISoluiNetService);

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

                        if (type.GetInterface(serviceType.FullName) != null)
                        {
                            Logger.Debug("Found class '{0}' in assembly '{1}'", type.FullName, assembly.FullName);

                            if (!this.Services.Any(x => x.GetType() == type))
                            {
                                Logger.Debug("Create new instance of '{0}' from assembly '{1}'", type.FullName, assembly.FullName);

                                var serviceObject = (ISoluiNetService)Activator.CreateInstance(type);
                                this.Services.Add(serviceObject);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException loadException)
                {
                    Logger.Fatal(
                        loadException,
                        "Error (Load Exception) while assigning plugin types for assembly '{0}': {1}",
                        assembly.FullName,
                        loadException.LoaderExceptions.Select(x => x.Message).Aggregate((x, y) => x + "\r\n" + y));
                }
                catch (Exception assignmentException)
                {
                    Logger.Fatal(assignmentException, "Error while assigning plugin types for assembly '{0}'", assembly.FullName);
                }
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += BaseSoluiNetApp.LoadAssembly;
        }
    }
}
