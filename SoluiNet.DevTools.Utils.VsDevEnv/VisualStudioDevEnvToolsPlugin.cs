// <copyright file="VisualStudioDevEnvToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.VsDevEnv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class VisualStudioDevEnvToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "VisualStudioDevEnvToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Visual Studio DevEnv Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new VsDevEnvToolsUserControl());
        }
    }
}
