﻿using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Utils.MachineLearning
{
    public class MachineLearningToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "MachineLearningToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Machine Learning Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new MachineLearningUserControl());
        }
    }
}
