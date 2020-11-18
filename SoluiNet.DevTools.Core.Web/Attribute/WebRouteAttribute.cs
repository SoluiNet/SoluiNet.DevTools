// <copyright file="WebRouteAttribute.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Attribute
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
    [AttributeUsage(AttributeTargets.All)]
    public sealed class WebRouteAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebRouteAttribute"/> class.
        /// </summary>
        /// <param name="routeTypes">The route type.</param>
        /// <param name="route">The route.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "Parameters will be mapped to different properties")]
        public WebRouteAttribute(RouteTypes routeTypes, string route)
        {
            if (routeTypes == RouteTypes.ControllerAndAction)
            {
                throw new ArgumentException("Can't set up a route with route type 'ControllerAndAction' without two route parameters");
            }

            if (routeTypes.HasFlag(RouteTypes.Controller))
            {
                this.ControllerRoute = route;
            }

            if (routeTypes.HasFlag(RouteTypes.Action))
            {
                this.ActionRoute = route;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRouteAttribute"/> class.
        /// </summary>
        /// <param name="routeTypes">The route type.</param>
        /// <param name="controllerRoute">The controller route.</param>
        /// <param name="actionRoute">The action route.</param>
        public WebRouteAttribute(RouteTypes routeTypes, string controllerRoute, string actionRoute)
        {
            if (routeTypes != RouteTypes.ControllerAndAction)
            {
                throw new ArgumentException("Too many route parameters");
            }

            this.ControllerRoute = controllerRoute;
            this.ActionRoute = actionRoute;
        }

        /// <summary>
        /// Gets the controller route.
        /// </summary>
        public string ControllerRoute { get; private set; }

        /// <summary>
        /// Gets the action route.
        /// </summary>
        public string ActionRoute { get; private set; }
    }
}
