// <copyright file="ISoluiNetApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.UIElement;

    /// <summary>
    /// Provides an interface for the SoluiNet.DevTools application.
    /// </summary>
    public interface ISoluiNetApp
    {
        /// <summary>
        /// Gets or sets all available plugins.
        /// </summary>
        ICollection<IBasePlugin> Plugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that provide database connectivity functions.
        /// </summary>
        ICollection<ISqlDevPlugin> SqlPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that provide utility functions.
        /// </summary>
        ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that will run in the background.
        /// </summary>
        ICollection<IPluginWithBackgroundTask> BackgroundTaskPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available UI elements.
        /// </summary>
        ICollection<ISoluiNetUIElement> UiElements { get; set; }
    }
}
