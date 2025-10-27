// <copyright file="WindowInfo.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents information about a window.
    /// </summary>
    public class WindowInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowInfo"/> class.
        /// </summary>
        public WindowInfo()
        {
            this.PlatformSpecificData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the process ID.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process path.
        /// </summary>
        public string ProcessPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the window class name (platform-specific).
        /// </summary>
        public string WindowClass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the platform name where this information was captured.
        /// </summary>
        public string Platform { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when this information was captured.
        /// </summary>
        public DateTime CapturedAt { get; set; }

        /// <summary>
        /// Gets or sets platform-specific additional data.
        /// </summary>
        public Dictionary<string, object> PlatformSpecificData { get; set; }

        /// <summary>
        /// Gets a value indicating whether this window information is valid.
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(this.Title) && !string.IsNullOrWhiteSpace(this.ProcessName);

        /// <summary>
        /// Creates a copy of this WindowInfo instance.
        /// </summary>
        /// <returns>A new WindowInfo instance with the same values.</returns>
        public WindowInfo Clone()
        {
            return new WindowInfo
            {
                Title = this.Title,
                ProcessName = this.ProcessName,
                ProcessId = this.ProcessId,
                ProcessPath = this.ProcessPath,
                WindowClass = this.WindowClass,
                Platform = this.Platform,
                CapturedAt = this.CapturedAt,
                PlatformSpecificData = new Dictionary<string, object>(this.PlatformSpecificData),
            };
        }

        /// <summary>
        /// Returns a string representation of this window information.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"{this.ProcessName} - {this.Title}";
        }
    }
}