// <copyright file="TfsToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class TfsToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "TfsToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "TFS Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new TfsUserControl());
        }
    }
}
