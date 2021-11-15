// <copyright file="WebRoute.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The web route.
    /// </summary>
    public class WebRoute
    {
        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public ICollection<WebParameter> Parameters { get; set; }
    }
}
