// <copyright file="CrossPlatformPluginFactory.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Factory for creating cross-platform plugin system components.
    /// </summary>
    public static class CrossPlatformPluginFactory
    {
        private static ICrossPlatformAssemblyResolver assemblyResolver;
        private static ICrossPlatformPluginDiscovery pluginDiscovery;
        private static ICrossPlatformPluginLoader pluginLoader;

        /// <summary>
        /// Gets the assembly resolver instance.
        /// </summary>
        /// <returns>The assembly resolver.</returns>
        public static ICrossPlatformAssemblyResolver GetAssemblyResolver()
        {
            if (assemblyResolver == null)
            {
                var platformService = PlatformServiceFactory.Create();
                assemblyResolver = new CrossPlatformAssemblyResolver(platformService);
            }

            return assemblyResolver;
        }

        /// <summary>
        /// Gets the plugin discovery instance.
        /// </summary>
        /// <returns>The plugin discovery service.</returns>
        public static ICrossPlatformPluginDiscovery GetPluginDiscovery()
        {
            if (pluginDiscovery == null)
            {
                pluginDiscovery = new CrossPlatformPluginDiscovery(GetAssemblyResolver());
            }

            return pluginDiscovery;
        }

        /// <summary>
        /// Gets the plugin loader instance.
        /// </summary>
        /// <returns>The plugin loader.</returns>
        public static ICrossPlatformPluginLoader GetPluginLoader()
        {
            if (pluginLoader == null)
            {
                pluginLoader = new CrossPlatformPluginLoader(GetPluginDiscovery(), GetAssemblyResolver());
            }

            return pluginLoader;
        }

        /// <summary>
        /// Creates a new plugin compatibility validator.
        /// </summary>
        /// <returns>A new plugin compatibility validator instance.</returns>
        public static PluginCompatibilityValidator CreateCompatibilityValidator()
        {
            var platformService = PlatformServiceFactory.Create();
            return new PluginCompatibilityValidator(platformService);
        }

        /// <summary>
        /// Resets all cached instances (useful for testing).
        /// </summary>
        public static void Reset()
        {
            assemblyResolver = null;
            pluginDiscovery = null;
            pluginLoader = null;
        }
    }
}