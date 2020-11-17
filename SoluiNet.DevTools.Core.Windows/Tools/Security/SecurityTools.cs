// <copyright file="SecurityTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.Security
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using SoluiNet.DevTools.Core.Windows.Application;

    /// <summary>
    /// A collection of security methods.
    /// </summary>
    public static class SecurityTools
    {
        private const int Logon32LogonInteractive = 2;

        private const int Logon32ProviderDefault = 0;

        /// <summary>
        /// Impersonate to a specific user.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <param name="password">Password of the user.</param>
        /// <param name="domain">The domain which should be used.</param>
        /// <returns>Returns an impersonation context.</returns>
        public static WindowsImpersonationContext Impersonate(string user, string password, string domain = "")
        {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            if (NativeMethods.RevertToSelf())
            {
                if (NativeMethods.LogonUserW(user, domain, password, Logon32LogonInteractive, Logon32ProviderDefault, ref token))
                {
                    if (NativeMethods.DuplicateToken(token, 2, ref tokenDuplicate))
                    {
                        var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        try
                        {
                            var impersonationContext = tempWindowsIdentity.Impersonate();

                            if (impersonationContext != null)
                            {
                                NativeMethods.CloseHandle(token);
                                NativeMethods.CloseHandle(tokenDuplicate);
                                return impersonationContext;
                            }
                        }
                        finally
                        {
                            tempWindowsIdentity.Dispose();
                        }
                    }
                }
            }

            if (token != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(token);
            }

            if (tokenDuplicate != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(tokenDuplicate);
            }

            return null;
        }
    }
}