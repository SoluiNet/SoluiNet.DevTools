// <copyright file="ClipboardNativeMethods.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Clipboard
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
    public static class ClipboardNativeMethods
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint errorCode);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }
}
