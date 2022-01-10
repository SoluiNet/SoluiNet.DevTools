// <copyright file="SafeTokenHandle.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.Security
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using Microsoft.Win32.SafeHandles;
    using SoluiNet.DevTools.Core.Windows.Application;

    /// <summary>
    /// The safe token handle.
    /// </summary>
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeTokenHandle"/> class.
        /// </summary>
        public SafeTokenHandle()
            : base(true)
        {
        }

        /// <summary>
        /// Release the handle.
        /// </summary>
        /// <returns>Returns true if handle has been released.</returns>
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(this.handle);
        }
    }
}