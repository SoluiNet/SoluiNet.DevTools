// <copyright file="IUtilitiesDevPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Blazor.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// Provides an interface for plugins which provides utility functions.
    /// </summary>
    public interface IUtilitiesDevPlugin : IBasePlugin
    {
        /// <summary>
        /// Gets the label which should be used for menu items.
        /// </summary>
        string MenuItemLabel { get; }
    }
}
