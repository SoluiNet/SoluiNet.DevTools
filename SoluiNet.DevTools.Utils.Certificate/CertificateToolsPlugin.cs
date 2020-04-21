﻿// <copyright file="CertificateToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Certificate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// A plugin which provides utility functions for working with certificates.
    /// </summary>
    public class CertificateToolsPlugin : IUtilitiesDevPlugin
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "CertificateToolsPlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Certificate Tools"; }
        }

        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new CertificateUserControl());
        }
    }
}