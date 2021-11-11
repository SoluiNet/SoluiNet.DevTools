// <copyright file="FreeAtHomePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.BuschJaeger.FreeAtHome
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Security.Principal;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// Provides a plugin for the Busch Jaeger free@home system.
    /// </summary>
    public class FreeAtHomePlugin: ISmartHomeUiPlugin
    {
        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        public string Environment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the connection string name.
        /// </summary>
        public string ConnectionStringName
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the default connection string name.
        /// </summary>
        public string DefaultConnectionStringName
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the first accent colour.
        /// </summary>
        public Color AccentColour1
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the second accent colour.
        /// </summary>
        public Color AccentColour2
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the foreground colour.
        /// </summary>
        public Color ForegroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the background colour.
        /// </summary>
        public Color BackgroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the background accent colour.
        /// </summary>
        public Color BackgroundAccentColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute a SQL statement.
        /// </summary>
        /// <param name="sqlCommand">The SQL statement.</param>
        /// <param name="identity">The windows identity under which the statement should be running.</param>
        /// <returns>Returns a DataTable for the result.</returns>
        public DataTable ExecuteSql(string sqlCommand, WindowsIdentity identity = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute a SQL script.
        /// </summary>
        /// <param name="sqlCommand">The SQL script.</param>
        /// <param name="identity">The windows identity under which the script should be running.</param>
        /// <returns>Returns a collection of DataTables for the result.</returns>
        public ICollection<DataTable> ExecuteSqlScript(string sqlCommand, WindowsIdentity identity = null)
        {
            throw new NotImplementedException();
        }
    }
}
