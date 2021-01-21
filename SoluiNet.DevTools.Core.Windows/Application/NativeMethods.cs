// <copyright file="NativeMethods.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
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
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Retrieves the calling thread's last-error code value. The last-error code is maintained on a per-thread basis. Multiple threads do not overwrite each other's last-error code.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.SysUInt)]
        internal static extern bool GetLastError();

        /// <summary>
        /// Register the HotKey.
        /// </summary>
        /// <param name="hWnd">The handle to the window that will receive hot key messages.</param>
        /// <param name="id">The ID of the hot key.</param>
        /// <param name="modifiers">The modifiers which can be used for the hot key (i.e. Shift or Control).</param>
        /// <param name="vkey">The virtual-key code of the hot key.</param>
        /// <returns>Returns true if hot key is registered.</returns>
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, int vkey);

        /// <summary>
        /// Unregister the HotKey.
        /// </summary>
        /// <param name="hWnd">The handle to the window that is associated to the hot key messages.</param>
        /// <param name="id">The ID of the hot key.</param>
        /// <returns>Returns true if hot key is unregistered.</returns>
        [DllImport("user32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
