// <copyright file="HotKeyEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Tools.HotKey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The hot key event arguments (taken from https://www.codeproject.com/Tips/274003/Global-Hotkeys-in-WPF).
    /// </summary>
    public class HotKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the hot key.
        /// </summary>
        public GlobalHotKey HotKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyEventArgs"/> class.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        public HotKeyEventArgs(GlobalHotKey hotKey)
        {
            this.HotKey = hotKey;
        }
    }
}
