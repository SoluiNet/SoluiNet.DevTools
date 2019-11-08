// <copyright file="Constants.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.HotKey
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A collection of constants which may be needed for hot keys.
    /// </summary>
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.NamingRules",
        "SA1310:FieldNamesMustNotContainUnderscore",
        Justification = "Use the default windows notification name.")]
    public static class Constants
    {
        // modifiers

        /// <summary>
        /// No modification.
        /// </summary>
        public const int NoMod = 0x0000;

        /// <summary>
        /// Alt-key pressed.
        /// </summary>
        public const int Alt = 0x0001;

        /// <summary>
        /// Control-key pressed.
        /// </summary>
        public const int Ctrl = 0x0002;

        /// <summary>
        /// Shift-key pressed.
        /// </summary>
        public const int Shift = 0x0004;

        /// <summary>
        /// Windows-key pressed.
        /// </summary>
        public const int Win = 0x0008;

        // windows message id for hot key

        /// <summary>
        /// Windows message ID for hot key.
        /// </summary>
        public const int WM_HOTKEY_MSG_ID = 0x0312;
    }
}
