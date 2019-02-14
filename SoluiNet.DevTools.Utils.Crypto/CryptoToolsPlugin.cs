using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SoluiNet.DevTools.Core;

namespace SoluiNet.DevTools.Utils.Crypto
{
    public class CryptoToolsPlugin : IUtilitiesDevPlugin
    {
        public string Name
        {
            get { return "CryptoToolsPlugin"; }
        }

        public string MenuItemLabel
        {
            get { return "Crypto Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new CryptToolsUserControl());
        }
    }
}
