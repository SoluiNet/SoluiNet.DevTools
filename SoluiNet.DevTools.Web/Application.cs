// <copyright file="Application.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.Web;
    using SoluiNet.DevTools.Core.Web.Application;
    using SoluiNet.DevTools.Core.Web.Plugin;

    /// <summary>
    /// Provides a collection of methods for the application.
    /// </summary>
    public static class Application
    {
        static Application()
        {
            App = new WebApplication();
        }

        /// <summary>
        /// Gets or sets the web application.
        /// </summary>
        public static WebApplication App { get; set; }
    }
}
