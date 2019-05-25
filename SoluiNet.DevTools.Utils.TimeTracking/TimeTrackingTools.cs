// <copyright file="TimeTrackingTools.cs" company="SoluiNet">
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

    public static class TimeTrackingTools
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetTitleOfWindowInForeground()
        {
            const int countOfCharacters = 256;
            var buffer = new StringBuilder(countOfCharacters);
            var windowInForeground = GetForegroundWindow();

            return GetWindowText(windowInForeground, buffer, countOfCharacters) > 0 ? buffer.ToString() : null;
        }
    }
}
