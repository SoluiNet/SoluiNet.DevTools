// <copyright file="ICrossPlatformAssemblyResolver.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Interface for cross-platform assembly resolution.
    /// </summary>
    public interface ICrossPlatformAssemblyResolver
    {
        /// <summary>
        /// Resolves an assembly by name from the specified search paths.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to resolve.</param>
        /// <param name="searchPaths">The paths to search for the assembly.</param>
        /// <returns>The resolved assembly or null if not found.</returns>
        Assembly ResolveAssembly(string assemblyName, string[] searchPaths);

        /// <summary>
        /// Gets the plugin search paths for the current platform.
        /// </summary>
        /// <returns>An enumerable of search paths.</returns>
        IEnumerable<string> GetPluginSearchPaths();

        /// <summary>
        /// Determines if an assembly is compatible with the current platform and architecture.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly to check.</param>
        /// <returns>True if the assembly is compatible, false otherwise.</returns>
        bool IsCompatibleAssembly(string assemblyPath);

        /// <summary>
        /// Validates that an assembly can be loaded safely.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly to validate.</param>
        /// <returns>True if the assembly can be loaded safely, false otherwise.</returns>
        bool ValidateAssembly(string assemblyPath);

        /// <summary>
        /// Gets metadata for a plugin assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly.</param>
        /// <returns>The plugin metadata or null if not available.</returns>
        IPluginMetadata GetPluginMetadata(string assemblyPath);
    }
}