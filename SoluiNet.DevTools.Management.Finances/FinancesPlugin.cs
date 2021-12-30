using NHibernate;
using NHibernate.Cfg;
using SoluiNet.DevTools.Core.Plugin;
using SoluiNet.DevTools.Core.UI.WPF.Extensions;
using SoluiNet.DevTools.Core.UI.WPF.Plugin;
using SoluiNet.DevTools.Management.Finances.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SoluiNet.DevTools.Management.Finances
{
    /// <summary>
    /// Provides a plugin that allows one to manage finances.
    /// </summary>
    public class FinancesPlugin : IManagementPlugin, IManagementUiPlugin
    {
        private Grid MainGrid { get; set; }

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
        /// Gets or sets the first accent colour.
        /// </summary>
        public Color AccentColour1
        {
            get { return Colors.Green; }
        }

        /// <summary>
        /// Gets or sets the second accent colour.
        /// </summary>
        public Color AccentColour2
        {
            get { return Colors.Blue; }
        }

        /// <summary>
        /// Gets or sets the foreground colour.
        /// </summary>
        public Color ForegroundColour
        {
            get { return Colors.White; }
        }

        /// <summary>
        /// Gets or sets the background colour.
        /// </summary>
        public Color BackgroundColour
        {
            get { return Colors.Black; }
        }

        /// <summary>
        /// Gets or sets the background accent colour.
        /// </summary>
        public Color BackgroundAccentColour
        {
            get { return Colors.DimGray; }
        }

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
                    Foreground = new SolidColorBrush(this.ForegroundColour)
                };

                tabControl.SelectionChanged += (sender, eventArgs) =>
                {
                    if(eventArgs.Source is TabControl)
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
                    Background = new LinearGradientBrush(this.BackgroundAccentColour, this.BackgroundColour, 45.00)
                };

                ((Grid)tabItem.Content).Children.Add(new FinancesUserControl());
            }
        }

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