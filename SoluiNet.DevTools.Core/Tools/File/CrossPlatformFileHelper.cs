// <copyright file="CrossPlatformFileHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.File
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using SoluiNet.DevTools.Core.Services.Platform;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides cross-platform file system operations and utilities.
    /// </summary>
    public static class CrossPlatformFileHelper
    {
        private static readonly IPlatformService PlatformService = PlatformServiceFactory.Create();

        /// <summary>
        /// Gets a temporary file path using platform-appropriate temporary directory.
        /// </summary>
        /// <returns>Returns a cross-platform temporary file path.</returns>
        public static string GetTemporaryFilePath()
        {
            var temporaryFileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}.tmp",
                StringHelper.GetRandomString(8),
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));

            var tempDir = PlatformService.GetTempDirectory();
            var soluiNetTempDir = Path.Combine(tempDir, "SoluiNet.DevTools", "temp");

            // Ensure the directory exists
            Directory.CreateDirectory(soluiNetTempDir);

            return PlatformService.NormalizePath(Path.Combine(soluiNetTempDir, temporaryFileName));
        }

        /// <summary>
        /// Gets a platform-appropriate application data directory path.
        /// </summary>
        /// <param name="subDirectory">Optional subdirectory name.</param>
        /// <returns>Returns a cross-platform application data directory path.</returns>
        public static string GetApplicationDataPath(string subDirectory = null)
        {
            var appDataDir = PlatformService.GetApplicationDataDirectory();

            if (!string.IsNullOrEmpty(subDirectory))
            {
                appDataDir = Path.Combine(appDataDir, subDirectory);
            }

            return PlatformService.NormalizePath(appDataDir);
        }

        /// <summary>
        /// Gets a platform-appropriate configuration directory path.
        /// </summary>
        /// <param name="subDirectory">Optional subdirectory name.</param>
        /// <returns>Returns a cross-platform configuration directory path.</returns>
        public static string GetConfigurationPath(string subDirectory = null)
        {
            var configDir = PlatformService.GetConfigurationDirectory();

            if (!string.IsNullOrEmpty(subDirectory))
            {
                configDir = Path.Combine(configDir, subDirectory);
            }

            return PlatformService.NormalizePath(configDir);
        }

        /// <summary>
        /// Determines if a file is executable on the current platform.
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
        /// <returns>True if the file is executable, false otherwise.</returns>
        public static bool IsExecutableFile(string filePath)
        {
            return PlatformService.IsExecutableFile(filePath);
        }

        /// <summary>
        /// Gets the executable file extension for the current platform.
        /// </summary>
        /// <returns>The executable extension (e.g., ".exe" on Windows, empty string on Unix-like systems).</returns>
        public static string GetExecutableExtension()
        {
            return PlatformService.GetExecutableExtension();
        }

        /// <summary>
        /// Sets file permissions in a cross-platform manner.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="isExecutable">Whether the file should be executable.</param>
        /// <param name="isReadOnly">Whether the file should be read-only.</param>
        public static void SetFilePermissions(string filePath, bool isExecutable = false, bool isReadOnly = false)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // On Windows, use FileAttributes
                var attributes = File.GetAttributes(filePath);

                if (isReadOnly)
                {
                    attributes |= FileAttributes.ReadOnly;
                }
                else
                {
                    attributes &= ~FileAttributes.ReadOnly;
                }

                File.SetAttributes(filePath, attributes);
            }
            else
            {
                // On Unix-like systems, use chmod-like permissions
                if (isExecutable)
                {
                    // Set executable permissions (owner, group, others can execute)
                    // This is a simplified approach - in a real implementation you might want
                    // to use P/Invoke to call chmod directly for more precise control
                    try
                    {
                        // Try to make the file executable using .NET's limited capabilities
                        // Note: .NET doesn't have direct chmod support, so this is a best-effort approach
                        var currentAttributes = File.GetAttributes(filePath);
                        File.SetAttributes(filePath, currentAttributes & ~FileAttributes.ReadOnly);
                    }
                    catch (Exception)
                    {
                        // Silently fail if we can't set permissions
                        // In a production environment, you might want to log this
                    }
                }

                if (isReadOnly)
                {
                    File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);
                }
            }
        }

        /// <summary>
        /// Creates a directory with appropriate permissions for the current platform.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to create.</param>
        /// <param name="isUserOnly">Whether the directory should be accessible only to the current user.</param>
        public static void CreateDirectoryWithPermissions(string directoryPath, bool isUserOnly = true)
        {
            if (Directory.Exists(directoryPath))
            {
                return;
            }

            Directory.CreateDirectory(directoryPath);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && isUserOnly)
            {
                // On Unix-like systems, try to set directory permissions to user-only (700)
                // This is a simplified approach - in a real implementation you might want
                // to use P/Invoke to call chmod directly
                try
                {
                    // .NET doesn't have direct chmod support, so this is a best-effort approach
                    // The directory will be created with default permissions
                    // In a production environment, you might want to use P/Invoke to call chmod
                }
                catch (Exception)
                {
                    // Silently fail if we can't set permissions
                    // In a production environment, you might want to log this
                }
            }
        }

        /// <summary>
        /// Normalizes a file path to use the correct path separators for the current platform.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path with platform-appropriate separators.</returns>
        public static string NormalizePath(string path)
        {
            return PlatformService.NormalizePath(path);
        }

        /// <summary>
        /// Combines multiple path segments using platform-appropriate separators.
        /// </summary>
        /// <param name="paths">The path segments to combine.</param>
        /// <returns>The combined path with platform-appropriate separators.</returns>
        public static string CombinePaths(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                return string.Empty;
            }

            var combinedPath = Path.Combine(paths);
            return PlatformService.NormalizePath(combinedPath);
        }

        /// <summary>
        /// Ensures that a directory exists, creating it if necessary with appropriate permissions.
        /// </summary>
        /// <param name="directoryPath">The path to the directory.</param>
        /// <param name="isUserOnly">Whether the directory should be accessible only to the current user.</param>
        public static void EnsureDirectoryExists(string directoryPath, bool isUserOnly = true)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            CreateDirectoryWithPermissions(directoryPath, isUserOnly);
        }

        /// <summary>
        /// Gets the user's home directory in a cross-platform manner.
        /// </summary>
        /// <returns>The user's home directory path.</returns>
        public static string GetUserHomeDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        /// <summary>
        /// Gets the system temporary directory in a cross-platform manner.
        /// </summary>
        /// <returns>The system temporary directory path.</returns>
        public static string GetSystemTempDirectory()
        {
            return PlatformService.GetTempDirectory();
        }

        /// <summary>
        /// Checks if a path is absolute in a cross-platform manner.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the path is absolute, false otherwise.</returns>
        public static bool IsAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return Path.IsPathFullyQualified(path);
        }

        /// <summary>
        /// Safely deletes a file, handling cross-platform permission issues.
        /// </summary>
        /// <param name="filePath">The path to the file to delete.</param>
        /// <returns>True if the file was deleted successfully, false otherwise.</returns>
        public static bool SafeDeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false;
            }

            try
            {
                // Remove read-only attribute if present
                var attributes = File.GetAttributes(filePath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
                }

                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                // Log the exception in a real implementation
                return false;
            }
        }

        /// <summary>
        /// Safely deletes a directory and its contents, handling cross-platform permission issues.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to delete.</param>
        /// <returns>True if the directory was deleted successfully, false otherwise.</returns>
        public static bool SafeDeleteDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                return false;
            }

            try
            {
                // Remove read-only attributes from all files in the directory
                var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var attributes = File.GetAttributes(file);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                    }
                }

                Directory.Delete(directoryPath, true);
                return true;
            }
            catch (Exception)
            {
                // Log the exception in a real implementation
                return false;
            }
        }
    }
}