using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SoluiNet.DevTools.Core
{
    /// <summary>
    /// The DIP Plugin interface
    /// </summary>
    public interface ISqlDevPlugin
    {
        /// <summary>
        /// Method which should be executed to display the plugin in an WPF application
        /// </summary>
        void DisplayForWpf(Grid mainGrid);

        /// <summary>
        /// The plugin name
        /// </summary>
        string Name { get; }

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

        /// <summary>
        /// The environment in which the plugin should run in.
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// The connection string for the environment the plugin runs in.
        /// </summary>
        string ConnectionStringName { get; }

        /// <summary>
        /// The default connection string for the plugin.
        /// </summary>
        string DefaultConnectionStringName { get; }

        /// <summary>
        /// Execute a custom SQL command
        /// </summary>
        /// <param name="sqlCommand">The SQL Command</param>
        /// <returns>A DataTable which has the results</returns>
        DataTable ExecuteSql(string sqlCommand);
    }
}
