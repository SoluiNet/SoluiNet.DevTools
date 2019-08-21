// <copyright file="SecurityTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.Security
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

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

            if (RevertToSelf())
            {
                if (LogonUserA(user, domain, password, Logon32LogonInteractive, Logon32ProviderDefault, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        var impersonationContext = tempWindowsIdentity.Impersonate();

                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return impersonationContext;
                        }
                    }
                }
            }

            if (token != IntPtr.Zero)
            {
                CloseHandle(token);
            }

            if (tokenDuplicate != IntPtr.Zero)
            {
                CloseHandle(tokenDuplicate);
            }

            return null;
        }

        [DllImport("advapi32.dll")]
        private static extern int LogonUserA(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);
    }
}