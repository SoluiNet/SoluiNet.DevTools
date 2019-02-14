using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SoluiNet.DevTools.Core;

namespace SoluiNet.DevTools.Utils.General
{
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
