// <copyright file="StoragePathProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Storage
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides platform-specific storage paths for the time tracking database.
    /// </summary>
    public static class StoragePathProvider
    {
        /// <summary>
        /// Gets the appropriate database path for the current platform.
        /// </summary>
        /// <returns>The database file path.</returns>
        public static string GetDatabasePath()
        {
            var baseDirectory = GetDataDirectory();
            var databaseFileName = "TimeTracking.db";

            return Path.Combine(baseDirectory, databaseFileName);
        }

        /// <summary>
        /// Gets the appropriate configuration directory for the current platform.
        /// </summary>
        /// <returns>The configuration directory path.</returns>
        public static string GetConfigurationDirectory()
        {
            return GetDataDirectory();
        }

        /// <summary>
        /// Gets the appropriate log directory for the current platform.
        /// </summary>
        /// <returns>The log directory path.</returns>
        public static string GetLogDirectory()
        {
            var baseDirectory = GetDataDirectory();
            return Path.Combine(baseDirectory, "logs");
        }

        /// <summary>
        /// Gets the base data directory for the current platform.
        /// </summary>
        /// <returns>The base data directory path.</returns>
        private static string GetDataDirectory()
        {
            var applicationName = "SoluiNet.DevTools.TimeTracking";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: Use LocalApplicationData
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localAppData, applicationName);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: Use XDG_DATA_HOME or ~/.local/share
                var xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (!string.IsNullOrEmpty(xdgDataHome))
                {
                    return Path.Combine(xdgDataHome, applicationName);
                }

                var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homeDirectory, ".local", "share", applicationName);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS: Use ~/Library/Application Support
                var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homeDirectory, "Library", "Application Support", applicationName);
            }
            else
            {
                // Fallback: Use user profile directory
                var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homeDirectory, $".{applicationName.ToLowerInvariant()}");
            }
        }

        /// <summary>
        /// Ensures that the specified directory exists.
        /// </summary>
        /// <param name="directoryPath">The directory path to ensure exists.</param>
        public static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Gets the temporary directory for the current platform.
        /// </summary>
        /// <returns>The temporary directory path.</returns>
        public static string GetTempDirectory()
        {
            var tempPath = Path.GetTempPath();
            var applicationTempPath = Path.Combine(tempPath, "SoluiNet.DevTools.TimeTracking");

            EnsureDirectoryExists(applicationTempPath);
            return applicationTempPath;
        }
    }
}