// <copyright file="RouteTypeEnum.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An enumeration of route types.
    /// </summary>
    [Flags]
    public enum RouteTypes
    {
        /// <summary>
        /// A route which is based on controller.
        /// </summary>
        Controller = 1,

        /// <summary>
        /// A route which is based on action.
        /// </summary>
        Action = 2,

        /// <summary>
        /// A route which is based on controller and action.
        /// </summary>
        ControllerAndAction = 3,
    }
}
