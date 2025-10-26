// <copyright file="IPluginMetadata.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents metadata information for a plugin.
    /// </summary>
    public interface IPluginMetadata
    {
        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the supported platforms for this plugin.
        /// </summary>
        IEnumerable<string> SupportedPlatforms { get; }

        /// <summary>
        /// Gets the supported architectures for this plugin.
        /// </summary>
        IEnumerable<string> SupportedArchitectures { get; }

        /// <summary>
        /// Gets the plugin dependencies.
        /// </summary>
        IEnumerable<string> Dependencies { get; }

        /// <summary>
        /// Gets a value indicating whether the plugin requires elevation.
        /// </summary>
        bool RequiresElevation { get; }

        /// <summary>
        /// Gets the minimum .NET version required by this plugin.
        /// </summary>
        Version MinimumDotNetVersion { get; }
    }
}