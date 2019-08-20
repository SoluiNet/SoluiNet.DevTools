// <copyright file="Office365ExchangePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Office365.Exchange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// A plugin which allows to connect with exchange in an office 365 environment.
    /// </summary>
    public class Office365ExchangePlugin : IUtilitiesDevPlugin, IContainsSettings
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "Office365ExchangePlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Office 365 Exchange Tools"; }
        }

        /// <inheritdoc/>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new Office365ExchangeUserControl());
        }
    }
}
