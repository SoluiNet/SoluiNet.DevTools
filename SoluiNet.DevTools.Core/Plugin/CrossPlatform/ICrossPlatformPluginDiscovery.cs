// <copyright file="ICrossPlatformPluginDiscovery.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for cross-platform plugin discovery.
    /// </summary>
    public interface ICrossPlatformPluginDiscovery
    {
        /// <summary>
        /// Discovers all plugin assemblies in the configured search paths.
        /// </summary>
        /// <returns>An enumerable of plugin assembly paths.</returns>
        IEnumerable<string> DiscoverPluginAssemblies();

        /// <summary>
        /// Discovers plugin assemblies of a specific type.
        /// </summary>
        /// <typeparam name="T">The plugin type to discover.</typeparam>
        /// <returns>An enumerable of plugin assembly paths that contain the specified type.</returns>
        IEnumerable<string> DiscoverPluginAssemblies<T>() where T : class;

        /// <summary>
        /// Gets all compatible plugin assemblies for the current platform.
        /// </summary>
        /// <returns>An enumerable of compatible plugin assembly paths.</returns>
        IEnumerable<string> GetCompatiblePluginAssemblies();

        /// <summary>
        /// Validates that a plugin assembly is compatible and can be loaded.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly to validate.</param>
        /// <returns>True if the assembly is valid and compatible, false otherwise.</returns>
        bool ValidatePluginAssembly(string assemblyPath);
    }
}