// <copyright file="IBasePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The basis for all SoluiNet.DevTools plugins.
    /// </summary>
    public interface IBasePlugin
    {
        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        string Name { get; }
    }
}
