// <copyright file="ICrossPlatformPluginLoader.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for cross-platform plugin loading.
    /// </summary>
    public interface ICrossPlatformPluginLoader
    {
        /// <summary>
        /// Loads all plugins of the specified type.
        /// </summary>
        /// <typeparam name="T">The plugin type to load.</typeparam>
        /// <returns>An enumerable of loaded plugin instances.</returns>
        IEnumerable<T> LoadPlugins<T>() where T : class;

        /// <summary>
        /// Loads a specific plugin by name.
        /// </summary>
        /// <typeparam name="T">The plugin type to load.</typeparam>
        /// <param name="pluginName">The name of the plugin to load.</param>
        /// <returns>The loaded plugin instance or null if not found.</returns>
        T LoadPlugin<T>(string pluginName) where T : class, IBasePlugin;

        /// <summary>
        /// Loads plugins from a specific assembly path.
        /// </summary>
        /// <typeparam name="T">The plugin type to load.</typeparam>
        /// <param name="assemblyPath">The path to the assembly to load plugins from.</param>
        /// <returns>An enumerable of loaded plugin instances from the assembly.</returns>
        IEnumerable<T> LoadPluginsFromAssembly<T>(string assemblyPath) where T : class;

        /// <summary>
        /// Gets information about incompatible plugins that could not be loaded.
        /// </summary>
        /// <returns>An enumerable of incompatible plugin information.</returns>
        IEnumerable<IncompatiblePluginInfo> GetIncompatiblePlugins();

        /// <summary>
        /// Validates that a plugin can be loaded before attempting to load it.
        /// </summary>
        /// <param name="assemblyPath">The path to the plugin assembly.</param>
        /// <returns>True if the plugin can be loaded, false otherwise.</returns>
        bool CanLoadPlugin(string assemblyPath);
    }

    /// <summary>
    /// Information about an incompatible plugin.
    /// </summary>
    public class IncompatiblePluginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatiblePluginInfo"/> class.
        /// </summary>
        /// <param name="assemblyPath">The path to the incompatible assembly.</param>
        /// <param name="reason">The reason why the plugin is incompatible.</param>
        /// <param name="exception">The exception that occurred during loading, if any.</param>
        public IncompatiblePluginInfo(string assemblyPath, string reason, Exception exception = null)
        {
            this.AssemblyPath = assemblyPath;
            this.Reason = reason;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the path to the incompatible assembly.
        /// </summary>
        public string AssemblyPath { get; }

        /// <summary>
        /// Gets the reason why the plugin is incompatible.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the exception that occurred during loading, if any.
        /// </summary>
        public Exception Exception { get; }
    }
}