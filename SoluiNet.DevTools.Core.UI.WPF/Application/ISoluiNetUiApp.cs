// <copyright file="ISoluiNetUiApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.UIElement;

    /// <summary>
    /// Provides an interface for the SoluiNet.DevTools application with an user interface.
    /// </summary>
    public interface ISoluiNetUiApp : ISoluiNetApp
    {
        /// <summary>
        /// Gets or sets all available plugins that provide database connectivity functions.
        /// </summary>
        ICollection<IProvidesDatabaseConnectivity> SqlPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that provide utility functions.
        /// </summary>
        ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available UI elements.
        /// </summary>
        ICollection<ISoluiNetUIElement> UiElements { get; set; }
    }
}
