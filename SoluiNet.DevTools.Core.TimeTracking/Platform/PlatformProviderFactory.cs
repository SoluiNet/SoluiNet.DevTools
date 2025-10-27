// <copyright file="PlatformProviderFactory.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Platform
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;

    /// <summary>
    /// Factory for creating platform-specific providers.
    /// </summary>
    public static class PlatformProviderFactory
    {
        /// <summary>
        /// Creates a platform provider for the current operating system.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <returns>A platform provider instance, or null if no suitable provider is available.</returns>
        public static IPlatformProvider? CreateProvider(ILogger? logger = null)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    logger?.LogDebug("Creating Windows platform provider");
                    return new WindowsPlatformProvider(logger);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    logger?.LogDebug("Creating Linux platform provider");
                    return new LinuxPlatformProvider(logger);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    logger?.LogDebug("Creating macOS platform provider");
                    return new MacOSPlatformProvider(logger);
                }

                logger?.LogWarning("No suitable platform provider found for current OS: {OS}", RuntimeInformation.OSDescription);
                return null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to create platform provider");
                return null;
            }
        }

        /// <summary>
        /// Gets the current platform name.
        /// </summary>
        /// <returns>The platform name.</returns>
        public static string GetCurrentPlatformName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "macOS";
            }

            return "Unknown";
        }

        /// <summary>
        /// Checks if the current platform is supported.
        /// </summary>
        /// <returns>True if the current platform is supported, false otherwise.</returns>
        public static bool IsCurrentPlatformSupported()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                   RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                   RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
    }
}