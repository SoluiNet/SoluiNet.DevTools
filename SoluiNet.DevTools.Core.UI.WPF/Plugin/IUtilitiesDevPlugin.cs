// <copyright file="IUtilitiesDevPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
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

        /// <summary>
        /// The method which should be called on click.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called once the plugin will be executed.</param>
        void Execute(Action<UserControl> displayInPluginContainer);
    }
}
