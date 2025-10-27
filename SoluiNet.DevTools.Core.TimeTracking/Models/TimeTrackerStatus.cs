// <copyright file="TimeTrackerStatus.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Models
{
    using System;

    /// <summary>
    /// Represents the status of the time tracker.
    /// </summary>
    public class TimeTrackerStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether the time tracker is running.
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the time when tracking was started.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the current monitoring interval.
        /// </summary>
        public TimeSpan MonitoringInterval { get; set; }

        /// <summary>
        /// Gets or sets the number of data points captured in the current session.
        /// </summary>
        public long DataPointsCaptured { get; set; }

        /// <summary>
        /// Gets or sets the last captured window information.
        /// </summary>
        public WindowInfo? LastCapturedWindow { get; set; }

        /// <summary>
        /// Gets or sets the platform provider being used.
        /// </summary>
        public string PlatformProvider { get; set; } = string.Empty;

        /// <summary>
        /// Gets the uptime of the current tracking session.
        /// </summary>
        public TimeSpan? Uptime => this.StartedAt.HasValue ? DateTime.UtcNow - this.StartedAt.Value : null;

        /// <summary>
        /// Returns a string representation of the time tracker status.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.IsRunning)
            {
                return $"Running (Uptime: {this.Uptime}, Data Points: {this.DataPointsCaptured})";
            }

            return "Stopped";
        }
    }
}