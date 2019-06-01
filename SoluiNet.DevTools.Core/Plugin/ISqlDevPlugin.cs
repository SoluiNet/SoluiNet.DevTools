// <copyright file="ISqlDevPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Provides the interface for a plugin which will be working with SQL databases.
    /// </summary>
    public interface ISqlDevPlugin : IBasePlugin, IThemedPlugin
    {
        /// <summary>
        /// Gets or sets the environment in which the plugin should run in.
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// Gets the connection string for the environment the plugin runs in.
        /// </summary>
        string ConnectionStringName { get; }

        /// <summary>
        /// Gets the default connection string for the plugin.
        /// </summary>
        string DefaultConnectionStringName { get; }

        /// <summary>
        /// Method which should be executed to display the plugin in an WPF application.
        /// </summary>
        /// <param name="mainGrid">The grid in which the plugin should be displayed.</param>
        void DisplayForWpf(Grid mainGrid);

        /// <summary>
        /// Execute a custom SQL command. Working only with single commands.
        /// </summary>
        /// <param name="sqlCommand">The SQL command text.</param>
        /// <returns>A <see cref="DataTable"/> which has the results.</returns>
        DataTable ExecuteSql(string sqlCommand);

        /// <summary>
        /// Execute a custom SQL command. May contain multiple commands.
        /// </summary>
        /// <param name="sqlCommand">The SQL command text.</param>
        /// <returns>A <see cref="DataTable"/> which has the results.</returns>
        List<DataTable> ExecuteSqlScript(string sqlCommand);
    }
}
