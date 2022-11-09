// <copyright file="FreeAtHomePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.BuschJaeger.FreeAtHome
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Security.Principal;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SoluiNet.DevTools.Core.Reference;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif

    /// <summary>
    /// Provides a plugin for the Busch Jaeger free@home system.
    /// </summary>
    public class FreeAtHomePlugin : ISmartHomeUiPlugin
    {
        /// <summary>
        /// Gets the first accent colour.
        /// </summary>
        public IColour AccentColour1
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public IColour AccentColour2
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public IColour ForegroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public IColour BackgroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public IColour BackgroundAccentColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName
        {
            get { return "BuschJaeger.FreeAtHome"; }
        }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        public object TypeDefinition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get { throw new NotImplementedException(); }
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
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
