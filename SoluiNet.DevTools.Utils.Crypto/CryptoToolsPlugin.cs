// <copyright file="CryptoToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class CryptoToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "CryptoToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Crypto Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new CryptToolsUserControl());
        }
    }
}
