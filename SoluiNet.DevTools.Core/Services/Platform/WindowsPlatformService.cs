// <copyright file="WindowsPlatformService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services.Platform
{
    using System;
    using System.IO;

    /// <summary>
    /// Windows-specific implementation of platform services.
    /// </summary>
    public class WindowsPlatformService : IPlatformService
    {
        /// <inheritdoc/>
        public string GetConfigurationDirectory()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, "SoluiNet", "DevTools");
        }

        /// <inheritdoc/>
        public string GetApplicationDataDirectory()
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppDataPath, "SoluiNet", "DevTools");
        }

        /// <inheritdoc/>
        public string GetTempDirectory()
        {
            return Path.GetTempPath();
        }

        /// <inheritdoc/>
        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            // Replace forward slashes with backslashes for Windows
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        /// <inheritdoc/>
        public bool IsExecutableFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            var extension = Path.GetExtension(filePath).ToUpperInvariant();
            return extension == ".EXE" || extension == ".BAT" || extension == ".CMD" || extension == ".COM";
        }

        /// <inheritdoc/>
        public string GetExecutableExtension()
        {
            return ".exe";
        }
    }
}