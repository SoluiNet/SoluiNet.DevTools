// <copyright file="NativeMethods.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Clipboard.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides native methods for clipboard handling. (see https://stackoverflow.com/questions/15333746/clipboardinterop-content-changed-fires-twice).
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        ///     The WM_DRAWCLIPBOARD message notifies a clipboard viewer window that
        ///     the content of the clipboard has changed.
        /// </summary>
        internal const int WM_DRAWCLIPBOARD = 0x0308;

        /// <summary>
        ///     A clipboard viewer window receives the WM_CHANGECBCHAIN message when
        ///     another window is removing itself from the clipboard viewer chain.
        /// </summary>
        internal const int WM_CHANGECBCHAIN = 0x030D;

        /// <summary>
        /// Adds the specified window to clipboard viewers.
        /// </summary>
        /// <param name="hWndNewViewer">A handle to the window to be added to the clipboard chain.</param>
        /// <returns>Returns the next window in clipboard viewer chain. If error or no other windows available it will return null.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        /// <summary>
        /// Removes a specified window from the chain of clipboard viewers.
        /// </summary>
        /// <param name="hWndRemove">The handle to be removed from the chain.</param>
        /// <param name="hWndNewNext">The handle which follows the handle that should be removed</param>
        /// <returns>Returns true if successful</returns>
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        /// <summary>
        /// Sends the specified message to a window.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="msg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information (W).</param>
        /// <param name="lParam">Additional message-specific information (L).</param>
        /// <returns>Returns a result object.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        internal static extern void SetLastError(uint errorCode);

        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();
    }
}
