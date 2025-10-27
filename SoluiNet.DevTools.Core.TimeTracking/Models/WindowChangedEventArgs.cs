// <copyright file="WindowChangedEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Models
{
    using System;

    /// <summary>
    /// Event arguments for window change events.
    /// </summary>
    public class WindowChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowChangedEventArgs"/> class.
        /// </summary>
        /// <param name="previousWindow">The previous window information.</param>
        /// <param name="currentWindow">The current window information.</param>
        public WindowChangedEventArgs(WindowInfo? previousWindow, WindowInfo? currentWindow)
        {
            this.PreviousWindow = previousWindow;
            this.CurrentWindow = currentWindow;
            this.ChangedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the previous window information.
        /// </summary>
        public WindowInfo? PreviousWindow { get; }

        /// <summary>
        /// Gets the current window information.
        /// </summary>
        public WindowInfo? CurrentWindow { get; }

        /// <summary>
        /// Gets the timestamp when the window change occurred.
        /// </summary>
        public DateTime ChangedAt { get; }

        /// <summary>
        /// Gets a value indicating whether this represents a significant change.
        /// </summary>
        public bool IsSignificantChange
        {
            get
            {
                if (this.PreviousWindow == null && this.CurrentWindow != null)
                {
                    return true;
                }

                if (this.PreviousWindow != null && this.CurrentWindow == null)
                {
                    return true;
                }

                if (this.PreviousWindow == null && this.CurrentWindow == null)
                {
                    return false;
                }

                return this.PreviousWindow!.ProcessName != this.CurrentWindow!.ProcessName ||
                       this.PreviousWindow.Title != this.CurrentWindow.Title;
            }
        }
    }
}