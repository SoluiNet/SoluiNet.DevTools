// <copyright file="IManagementUiPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// Provides the interface for a plugin which will provide UI elements for Management functions.
    /// </summary>
    public interface IManagementUiPlugin : IProvidesDatabaseConnectivity, IThemable
    {
        /// <summary>
        /// Method which should be executed to display the plugin in an WPF application.
        /// </summary>
        /// <param name="mainGrid">The grid in which the plugin should be displayed.</param>
        void Display(Grid mainGrid);
    }
}
