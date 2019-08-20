﻿// <copyright file="ISoluiNetUiApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.Application;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// Provides an interface for the SoluiNet.DevTools WPF application.
    /// </summary>
    public interface ISoluiNetUiWpfApp : ISoluiNetUiApp
    {
        /// <summary>
        /// Gets or sets all available plugins that provide database connectivity functions.
        /// </summary>
        ICollection<ISqlUiPlugin> SqlPlugins { get; set; }

        /// <summary>
        /// Gets or sets all available plugins that provide utility functions.
        /// </summary>
        ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; set; }
    }
}