// <copyright file="WebRouteAttribute.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Attribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Enums;

    /// <summary>
    /// The web route attribute.
    /// </summary>
    public class WebRouteAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebRouteAttribute"/> class.
        /// </summary>
        /// <param name="routeType">The route type.</param>
        /// <param name="route">The route.</param>
        public WebRouteAttribute(RouteTypeEnum routeType, string route)
        {
            if (routeType == RouteTypeEnum.ControllerAndAction)
            {
                throw new ArgumentException("Can't set up a route with route type 'ControllerAndAction' without two route parameters");
            }

            if (routeType.HasFlag(RouteTypeEnum.Controller))
            {
                this.ControllerRoute = route;
            }

            if (routeType.HasFlag(RouteTypeEnum.Action))
            {
                this.ActionRoute = route;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRouteAttribute"/> class.
        /// </summary>
        /// <param name="routeType">The route type.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <param name="actionRoute">The action route.</param>
        public WebRouteAttribute(RouteTypeEnum routeType, string controllerRoute, string actionRoute)
        {
            if (routeType != RouteTypeEnum.ControllerAndAction)
            {
                throw new ArgumentException("Too many route parameters");
            }

            this.ControllerRoute = controllerRoute;
            this.ActionRoute = actionRoute;
        }

        /// <summary>
        /// Gets or sets the controller route.
        /// </summary>
        public string ControllerRoute { get; set; }

        /// <summary>
        /// Gets or sets the action route.
        /// </summary>
        public string ActionRoute { get; set; }
    }
}
