// <copyright file="GitToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class GitToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "GitToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Git Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new GitUserControl());
        }
    }
}
