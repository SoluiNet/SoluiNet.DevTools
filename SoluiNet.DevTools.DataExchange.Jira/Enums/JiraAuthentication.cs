// <copyright file="JiraAuthentication.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.DataExchange.Jira.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An enumeration about possible JIRA authentication methods.
    /// </summary>
    public enum JiraAuthentication
    {
        /// <summary>
        /// Basic authentication
        /// </summary>
        BasicAuthentication,

        /// <summary>
        /// JWT authentication
        /// </summary>
        JwtAuthentication,
    }
}
