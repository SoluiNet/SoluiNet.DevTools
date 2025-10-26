// <copyright file="PlatformHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services.Platform
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helper class providing platform detection and information using OperatingSystem and RuntimeInformation APIs.
    /// </summary>
    public static class PlatformHelper
    {
        /// <summary>
        /// Gets a value indicating whether the current platform is Windows.
        /// </summary>
        public static bool IsWindows => OperatingSystem.IsWindows();

        /// <summary>
        /// Gets a value indicating whether the current platform is Linux.
        /// </summary>
        public static bool IsLinux => OperatingSystem.IsLinux();

        /// <summary>
        /// Gets a value indicating whether the current platform is macOS.
        /// </summary>
        public static bool IsMacOS => OperatingSystem.IsMacOS();

        /// <summary>
        /// Gets a value indicating whether the current platform is a Unix-like system (Linux or macOS).
        /// </summary>
        public static bool IsUnixLike => IsLinux || IsMacOS;

        /// <summary>
        /// Gets the current process architecture.
        /// </summary>
        public static Architecture Architecture => RuntimeInformation.ProcessArchitecture;

        /// <summary>
        /// Gets the runtime identifier for the current platform and architecture.
        /// </summary>
        public static string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;

        /// <summary>
        /// Gets the operating system description.
        /// </summary>
        public static string OSDescription => RuntimeInformation.OSDescription;

        /// <summary>
        /// Gets the .NET framework description.
        /// </summary>
        public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;

        /// <summary>
        /// Gets a value indicating whether the current process is running on ARM64 architecture.
        /// </summary>
        public static bool IsArm64 => Architecture == Architecture.Arm64;

        /// <summary>
        /// Gets a value indicating whether the current process is running on x64 architecture.
        /// </summary>
        public static bool IsX64 => Architecture == Architecture.X64;

        /// <summary>
        /// Gets a friendly name for the current platform.
        /// </summary>
        public static string PlatformName
        {
            get
            {
                if (IsWindows)
                {
                    return "Windows";
                }

                if (IsLinux)
                {
                    return "Linux";
                }

                if (IsMacOS)
                {
                    return "macOS";
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Gets a detailed platform description including OS and architecture.
        /// </summary>
        /// <returns>A detailed string describing the current platform.</returns>
        public static string GetDetailedPlatformInfo()
        {
            return $"{PlatformName} {Architecture} ({RuntimeIdentifier})";
        }

        /// <summary>
        /// Determines if the current platform supports a specific feature based on conditional compilation.
        /// </summary>
        /// <returns>True if Windows-specific features are available.</returns>
        public static bool SupportsWindowsFeatures()
        {
#if WINDOWS
            return true;
#else
            return IsWindows;
#endif
        }

        /// <summary>
        /// Determines if the current platform supports Unix-like features based on conditional compilation.
        /// </summary>
        /// <returns>True if Unix-like features are available.</returns>
        public static bool SupportsUnixFeatures()
        {
#if LINUX || MACOS
            return true;
#else
            return IsUnixLike;
#endif
        }
    }
}