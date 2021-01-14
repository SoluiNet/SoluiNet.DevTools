// <copyright file="SecurityTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System.Security.Cryptography.X509Certificates;

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

                        Impersonate(tempWindowsIdentity, token, tokenDuplicate);
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

        /// <summary>
        /// Impersonate to a specific identity.
        /// </summary>
        /// <param name="impersonationIdentity">The impersonation identity.</param>
        /// <param name="tokens">The tokens.</param>
        /// <returns>Returns an impersonation context.</returns>
        public static WindowsImpersonationContext Impersonate(WindowsIdentity impersonationIdentity, params IntPtr[] tokens)
        {
            try
            {
                var impersonationContext = impersonationIdentity.Impersonate();

                if (impersonationContext != null)
                {
                    foreach (var token in tokens)
                    {
                        NativeMethods.CloseHandle(token);
                    }

                    return impersonationContext;
                }
            }
            finally
            {
                impersonationIdentity.Dispose();
            }

            return null;
        }
    }
}