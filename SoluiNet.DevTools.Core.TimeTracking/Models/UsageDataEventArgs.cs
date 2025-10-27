// <copyright file="UsageDataEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Models
{
    using System;

    /// <summary>
    /// Event arguments for usage data capture events.
    /// </summary>
    public class UsageDataEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageDataEventArgs"/> class.
        /// </summary>
        /// <param name="windowInfo">The captured window information.</param>
        /// <param name="duration">The duration for which the window was active.</param>
        public UsageDataEventArgs(WindowInfo windowInfo, TimeSpan duration)
        {
            this.WindowInfo = windowInfo ?? throw new ArgumentNullException(nameof(windowInfo));
            this.Duration = duration;
            this.CapturedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the captured window information.
        /// </summary>
        public WindowInfo WindowInfo { get; }

        /// <summary>
        /// Gets the duration for which the window was active.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets the timestamp when this data was captured.
        /// </summary>
        public DateTime CapturedAt { get; }
    }
}