using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Utils.File
{
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
