// <copyright file="NativeMethods.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a wrapper for native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Retrieves a handle to the foreground window.
        /// </summary>
        /// <returns>Returns a handle to the foreground window. Can be null in certain circumstances.</returns>
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <param name="text">The buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a null character.</param>
        /// <param name="count">The maximum number of characters to copy to the buffer, including the null character. If the text exceeds this limit, it is truncated.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character. Otherwise it will return zero.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}
