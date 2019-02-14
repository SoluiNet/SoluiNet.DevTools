using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SoluiNet.DevTools.Core;

namespace SoluiNet.DevTools.Utils.XmlTransformation
{
    public class XmlToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name {
            get { return "XmlTransformationPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "XML Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new XmlTransformationUserControl());
        }
    }
}
