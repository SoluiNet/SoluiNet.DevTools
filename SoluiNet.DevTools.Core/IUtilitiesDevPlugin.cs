using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Core
{
    public interface IUtilitiesDevPlugin
    {
        /// <summary>
        /// The plugin name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The label which should be used for menu items
        /// </summary>
        string MenuItemLabel { get; }

        /// <summary>
        /// The method which should be called on click
        /// </summary>
        void Execute(Action<UserControl> displayInPluginContainer);
    }
}
