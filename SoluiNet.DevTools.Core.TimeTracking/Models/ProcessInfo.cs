// <copyright file="ProcessInfo.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents information about a process.
    /// </summary>
    public class ProcessInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        public ProcessInfo()
        {
            this.PlatformSpecificData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the process ID.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full path to the process executable.
        /// </summary>
        public string ProcessPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the command line arguments.
        /// </summary>
        public string CommandLine { get; set; } = string.Empty;

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
        /// Gets a value indicating whether this process information is valid.
        /// </summary>
        public bool IsValid => this.ProcessId > 0 && !string.IsNullOrWhiteSpace(this.ProcessName);

        /// <summary>
        /// Creates a copy of this ProcessInfo instance.
        /// </summary>
        /// <returns>A new ProcessInfo instance with the same values.</returns>
        public ProcessInfo Clone()
        {
            return new ProcessInfo
            {
                ProcessId = this.ProcessId,
                ProcessName = this.ProcessName,
                ProcessPath = this.ProcessPath,
                CommandLine = this.CommandLine,
                Platform = this.Platform,
                CapturedAt = this.CapturedAt,
                PlatformSpecificData = new Dictionary<string, object>(this.PlatformSpecificData),
            };
        }

        /// <summary>
        /// Returns a string representation of this process information.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"{this.ProcessName} (PID: {this.ProcessId})";
        }
    }
}