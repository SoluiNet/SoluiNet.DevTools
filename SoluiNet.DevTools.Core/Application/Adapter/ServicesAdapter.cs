// <copyright file="ServicesAdapter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application.Adapter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SoluiNet.DevTools.Core.Common;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Services;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// The adapter for services.
    /// </summary>
    public class ServicesAdapter
    {
        private BaseSoluiNetApp baseApp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesAdapter"/> class.
        /// </summary>
        public ServicesAdapter()
        {
            this.SetBaseApp();
        }

        /// <summary>
        /// Get service by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns an object that implements the <see cref="ISoluiNetService"/> interface.</returns>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if passed key is null or empty.</exception>
        public ISoluiNetService this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return this.baseApp.Services.FirstOrDefault(x => x.Name == key);
            }
        }

        /// <summary>
        /// Set the base app field.
        /// </summary>
        private void SetBaseApp()
        {
            if (this.baseApp == null)
            {
                this.baseApp = ApplicationContext.Application is BaseSoluiNetApp
                    ? ApplicationContext.Application as BaseSoluiNetApp
                    : (ApplicationContext.Application as IHoldsBaseApp)?.BaseApp;
            }
        }
    }
}
