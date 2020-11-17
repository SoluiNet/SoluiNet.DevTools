// <copyright file="NativeMethods.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provide a wrapper for native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Register the HotKey.
        /// </summary>
        /// <param name="hwnd">The handle to the window that will receive hot key messages.</param>
        /// <param name="id">The ID of the hot key.</param>
        /// <param name="modifiers">The modifiers which can be used for the hot key (i.e. Shift or Control).</param>
        /// <param name="key">The virtual-key code of the hot key.</param>
        /// <returns>Returns true if hot key is registered.</returns>
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RegisterHotKey(IntPtr hwnd, int id, int modifiers, int key);

        /// <summary>
        /// Unregister the HotKey.
        /// </summary>
        /// <param name="hwnd">The handle to the window that is associated to the hot key messages.</param>
        /// <param name="id">The ID of the hot key.</param>
        /// <returns>Returns true if hot key is unregistered.</returns>
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern int UnregisterHotKey(IntPtr hwnd, int id);
    }
}
