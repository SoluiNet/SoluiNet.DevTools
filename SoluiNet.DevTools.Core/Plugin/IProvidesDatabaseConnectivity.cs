// <copyright file="IProvidesDatabaseConnectivity.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the interface for a plugin which will be working with SQL databases.
    /// </summary>
    public interface IProvidesDatabaseConnectivity : IBasePlugin
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
        /// Execute a custom SQL command. Working only with single commands.
        /// </summary>
        /// <param name="sqlCommand">The SQL command text.</param>
        /// <param name="identity">The impersonation identity.</param>
        /// <returns>A <see cref="DataTable"/> which has the results.</returns>
        DataTable ExecuteSql(string sqlCommand, WindowsIdentity identity = null);

        /// <summary>
        /// Execute a custom SQL command. May contain multiple commands.
        /// </summary>
        /// <param name="sqlCommand">The SQL command text.</param>
        /// <param name="identity">The impersonation identity.</param>
        /// <returns>A <see cref="DataTable"/> which has the results.</returns>
        List<DataTable> ExecuteSqlScript(string sqlCommand, WindowsIdentity identity = null);
    }
}
