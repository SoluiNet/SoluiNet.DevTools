// <copyright file="FileToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.File
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class FileToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "FileToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "File Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new FileToolsUserControl());
        }
    }
}
