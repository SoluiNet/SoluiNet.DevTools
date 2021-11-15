// <copyright file="SecurityTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.Security
{
    using System;
#if !COMPILED_FOR_NETSTANDARD
    using System.DirectoryServices.AccountManagement;
#endif
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using NLog;
    using SoluiNet.DevTools.Core.Windows.Application;

    /// <summary>
    /// A collection of security methods.
    /// </summary>
    public static class SecurityTools
    {
        private const int Logon32LogonInteractive = 2;

        private const int Logon32ProviderDefault = 0;

#if !COMPILED_FOR_NETSTANDARD
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

            WindowsImpersonationContext impersonationContext = null;

            if (NativeMethods.RevertToSelf())
            {
                if (NativeMethods.LogonUserW(user, domain, password, Logon32LogonInteractive, Logon32ProviderDefault, ref token))
                {
                    if (NativeMethods.DuplicateToken(token, 2, ref tokenDuplicate))
                    {
                        var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);

                        impersonationContext = Impersonate(tempWindowsIdentity, token, tokenDuplicate);
                    }
                }
            }

            if (token != IntPtr.Zero)
            {
                try
                {
                    NativeMethods.CloseHandle(token);
                }
                catch (Exception exception)
                {
                    var logger = LogManager.GetCurrentClassLogger();

                    logger.Error(exception, "Error while closing handle: {0}", Marshal.GetLastWin32Error());
                }
            }

            if (tokenDuplicate != IntPtr.Zero)
            {
                try
                {
                    NativeMethods.CloseHandle(tokenDuplicate);
                }
                catch (Exception exception)
                {
                    var logger = LogManager.GetCurrentClassLogger();

                    logger.Error(exception, "Error while closing handle: {0}", Marshal.GetLastWin32Error());
                }
            }

            return impersonationContext;
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

                foreach (var token in tokens)
                {
                    try
                    {
                        NativeMethods.CloseHandle(token);
                    }
                    catch (Exception exception)
                    {
                        var logger = LogManager.GetCurrentClassLogger();

                        logger.Error(exception, "Error while closing handle: {0}", NativeMethods.GetLastError());
                    }
                }

                return impersonationContext;
            }
            finally
            {
                impersonationIdentity.Dispose();
            }
        }
#endif

        /// <summary>
        /// Run a task under impersonated context.
        /// </summary>
        /// <param name="identity">The identity which should be used for impersonation.</param>
        /// <param name="task">The task which should be executed.</param>
        public static void RunImpersonated(WindowsIdentity identity, Action task)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

#if COMPILED_FOR_NETSTANDARD
            WindowsIdentity.RunImpersonated(identity.AccessToken, task);
#else
            var impersonationContext = identity.Impersonate();

            try
            {
                task.Invoke();
            }
            finally
            {
                impersonationContext.Dispose();
            }
#endif
        }

        /// <summary>
        /// Run a task under impersonated context.
        /// </summary>
        /// <param name="identity">The identity which should be used for impersonation.</param>
        /// <param name="task">The task which should be executed.</param>
        /// <returns>Returns the tasks result.</returns>
        public static object RunImpersonated(WindowsIdentity identity, Func<object> task)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

#if COMPILED_FOR_NETSTANDARD
            return WindowsIdentity.RunImpersonated(identity.AccessToken, task);
#else
            var impersonationContext = identity.Impersonate();

            try
            {
                return task.Invoke();
            }
            finally
            {
                impersonationContext.Dispose();
            }
#endif
        }

        /// <summary>
        /// Run a task under impersonated context.
        /// </summary>
        /// <param name="identity">The identity which should be used for impersonation.</param>
        /// <param name="task">The task which should be executed.</param>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <returns>Returns the tasks result.</returns>
        public static T RunImpersonated<T>(WindowsIdentity identity, Func<T> task)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

#if COMPILED_FOR_NETSTANDARD
            return WindowsIdentity.RunImpersonated<T>(identity.AccessToken, task);
#else
            var impersonationContext = identity.Impersonate();

            try
            {
                return task.Invoke();
            }
            finally
            {
                impersonationContext.Dispose();
            }
#endif
        }

        /// <summary>
        /// Get a WindowsIdentity-object. The returned identity must be disposed after it isn't needed anymore.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="domainName">The domain name.</param>
        /// <param name="password">The password.</param>
        /// <returns>Returns a WindowsIdentity-object. It will return the object for the logged in user if there are no parameters.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose should be called from outside scope.")]
        public static WindowsIdentity GetIdentityByName(string userName = "", string domainName = "", string password = "")
        {
            domainName = string.IsNullOrWhiteSpace(domainName) ? Environment.UserDomainName : domainName;
            userName = string.IsNullOrWhiteSpace(userName) ? Environment.UserName : userName;

#if COMPILED_FOR_NETSTANDARD
            var windowsIdentity = new WindowsLogin(userName, domainName, password);

            return windowsIdentity.Identity;
#else
            var accountName = $@"{domainName}\{userName}";

            // cannot create WindowsIdentity because it requires username in form user@domain.com but the passed value will be DOMAIN\user.
            using (var context = new PrincipalContext(ContextType.Domain, domainName))
            {
                using (var user = UserPrincipal.FindByIdentity(context, accountName))
                {
                    return user == null ? null : new WindowsIdentity(user.UserPrincipalName);
                }
            }
#endif
        }
    }
}