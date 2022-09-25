﻿// <copyright file="ScanImagePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.ScanImage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif

    /// <summary>
    /// A plugin which provides utility functions to scan and extract data from an image.
    /// </summary>
    public class ScanImagePlugin : IUtilitiesDevPlugin
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "ScanImagePlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Scan Image"; }
        }

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get
            {
                throw new NotImplementedException();
            }
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
        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            if (displayInPluginContainer == null)
            {
                throw new ArgumentNullException(nameof(displayInPluginContainer));
            }

            displayInPluginContainer(new ScanImageUserControl());
        }
#endif
    }
}
