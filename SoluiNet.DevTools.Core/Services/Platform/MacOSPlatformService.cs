// <copyright file="MacOSPlatformService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services.Platform
{
    using System;
    using System.IO;

    /// <summary>
    /// macOS-specific implementation of platform services.
    /// </summary>
    public class MacOSPlatformService : IPlatformService
    {
        /// <inheritdoc/>
        public string GetConfigurationDirectory()
        {
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDirectory, "Library", "Application Support", "SoluiNet.DevTools");
        }

        /// <inheritdoc/>
        public string GetApplicationDataDirectory()
        {
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDirectory, "Library", "Application Support", "SoluiNet.DevTools");
        }

        /// <inheritdoc/>
        public string GetTempDirectory()
        {
            // macOS uses /tmp, but we can also check TMPDIR environment variable
            var tmpDir = Environment.GetEnvironmentVariable("TMPDIR");
            if (!string.IsNullOrEmpty(tmpDir) && Directory.Exists(tmpDir))
            {
                return tmpDir.TrimEnd('/');
            }

            return "/tmp";
        }

        /// <inheritdoc/>
        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            // Replace backslashes with forward slashes for macOS
            return path.Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <inheritdoc/>
        public bool IsExecutableFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            // Only use Unix file modes on supported platforms
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                try
                {
                    // Check if file has execute permissions
                    var unixFileMode = File.GetUnixFileMode(filePath);

                    // Check if any execute bit is set (owner, group, or other)
                    return (unixFileMode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
                }
                catch (UnauthorizedAccessException)
                {
                    // If we can't check permissions due to access issues, fall back to checking common executable extensions
                }
                catch (PlatformNotSupportedException)
                {
                    // If Unix file modes are not supported, fall back to checking common executable extensions
                }
            }

            // Fall back to checking common executable extensions
            var extension = Path.GetExtension(filePath).ToUpperInvariant();
            return extension == ".SH" || extension == ".PY" || extension == ".PL" || extension == ".COMMAND" || string.IsNullOrEmpty(extension);
        }

        /// <inheritdoc/>
        public string GetExecutableExtension()
        {
            return string.Empty; // macOS executables typically don't have extensions
        }
    }
}