// <copyright file="FlywayPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Flyway
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class FlywayPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "FlywayToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Flyway Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new FlywayUserControl());
        }
    }
}
