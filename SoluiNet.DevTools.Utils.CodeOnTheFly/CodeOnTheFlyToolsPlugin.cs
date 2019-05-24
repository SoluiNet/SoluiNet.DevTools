// <copyright file="CodeOnTheFlyToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.CodeOnTheFly
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class CodeOnTheFlyToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "CodeOnTheFlyToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Code On The Fly Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new CodeOnTheFlyUserControl());
        }
    }
}
