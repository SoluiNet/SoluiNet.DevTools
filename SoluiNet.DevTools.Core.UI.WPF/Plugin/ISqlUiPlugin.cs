// <copyright file="ISqlUiPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Provides the interface for a plugin which will provide UI elements for SQL functions.
    /// </summary>
    public interface ISqlUiPlugin : IProvidesDatabaseConnectivity, IThemable
    {
        /// <summary>
        /// Method which should be executed to display the plugin in an WPF application.
        /// </summary>
        /// <param name="mainGrid">The grid in which the plugin should be displayed.</param>
        void Display(Grid mainGrid);
    }
}
