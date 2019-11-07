// <copyright file="HotKeyAlreadyRegisteredException.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Tools.HotKey
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception if the hot key has already been registered (taken from https://www.codeproject.com/Tips/274003/Global-Hotkeys-in-WPF).
    /// </summary>
    [Serializable]
    public class HotKeyAlreadyRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyAlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="hotKey">The hot key.</param>
        public HotKeyAlreadyRegisteredException(string message, GlobalHotKey hotKey)
            : base(message)
        {
            this.HotKey = hotKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyAlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="hotKey">The hot key.</param>
        /// <param name="innerException">The inner exception.</param>
        public HotKeyAlreadyRegisteredException(string message, GlobalHotKey hotKey, Exception innerException)
            : base(message, innerException)
        {
            this.HotKey = hotKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyAlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected HotKeyAlreadyRegisteredException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            // do nothing
        }

        /// <summary>
        /// Gets the hot key.
        /// </summary>
        public GlobalHotKey HotKey { get; private set; }
    }
}
