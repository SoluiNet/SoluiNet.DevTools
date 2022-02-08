// <copyright file="BlazorPluginBase.cs" company="SoluiNet">
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

    /// <summary>
    /// The base class for blazor plugins.
    /// </summary>
    public abstract class BlazorPluginBase : IBlazorPlugin
    {
        /// <inheritdoc />
        public abstract Dictionary<string, ICollection<object>> Resources { get; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract void Configure(IApplicationBuilder app, IWebHostEnvironment environment);

        /// <inheritdoc />
        public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
