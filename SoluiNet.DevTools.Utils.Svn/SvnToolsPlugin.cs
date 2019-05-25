// <copyright file="SvnToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Svn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;

    public class SvnToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "SvnToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "SVN Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new SvnUserControl());
        }
    }
}
