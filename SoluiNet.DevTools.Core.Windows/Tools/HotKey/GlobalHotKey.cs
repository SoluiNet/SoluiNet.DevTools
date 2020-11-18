// <copyright file="GlobalHotKey.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.HotKey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using SoluiNet.DevTools.Core.Windows.Application;

    /// <summary>
    /// Provides a class to set up a global hot key. (See also https://www.dreamincode.net/forums/topic/180436-global-hotkeys/).
    /// </summary>
    public class GlobalHotKey
    {
        private readonly int modifier;
        private readonly int key;
        private readonly IntPtr hWnd;
        private readonly int id;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalHotKey"/> class.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="key">The key.</param>
        /// <param name="windowHandle">The window handle.</param>
        public GlobalHotKey(int modifier, Keys key, IntPtr windowHandle)
        {
            this.modifier = modifier;
            this.key = (int)key;
            this.hWnd = windowHandle;
            this.id = this.GetHashCode();
        }

        /// <summary>
        /// Get the hash code of the hot key.
        /// </summary>
        /// <returns>Returns the hash code of the hot key.</returns>
        public sealed override int GetHashCode()
        {
            return this.modifier ^ this.key ^ this.hWnd.ToInt32();
        }

        /// <summary>
        /// Register the hot key.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        public bool Register()
        {
            return NativeMethods.RegisterHotKey(this.hWnd, this.id, this.modifier, this.key);
        }

        /// <summary>
        /// Unregister the hot key.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        public bool Unregister()
        {
            return NativeMethods.UnregisterHotKey(this.hWnd, this.id);
        }
    }
}
