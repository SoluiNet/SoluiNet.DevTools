﻿// <copyright file="ConfigurationScope.cs" company="SoluiNet">
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
    /// The configuration scope.
    /// </summary>
    public enum ConfigurationScope
    {
        /// <summary>
        /// No scope
        /// </summary>
        None = 0,

        /// <summary>
        /// A solitary relationless configuration entry.
        /// </summary>
        Solitary = 1,

        /// <summary>
        /// A configuration entry which is related to a group.
        /// </summary>
        Grouped = 5,

        /// <summary>
        /// A configuration entry which is related to a plugin.
        /// </summary>
        PerPlugin = 10,

        /// <summary>
        /// A configuration entry which is related to the installation.
        /// </summary>
        PerInstallation = 20,

        /// <summary>
        /// A configuration entry which is related to the system.
        /// </summary>
        PerSystem = 30,

        /// <summary>
        /// A configuration entry for a general purpose.
        /// </summary>
        General = 100,

        /// <summary>
        /// A default value for not defined configuration entries.
        /// </summary>
        Default = 999,
    }
}
