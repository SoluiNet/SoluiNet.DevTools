using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Utils.Tfs
{
    public class TfsToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "TfsToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "TFS Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new TfsUserControl());
        }
    }
}
