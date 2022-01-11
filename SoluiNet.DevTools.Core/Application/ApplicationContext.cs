// <copyright file="ApplicationContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Application.Adapter;

    /// <summary>
    /// Defines the application context.
    /// </summary>
    public static class ApplicationContext
    {
        static ApplicationContext()
        {
            SessionValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public static ISoluiNetApp Application { get; set; }

        /// <summary>
        /// Gets the session values.
        /// </summary>
        public static Dictionary<string, object> SessionValues { get; }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public static ApplicationConfigurationAdapter Configuration { get; }
    }
}
