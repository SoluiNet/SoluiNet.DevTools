// <copyright file="CrossPlatformPluginDiscovery.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using NLog;

    /// <summary>
    /// Cross-platform plugin discovery implementation.
    /// </summary>
    public class CrossPlatformPluginDiscovery : ICrossPlatformPluginDiscovery
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICrossPlatformAssemblyResolver assemblyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossPlatformPluginDiscovery"/> class.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        public CrossPlatformPluginDiscovery(ICrossPlatformAssemblyResolver assemblyResolver)
        {
            this.assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
        }

        /// <inheritdoc />
        public IEnumerable<string> DiscoverPluginAssemblies()
        {
            var discoveredAssemblies = new List<string>();
            var searchPaths = this.assemblyResolver.GetPluginSearchPaths();

            foreach (var searchPath in searchPaths)
            {
                if (!Directory.Exists(searchPath))
                {
                    Logger.Debug("Search path does not exist: {0}", searchPath);
                    continue;
                }

                try
                {
                    // Use platform-agnostic file enumeration
                    var dllFiles = Directory.EnumerateFiles(
                        searchPath,
                        "*.dll",
                        SearchOption.TopDirectoryOnly);

                    foreach (var dllFile in dllFiles)
                    {
                        if (this.ValidatePluginAssembly(dllFile))
                        {
                            discoveredAssemblies.Add(dllFile);
                            Logger.Debug("Discovered plugin assembly: {0}", dllFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error discovering plugins in path: {0}", searchPath);
                }
            }

            return discoveredAssemblies.Distinct();
        }

        /// <inheritdoc />
        public IEnumerable<string> DiscoverPluginAssemblies<T>() where T : class
        {
            var discoveredAssemblies = new List<string>();
            var allAssemblies = this.DiscoverPluginAssemblies();

            foreach (var assemblyPath in allAssemblies)
            {
                try
                {
                    if (this.AssemblyContainsType<T>(assemblyPath))
                    {
                        discoveredAssemblies.Add(assemblyPath);
                        Logger.Debug("Assembly {0} contains type {1}", assemblyPath, typeof(T).Name);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex, "Error checking assembly {0} for type {1}", assemblyPath, typeof(T).Name);
                }
            }

            return discoveredAssemblies;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetCompatiblePluginAssemblies()
        {
            var compatibleAssemblies = new List<string>();
            var allAssemblies = this.DiscoverPluginAssemblies();

            foreach (var assemblyPath in allAssemblies)
            {
                if (this.assemblyResolver.IsCompatibleAssembly(assemblyPath))
                {
                    compatibleAssemblies.Add(assemblyPath);
                    Logger.Debug("Assembly {0} is compatible with current platform", assemblyPath);
                }
                else
                {
                    Logger.Debug("Assembly {0} is not compatible with current platform", assemblyPath);
                }
            }

            return compatibleAssemblies;
        }

        /// <inheritdoc />
        public bool ValidatePluginAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return false;
            }

            try
            {
                // Skip system assemblies and known non-plugin assemblies
                var fileName = Path.GetFileNameWithoutExtension(assemblyPath);
                if (this.IsSystemAssembly(fileName))
                {
                    return false;
                }

                // Use the assembly resolver to validate
                return this.assemblyResolver.ValidateAssembly(assemblyPath) &&
                       this.assemblyResolver.IsCompatibleAssembly(assemblyPath);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to validate plugin assembly: {0}", assemblyPath);
                return false;
            }
        }

        private bool AssemblyContainsType<T>(string assemblyPath) where T : class
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var targetType = typeof(T);

                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }

                    // Check if type implements the target interface
                    if (targetType.IsInterface && type.GetInterface(targetType.FullName) != null)
                    {
                        return true;
                    }

                    // Check if type inherits from the target class
                    if (!targetType.IsInterface && targetType.IsAssignableFrom(type))
                    {
                        return true;
                    }

                    // Handle generic interfaces
                    if (targetType.IsInterface && targetType.IsGenericType)
                    {
                        var typeInterfaces = type.GetInterfaces()
                            .Where(x => x.IsGenericType && 
                                       x.GetGenericTypeDefinition().FullName == targetType.GetGenericTypeDefinition().FullName);

                        if (typeInterfaces.Any())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Error checking if assembly {0} contains type {1}", assemblyPath, typeof(T).Name);
                return false;
            }
        }

        private bool IsSystemAssembly(string fileName)
        {
            var systemPrefixes = new[]
            {
                "System.",
                "Microsoft.",
                "mscorlib",
                "netstandard",
                "WindowsBase",
                "PresentationCore",
                "PresentationFramework",
                "NLog",
                "Newtonsoft.Json"
            };

            return systemPrefixes.Any(prefix => 
                fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}