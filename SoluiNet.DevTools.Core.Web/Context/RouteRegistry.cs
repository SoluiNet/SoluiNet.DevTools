// <copyright file="RouteRegistry.cs" company="SoluiNet">
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
    /// The route registry.
    /// </summary>
    public class RouteRegistry : List<WebRoute>
    {
        /// <summary>
        /// Gets or sets the route for the overgiven name.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="actionName">The action name.</param>
        /// <returns>Returns the route for the overgiven name.</returns>
        public WebRoute this[string controllerName, string actionName]
        {
            get
            {
                return this.Where(x => x.Controller == controllerName && x.Action == actionName).SingleOrDefault();
            }

            set
            {
                var index = this.IndexOf(this.Where(x => x.Controller == controllerName && x.Action == actionName).SingleOrDefault());

                this[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the route for the overgiven name.
        /// </summary>
        /// <param name="routeName">The route name.</param>
        /// <returns>Returns the route for the overgiven name.</returns>
        public WebRoute this[string routeName]
        {
            get
            {
                if (routeName.StartsWith("/"))
                {
                    routeName = routeName.Remove(0, 1);
                }

                var controllerName = routeName.Split('/')[0];
                var actionName = routeName.Split('/')[1];

                return this[controllerName, actionName];
            }

            set
            {
                if (routeName.StartsWith("/"))
                {
                    routeName = routeName.Remove(0, 1);
                }

                var controllerName = routeName.Split('/')[0];
                var actionName = routeName.Split('/')[1];

                this[controllerName, actionName] = value;
            }
        }
    }
}
