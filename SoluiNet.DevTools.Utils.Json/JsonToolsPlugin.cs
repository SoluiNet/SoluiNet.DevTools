// <copyright file="JsonToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Json
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
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
#endif
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif

    /// <summary>
    /// A plugin which provides utility functions for working with JSON objects.
    /// </summary>
    public class JsonToolsPlugin : IUtilitiesDevPlugin
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "JsonToolsPlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "JSON Tools"; }
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

            displayInPluginContainer(new JsonUserControl());
        }
#endif
    }
}
