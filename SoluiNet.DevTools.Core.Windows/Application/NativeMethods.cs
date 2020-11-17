﻿// <copyright file="NativeMethods.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Application
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
        /// Log a user on to the local computer.
        /// </summary>
        /// <param name="lpszUserName">The user name.</param>
        /// <param name="lpszDomain">The domain.</param>
        /// <param name="lpszPassword">The password.</param>
        /// <param name="dwLogonType">The logon type.</param>
        /// <param name="dwLogonProvider">The logon provider.</param>
        /// <param name="phToken">A pointer to handle variable that receives a handle to a token that represents the specified user.</param>
        /// <returns>Returns true if successful.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LogonUserW(
            [MarshalAs(UnmanagedType.LPWStr)]string lpszUserName,
            [MarshalAs(UnmanagedType.LPWStr)]string lpszDomain,
            [MarshalAs(UnmanagedType.LPWStr)]string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        /// <summary>
        /// Create a new access token from existing one.
        /// </summary>
        /// <param name="hToken">A handle to an access token.</param>
        /// <param name="impersonationLevel">Specifies the impersonation level of the new token.</param>
        /// <param name="hNewToken">A pointer to the variable that receives the new token.</param>
        /// <returns>Returns true if successfully duplicated.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DuplicateToken(
            IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        /// <summary>
        /// Terminates the impersonation of an application.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RevertToSelf();

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        /// <returns>Returns true if successful.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);
    }
}