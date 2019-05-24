// <copyright file="GeneralToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.General
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class GeneralToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "GeneralToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "General Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new GeneralToolsUserControl());
        }
    }
}
