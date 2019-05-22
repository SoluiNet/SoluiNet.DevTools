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
    public interface ISqlDevPlugin : IBasePlugin, IThemedPlugin
    {
        /// <summary>
        /// Method which should be executed to display the plugin in an WPF application
        /// </summary>
        void DisplayForWpf(Grid mainGrid);

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

        /// <summary>
        /// Execute a custom SQL command
        /// </summary>
        /// <param name="sqlCommand">The SQL Command</param>
        /// <returns>A DataTable which has the results</returns>
        List<DataTable> ExecuteSqlScript(string sqlCommand);
    }
}
