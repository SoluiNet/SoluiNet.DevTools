// <copyright file="LinuxPlatformService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services.Platform
{
    using System;
    using System.IO;

    /// <summary>
    /// Linux-specific implementation of platform services.
    /// </summary>
    public class LinuxPlatformService : IPlatformService
    {
        /// <inheritdoc/>
        public string GetConfigurationDirectory()
        {
            // Follow XDG Base Directory Specification
            var xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (!string.IsNullOrEmpty(xdgConfigHome))
            {
                return Path.Combine(xdgConfigHome, "soluinet-devtools");
            }

            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDirectory, ".config", "soluinet-devtools");
        }

        /// <inheritdoc/>
        public string GetApplicationDataDirectory()
        {
            // Follow XDG Base Directory Specification
            var xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (!string.IsNullOrEmpty(xdgDataHome))
            {
                return Path.Combine(xdgDataHome, "soluinet-devtools");
            }

            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDirectory, ".local", "share", "soluinet-devtools");
        }

        /// <inheritdoc/>
        public string GetTempDirectory()
        {
            // Check for XDG_RUNTIME_DIR first, then fall back to /tmp
            var xdgRuntimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
            if (!string.IsNullOrEmpty(xdgRuntimeDir) && Directory.Exists(xdgRuntimeDir))
            {
                return xdgRuntimeDir;
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

            // Replace backslashes with forward slashes for Linux
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
            return extension == ".SH" || extension == ".PY" || extension == ".PL" || string.IsNullOrEmpty(extension);
        }

        /// <inheritdoc/>
        public string GetExecutableExtension()
        {
            return string.Empty; // Linux executables typically don't have extensions
        }
    }
}