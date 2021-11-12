// <copyright file="WindowsLogin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Principal;
    using SoluiNet.DevTools.Core.Windows.Application;

    /// <summary>
    /// Provides a Windows Identity for .NET Core and .NET Standard.
    ///
    /// See also: https://stackoverflow.com/questions/46529121/windowsimpersonationcontext-impersonate-not-found-in-asp-core.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Deviate from naming convention to improve readability.")]
    public class WindowsLogin : IDisposable
    {
        /// <summary>
        /// Constant for default login.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Use underscores for better readability")]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Use underscores for better readability")]
        protected const int LOGON32_PROVIDER_DEFAULT = 0;

        /// <summary>
        /// Constant for interactive login.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Use underscores for better readability")]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Use underscores for better readability")]
        protected const int LOGON32_LOGON_INTERACTIVE = 2;

        private System.IntPtr accessToken;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsLogin"/> class.
        /// </summary>
        public WindowsLogin()
        {
            this.Identity = WindowsIdentity.GetCurrent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsLogin"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password">The password.</param>
        public WindowsLogin(string username, string domain, string password)
        {
            this.Login(username, domain, password);
        }

        /// <summary>
        /// Gets or sets the Windows Identity.
        /// </summary>
        public WindowsIdentity Identity { get; set; }

        /// <summary>
        /// Try to login with passed credentials.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="password">The password.</param>
        public void Login(string username, string domain, string password)
        {
            if (this.Identity != null)
            {
                this.Identity.Dispose();
                this.Identity = null;
            }

            try
            {
                this.accessToken = new System.IntPtr(0);
                this.Logout();

                this.accessToken = System.IntPtr.Zero;
                var logonSuccessful = NativeMethods.LogonUserW(
                   username,
                   domain,
                   password,
                   LOGON32_LOGON_INTERACTIVE,
                   LOGON32_PROVIDER_DEFAULT,
                   ref this.accessToken);

                if (!logonSuccessful)
                {
                    int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(error);
                }

                this.Identity = new WindowsIdentity(this.accessToken);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Try to log out.
        /// </summary>
        public void Logout()
        {
            if (this.accessToken != System.IntPtr.Zero)
            {
                NativeMethods.CloseHandle(this.accessToken);
            }

            this.accessToken = System.IntPtr.Zero;

            if (this.Identity != null)
            {
                this.Identity.Dispose();
                this.Identity = null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// <param name="disposing">Set to true if object is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.Logout();

            this.disposed = true;
        }
    }
}
