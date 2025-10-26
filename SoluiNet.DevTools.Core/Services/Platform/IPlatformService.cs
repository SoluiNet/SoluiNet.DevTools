// <copyright file="IPlatformService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services.Platform
{
    /// <summary>
    /// Interface defining platform-specific operations for cross-platform compatibility.
    /// </summary>
    public interface IPlatformService
    {
        /// <summary>
        /// Gets the platform-appropriate configuration directory path.
        /// </summary>
        /// <returns>The configuration directory path for the current platform.</returns>
        string GetConfigurationDirectory();

        /// <summary>
        /// Gets the platform-appropriate application data directory path.
        /// </summary>
        /// <returns>The application data directory path for the current platform.</returns>
        string GetApplicationDataDirectory();

        /// <summary>
        /// Gets the platform-appropriate temporary directory path.
        /// </summary>
        /// <returns>The temporary directory path for the current platform.</returns>
        string GetTempDirectory();

        /// <summary>
        /// Normalizes a file path to use the correct path separators for the current platform.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The normalized path with platform-appropriate separators.</returns>
        string NormalizePath(string path);

        /// <summary>
        /// Determines if a file is executable on the current platform.
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
        /// <returns>True if the file is executable, false otherwise.</returns>
        bool IsExecutableFile(string filePath);

        /// <summary>
        /// Gets the executable file extension for the current platform.
        /// </summary>
        /// <returns>The executable extension (e.g., ".exe" on Windows, empty string on Unix-like systems).</returns>
        string GetExecutableExtension();
    }
}