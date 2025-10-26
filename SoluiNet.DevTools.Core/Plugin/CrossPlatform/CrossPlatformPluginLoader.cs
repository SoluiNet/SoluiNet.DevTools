// <copyright file="CrossPlatformPluginLoader.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NLog;
    using SoluiNet.DevTools.Core.Application;

    /// <summary>
    /// Cross-platform plugin loader implementation.
    /// </summary>
    public class CrossPlatformPluginLoader : ICrossPlatformPluginLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICrossPlatformPluginDiscovery pluginDiscovery;
        private readonly ICrossPlatformAssemblyResolver assemblyResolver;
        private readonly List<IncompatiblePluginInfo> incompatiblePlugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossPlatformPluginLoader"/> class.
        /// </summary>
        /// <param name="pluginDiscovery">The plugin discovery service.</param>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        public CrossPlatformPluginLoader(
            ICrossPlatformPluginDiscovery pluginDiscovery,
            ICrossPlatformAssemblyResolver assemblyResolver)
        {
            this.pluginDiscovery = pluginDiscovery ?? throw new ArgumentNullException(nameof(pluginDiscovery));
            this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
            this.incompatiblePlugins = new List<IncompatiblePluginInfo>();
        }

        /// <inheritdoc />
        public IEnumerable<T> LoadPlugins<T>()
            where T : class
        {
            var loadedPlugins = new List<T>();
            var compatibleAssemblies = this.pluginDiscovery.GetCompatiblePluginAssemblies();

            foreach (var assemblyPath in compatibleAssemblies)
            {
                try
                {
                    var pluginsFromAssembly = this.LoadPluginsFromAssembly<T>(assemblyPath);
                    loadedPlugins.AddRange(pluginsFromAssembly);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to load plugins from assembly: {0}", assemblyPath);
                    this.incompatiblePlugins.Add(new IncompatiblePluginInfo(
                        assemblyPath,
                        "Failed to load plugins from assembly",
                        ex));
                }
            }

            Logger.Info("Loaded {0} plugins of type {1}", loadedPlugins.Count, typeof(T).Name);
            return loadedPlugins;
        }

        /// <inheritdoc />
        public T LoadPlugin<T>(string pluginName)
            where T : class, IBasePlugin
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                return null;
            }

            var allPlugins = this.LoadPlugins<T>();
            return allPlugins.FirstOrDefault(p => p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public IEnumerable<T> LoadPluginsFromAssembly<T>(string assemblyPath)
            where T : class
        {
            var loadedPlugins = new List<T>();

            if (!this.CanLoadPlugin(assemblyPath))
            {
                Logger.Debug("Cannot load plugin from assembly: {0}", assemblyPath);
                return loadedPlugins;
            }

            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var pluginType = typeof(T);

                // Check if plugin is enabled in configuration
                var assemblyName = assembly.GetName().Name;
                var enabledPlugins = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Effective;

                if (enabledPlugins.ContainsKey(assemblyName) && !enabledPlugins[assemblyName])
                {
                    Logger.Debug("Plugin {0} is disabled in configuration", assemblyName);
                    return loadedPlugins;
                }

                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }

                    if (this.IsPluginType<T>(type, pluginType))
                    {
                        try
                        {
                            var plugin = this.CreatePluginInstance<T>(type);
                            if (plugin != null)
                            {
                                loadedPlugins.Add(plugin);
                                Logger.Debug("Loaded plugin: {0} from assembly: {1}", type.FullName, assemblyPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Failed to create instance of plugin: {0}", type.FullName);
                            this.incompatiblePlugins.Add(new IncompatiblePluginInfo(
                                assemblyPath,
                                $"Failed to create instance of plugin: {type.FullName}",
                                ex));
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Logger.Error(ex, "ReflectionTypeLoadException loading assembly: {0}", assemblyPath);
                this.incompatiblePlugins.Add(new IncompatiblePluginInfo(
                    assemblyPath,
                    "ReflectionTypeLoadException during assembly loading",
                    ex));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load plugins from assembly: {0}", assemblyPath);
                this.incompatiblePlugins.Add(new IncompatiblePluginInfo(
                    assemblyPath,
                    "Failed to load assembly",
                    ex));
            }

            return loadedPlugins;
        }

        /// <inheritdoc />
        public IEnumerable<IncompatiblePluginInfo> GetIncompatiblePlugins()
        {
            return this.incompatiblePlugins.AsReadOnly();
        }

        /// <inheritdoc />
        public bool CanLoadPlugin(string assemblyPath)
        {
            return this.pluginDiscovery.ValidatePluginAssembly(assemblyPath) &&
                   this.assemblyResolver.IsCompatibleAssembly(assemblyPath);
        }

        private bool IsPluginType<T>(Type type, Type pluginType)
            where T : class
        {
            // Check if type implements the target interface
            if (pluginType.IsInterface && type.GetInterface(pluginType.FullName) != null)
            {
                return true;
            }

            // Check if type inherits from the target class
            if (!pluginType.IsInterface && pluginType.IsAssignableFrom(type))
            {
                return true;
            }

            // Handle generic interfaces
            if (pluginType.IsInterface && pluginType.IsGenericType)
            {
                var typeInterfaces = type.GetInterfaces()
                    .Where(x => x.IsGenericType &&
                               x.GetGenericTypeDefinition().FullName == pluginType.GetGenericTypeDefinition().FullName);

                if (typeInterfaces.Any())
                {
                    foreach (var typeInterface in typeInterfaces)
                    {
                        if (this.VerifyGenericType(pluginType, typeInterface))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private T CreatePluginInstance<T>(Type type)
            where T : class
        {
            // Check if plugin instance already exists in application context
            if (ApplicationContext.Application?.Plugins != null)
            {
                var existingPlugin = ApplicationContext.Application.Plugins
                    .FirstOrDefault(x => x.GetType() == type);

                if (existingPlugin is T existingTypedPlugin)
                {
                    return existingTypedPlugin;
                }
            }

            // Create new instance
            var plugin = (T)Activator.CreateInstance(type);

            // Add to application context if it's a base plugin
            if (plugin is IBasePlugin basePlugin && ApplicationContext.Application?.Plugins != null)
            {
                ApplicationContext.Application.Plugins.Add(basePlugin);
            }

            return plugin;
        }

        private bool VerifyGenericType(Type sourceType, Type comparingType)
        {
            var sourceGenericArguments = sourceType.GetGenericArguments();
            var comparisonGenericArguments = comparingType.GetGenericArguments();

            if (sourceGenericArguments.Length != comparisonGenericArguments.Length)
            {
                return false;
            }

            for (int i = 0; i < sourceGenericArguments.Length; i++)
            {
                var genericArgument = sourceGenericArguments[i];

                if (genericArgument.FullName != comparisonGenericArguments[i].FullName)
                {
                    return false;
                }
            }

            return true;
        }
    }
}