using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Core
{
    public interface IGroupedUtilitiesDevPlugin : IUtilitiesDevPlugin
    {
        /// <summary>
        /// The label which should be used for group
        /// </summary>
        string Group { get; }
    }
}
