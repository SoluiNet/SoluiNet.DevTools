// <copyright file="PluginMetadata.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.CrossPlatform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents metadata information for a plugin.
    /// </summary>
    public class PluginMetadata : IPluginMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginMetadata"/> class.
        /// </summary>
        /// <param name="name">The plugin name.</param>
        /// <param name="version">The plugin version.</param>
        /// <param name="supportedPlatforms">The supported platforms.</param>
        /// <param name="supportedArchitectures">The supported architectures.</param>
        /// <param name="dependencies">The plugin dependencies.</param>
        /// <param name="requiresElevation">Whether the plugin requires elevation.</param>
        /// <param name="minimumDotNetVersion">The minimum .NET version required.</param>
        public PluginMetadata(
            string name,
            string version,
            IEnumerable<string> supportedPlatforms = null,
            IEnumerable<string> supportedArchitectures = null,
            IEnumerable<string> dependencies = null,
            bool requiresElevation = false,
            Version minimumDotNetVersion = null)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Version = version ?? throw new ArgumentNullException(nameof(version));
            this.SupportedPlatforms = supportedPlatforms?.ToList() ?? new List<string> { "Windows", "Linux", "macOS" };
            this.SupportedArchitectures = supportedArchitectures?.ToList() ?? new List<string> { "x64", "arm64" };
            this.Dependencies = dependencies?.ToList() ?? new List<string>();
            this.RequiresElevation = requiresElevation;
            this.MinimumDotNetVersion = minimumDotNetVersion ?? new Version(6, 0);
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Version { get; }

        /// <inheritdoc />
        public IEnumerable<string> SupportedPlatforms { get; }

        /// <inheritdoc />
        public IEnumerable<string> SupportedArchitectures { get; }

        /// <inheritdoc />
        public IEnumerable<string> Dependencies { get; }

        /// <inheritdoc />
        public bool RequiresElevation { get; }

        /// <inheritdoc />
        public Version MinimumDotNetVersion { get; }
    }
}