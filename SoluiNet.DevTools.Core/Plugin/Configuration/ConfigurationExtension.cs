// <copyright file="ConfigurationExtension.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Enums;

    /// <summary>
    /// The plugin entry type.
    /// </summary>
    public partial class SoluiNetPluginEntryType
    {
        /// <summary>
        /// Gets or sets the configuration scope.
        /// </summary>
        public ConfigurationScopeEnum Scope { get; set; }
    }
}
