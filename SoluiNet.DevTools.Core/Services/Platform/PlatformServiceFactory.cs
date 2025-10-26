// <copyright file="PlatformServiceFactory.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services.Platform
{
    using System;

    /// <summary>
    /// Factory class for creating platform-specific service instances using built-in .NET platform detection.
    /// </summary>
    public static class PlatformServiceFactory
    {
        /// <summary>
        /// Creates the appropriate platform service instance based on the current operating system.
        /// </summary>
        /// <returns>An instance of <see cref="IPlatformService"/> for the current platform.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported.</exception>
        public static IPlatformService Create()
        {
            if (OperatingSystem.IsWindows())
            {
                return new WindowsPlatformService();
            }

            if (OperatingSystem.IsLinux())
            {
                return new LinuxPlatformService();
            }

            if (OperatingSystem.IsMacOS())
            {
                return new MacOSPlatformService();
            }

            throw new PlatformNotSupportedException($"The current platform is not supported. Detected OS: {Environment.OSVersion}");
        }

        /// <summary>
        /// Gets a value indicating whether the current platform is supported.
        /// </summary>
        /// <returns>True if the current platform is supported, false otherwise.</returns>
        public static bool IsCurrentPlatformSupported()
        {
            return OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();
        }
    }
}