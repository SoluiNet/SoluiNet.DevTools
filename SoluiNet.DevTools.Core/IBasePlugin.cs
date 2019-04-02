using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core
{
    public interface IBasePlugin
    {
        /// <summary>
        /// The plugin name
        /// </summary>
        string Name { get; }
    }
}
