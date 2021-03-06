﻿// <copyright file="TimeTrackingTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Utils.TimeTracking.Application;

    /// <summary>
    /// Provides a collection of methods for time tracking.
    /// </summary>
    public static class TimeTrackingTools
    {
        /// <summary>
        /// Get the title of the most foreground window.
        /// </summary>
        /// <returns>Returns the title of the most foreground window.</returns>
        public static string GetTitleOfWindowInForeground()
        {
            const int countOfCharacters = 256;
            var buffer = new StringBuilder(countOfCharacters);
            var windowInForeground = NativeMethods.GetForegroundWindow();

            return NativeMethods.GetWindowText(windowInForeground, buffer, countOfCharacters) > 0 ? buffer.ToString() : null;
        }
    }
}
