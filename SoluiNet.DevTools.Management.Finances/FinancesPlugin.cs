// <copyright file="FinancesPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Security.Principal;
    using System.Windows;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
    using System.Windows.Media;
#endif
    using NHibernate;
    using NHibernate.Cfg;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Reference;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Extensions;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif
    using SoluiNet.DevTools.Management.Finances.Data;

    /// <summary>
    /// Provides a plugin that allows one to manage finances.
    /// </summary>
    public class FinancesPlugin : IManagementPlugin, IManagementUiPlugin
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return "FinancesPlugin"; }
        }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Gets the connection string name.
        /// </summary>
        public string ConnectionStringName { get; }

        /// <summary>
        /// Gets the default connection string name.
        /// </summary>
        public string DefaultConnectionStringName { get; }

        /// <summary>
        /// Gets the first accent colour.
        /// </summary>
        public IColour AccentColour1
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Green"); }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public IColour AccentColour2
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Blue"); }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public IColour ForegroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("White"); }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public IColour BackgroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Black"); }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public IColour BackgroundAccentColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DimGray"); }
        }

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

#if BUILD_FOR_WINDOWS
        /// <summary>
        /// Gets or sets the main grid.
        /// </summary>
        private Grid MainGrid { get; set; }

        /// <summary>
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            this.MainGrid = mainGrid;

            var tabControl = mainGrid.GetChildOfType<TabControl>();

            if (tabControl.Name == "MainTabs")
            {
                var tabItem = new TabItem()
                {
                    Header = "Finances",
                    Name = "Finances_TabItem",
                    Background = new LinearGradientBrush(this.AccentColour1, this.AccentColour2, 0.00),
                    Foreground = new SolidColorBrush(this.ForegroundColour),
                };

                tabControl.SelectionChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.Source is TabControl)
                    {
                        if (tabItem.IsSelected)
                        {
                            tabControl.Background = new SolidColorBrush(this.BackgroundColour);
                        }
                    }
                };

                tabControl.Items.Add(tabItem);

                tabItem.Content = new Grid()
                {
                    Name = "Finances_TabItem_Content",
                    Background = new LinearGradientBrush(this.BackgroundAccentColour, this.BackgroundColour, 45.00),
                };

                ((Grid)tabItem.Content).Children.Add(new FinancesUserControl());
            }
        }
#endif

        /// <summary>
        /// Execute a SQL statement.
        /// </summary>
        /// <param name="sqlCommand">The SQL statement.</param>
        /// <param name="identity">The windows identity under which the statement should be running.</param>
        /// <returns>Returns a DataTable for the result.</returns>
        public DataTable ExecuteSql(string sqlCommand, WindowsIdentity identity = null)
        {
            var result = new DataTable();

            try
            {
                var session = NHibernateContext.GetCurrentSession();

                using (var transaction = session.BeginTransaction())
                {
                    var query = session.CreateSQLQuery(sqlCommand);

                    var sqlResult = query.List();

                    foreach (var row in sqlResult)
                    {
                        if (row is object[] rowArray)
                        {
                            var i = 0;

                            while (result.Columns.Count < rowArray.Length)
                            {
                                result.Columns.Add(new DataColumn());
                                i++;
                            }

                            foreach (var item in rowArray)
                            {
                                result.Rows.Add(item);
                            }
                        }
                        else
                        {
                            if (result.Columns.Count == 0)
                            {
                                result.Columns.Add(new DataColumn());
                            }

                            result.Rows.Add(row);
                        }
                    }

                    // transaction.Commit();
                }
            }
            finally
            {
                NHibernateContext.CloseSession();
            }

            return result;
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