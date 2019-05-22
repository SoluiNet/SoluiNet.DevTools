using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoluiNet.DevTools.Core
{
    public interface IThemedPlugin : IBasePlugin
    {
        /// <summary>
        /// The first colour accent
        /// </summary>
        Color AccentColour1 { get; }

        /// <summary>
        /// The second colour accent
        /// </summary>
        Color AccentColour2 { get; }

        /// <summary>
        /// The foreground colour
        /// </summary>
        Color ForegroundColour { get; }

        /// <summary>
        /// The background colour
        /// </summary>
        Color BackgroundColour { get; }

        /// <summary>
        /// The background accent colour
        /// </summary>
        Color BackgroundAccentColour { get; }
    }
}
