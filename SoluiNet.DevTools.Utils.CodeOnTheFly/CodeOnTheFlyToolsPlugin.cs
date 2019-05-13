using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Utils.CodeOnTheFly
{
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
