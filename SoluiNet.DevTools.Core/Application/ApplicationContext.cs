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

    /// <summary>
    /// Defines the application context.
    /// </summary>
    public static class ApplicationContext
    {
        static ApplicationContext()
        {
            Storage = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public static ISoluiNetApp Application { get; set; }

        /// <summary>
        /// Gets the storage.
        /// </summary>
        public static Dictionary<string, object> Storage { get; }
    }
}
