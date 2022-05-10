// <copyright file="Office365ExchangePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Office365.Exchange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
#endif
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#ifdef BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif

    /// <summary>
    /// A plugin which allows to connect with exchange in an office 365 environment.
    /// </summary>
    public class Office365ExchangePlugin : IUtilitiesDevPlugin, IContainsSettings
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "Office365ExchangePlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Office 365 Exchange Tools"; }
        }

        /// <inheritdoc/>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

#if BUILD_FOR_WINDOWS
        /// <inheritdoc/>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            if (displayInPluginContainer == null)
            {
                throw new ArgumentNullException(nameof(displayInPluginContainer));
            }

            displayInPluginContainer(new Office365ExchangeUserControl());
        }
#endif
    }
}
