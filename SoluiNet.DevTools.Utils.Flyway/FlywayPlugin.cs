using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Utils.Flyway
{
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
