using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Utils.Svn
{
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
