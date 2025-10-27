// <copyright file="PlatformInfoLayoutRenderer.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using NLog;
    using NLog.LayoutRenderers;

    /// <summary>
    /// Custom NLog layout renderer that provides platform information for log entries.
    /// </summary>
    [LayoutRenderer("platform-info")]
    public class PlatformInfoLayoutRenderer : LayoutRenderer
    {
        private static readonly string CachedPlatformInfo = GeneratePlatformInfo();

        /// <summary>
        /// Gets or sets a value indicating whether to include detailed platform information.
        /// </summary>
        public bool Detailed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include architecture information.
        /// </summary>
        public bool IncludeArchitecture { get; set; } = true;

        /// <summary>
        /// Renders the platform information.
        /// </summary>
        /// <param name="builder">The string builder to append the result to.</param>
        /// <param name="logEvent">The log event being processed.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (this.Detailed)
            {
                builder.Append(this.GetDetailedPlatformInfo());
            }
            else
            {
                builder.Append(CachedPlatformInfo);
            }
        }

        /// <summary>
        /// Generates the basic platform information string.
        /// </summary>
        /// <returns>The platform information string.</returns>
        private static string GeneratePlatformInfo()
        {
            var platformName = GetPlatformName();
            var architecture = RuntimeInformation.ProcessArchitecture.ToString();

            return $"{platformName}/{architecture}";
        }

        /// <summary>
        /// Gets detailed platform information.
        /// </summary>
        /// <returns>Detailed platform information string.</returns>
        private string GetDetailedPlatformInfo()
        {
            var sb = new StringBuilder();

            sb.Append(GetPlatformName());

            if (this.IncludeArchitecture)
            {
                sb.Append('/');
                sb.Append(RuntimeInformation.ProcessArchitecture.ToString());
            }

            if (this.Detailed)
            {
                sb.Append(" | ");
                sb.Append(RuntimeInformation.FrameworkDescription);
                sb.Append(" | ");
                sb.Append(Environment.OSVersion.VersionString);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the platform name.
        /// </summary>
        /// <returns>The platform name.</returns>
        private static string GetPlatformName()
        {
            if (OperatingSystem.IsWindows())
            {
                return "Win";
            }

            if (OperatingSystem.IsLinux())
            {
                return "Linux";
            }

            if (OperatingSystem.IsMacOS())
            {
                return "macOS";
            }

            return "Unknown";
        }
    }
}