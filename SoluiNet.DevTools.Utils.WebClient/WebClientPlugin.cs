using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SoluiNet.DevTools.Core;

namespace SoluiNet.DevTools.Utils.WebClient
{
    public class WebClientPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "WebClientPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Web Client"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new WebClientUserControl());
        }
    }
}
