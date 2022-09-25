// <copyright file="IBlazorPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Blazor.Plugin
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
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// Provides an interface for blazor plugins.
    /// </summary>
    /// <remarks>
    /// See also: http://codegator.com/blazor-plugins-part-1/.
    /// </remarks>
    public interface IBlazorPlugin : IBasePlugin, IHoldsResources
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Configure the application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="environment">The environment.</param>
        void Configure(IApplicationBuilder app, IWebHostEnvironment environment);
    }
}
