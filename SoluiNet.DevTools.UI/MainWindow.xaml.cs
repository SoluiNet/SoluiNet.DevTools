using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.Win32;
using NLog;
using NLog.Internal;
using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Formatter;
using SoluiNet.DevTools.Core.Models;
using SoluiNet.DevTools.Core.ScriptEngine;
using SoluiNet.DevTools.Core.Tools;
using SoluiNet.DevTools.Core.Tools.UI;
using SoluiNet.DevTools.Core.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SoluiNet.DevTools.UI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string LoggingPath { get; set; }

        private Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            var highlighting = UIHelper.LoadHighlightingDefinition(typeof(ShowText), "SQL.xshd");

            SqlCommandText.SyntaxHighlighting = highlighting;
            SqlCommandText.TextArea.TextEntered += SqlCommandText_TextEntered;

            foreach (var plugin in ((App)Application.Current).SqlPlugins)
            {
                try
                {
                    plugin.DisplayForWpf(MainGrid);

                    Project.Items.Add(plugin.Name);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }
            }

            foreach (var utilityPlugin in ((App)Application.Current).UtilityPlugins)
            {
                try
                {
                    if (utilityPlugin is IGroupedUtilitiesDevPlugin)
                    {
                        var groupPluginMenuItem = UIHelper.GetMenuItemByName(ExtrasMenuItem, (utilityPlugin as IGroupedUtilitiesDevPlugin).Group);

                        var utilityPluginMenuItem = new MenuItem()
                        {
                            Header = utilityPlugin.MenuItemLabel
                        };

                        utilityPluginMenuItem.Click += (sender, args) =>
                        {
                            utilityPlugin.Execute(x =>
                            {
                                var pluginVisualizeWindow = new VisualPluginContainer();

                                pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                pluginVisualizeWindow.Show();
                            });
                        };

                        groupPluginMenuItem.Items.Add(utilityPluginMenuItem);
                    }
                    else
                    {
                        var utilityPluginMenuItem = new MenuItem()
                        {
                            Header = utilityPlugin.MenuItemLabel
                        };

                        utilityPluginMenuItem.Click += (sender, args) =>
                        {
                            utilityPlugin.Execute(x =>
                            {
                                var pluginVisualizeWindow = new VisualPluginContainer();

                                pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                pluginVisualizeWindow.Show();
                            });
                        };

                        ExtrasMenuItem.Items.Add(utilityPluginMenuItem);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }
            }

            LoggingPath = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoluiNet.DevTools.UI");
        }

        private static void PopulateGridContextMenu(DataGrid dataGrid)
        {
            var contextMenu = dataGrid.ContextMenu;

            if (contextMenu == null)
            {
                return;
            }

            #region Copy to Clipboard
            var copyToClipboardMenuItem = new MenuItem()
            {
                Header = "Copy to Clipboard"
            };

            copyToClipboardMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                //Clipboard.SetData(DataFormats.StringFormat, UIHelper.GetDataGridData(containingGrid));
                Clipboard.SetText(UIHelper.GetDataGridAsText(containingGrid));
            };

            contextMenu.Items.Add(copyToClipboardMenuItem);
            #endregion

            #region Copy selected rows to Clipboard
            var copySelectionToClipboardMenuItem = new MenuItem()
            {
                Header = "Copy selected rows to Clipboard"
            };

            copySelectionToClipboardMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                Clipboard.SetText(UIHelper.GetDataGridSelectedRowsAsText(containingGrid));
            };

            contextMenu.Items.Add(copySelectionToClipboardMenuItem);
            #endregion

            #region Copy column to Clipboard
            var copyColumnToClipboardMenuItem = new MenuItem()
            {
                Header = "Copy column to Clipboard"
            };

            copyColumnToClipboardMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                Clipboard.SetText(UIHelper.GetDataGridColumnsAsText(containingGrid));
            };

            contextMenu.Items.Add(copyColumnToClipboardMenuItem);
            #endregion

            #region Copy selected column to Clipboard
            var copySelectedColumnToClipboardMenuItem = new MenuItem()
            {
                Header = "Copy selected column to Clipboard"
            };

            copySelectedColumnToClipboardMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                Clipboard.SetText(UIHelper.GetDataGridSelectedColumnsAsText(containingGrid));
            };

            contextMenu.Items.Add(copySelectedColumnToClipboardMenuItem);
            #endregion

            #region Copy selected cell to Clipboard
            var copySelectedCellToClipboardMenuItem = new MenuItem()
            {
                Header = "Copy selected cell to Clipboard"
            };

            copySelectedCellToClipboardMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                Clipboard.SetText(UIHelper.GetSelectedCellAsText(containingGrid));
            };

            contextMenu.Items.Add(copySelectedCellToClipboardMenuItem);
            #endregion

            contextMenu.Items.Add(new Separator());

            #region Save as CSV
            var saveAsCsvMenuItem = new MenuItem()
            {
                Header = "Save as CSV"
            };

            saveAsCsvMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                var saveFileDialog = new SaveFileDialog()
                {
                    DefaultExt = ".csv",
                    Filter = "Comma Seperated Values (*.csv)|*.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, UIHelper.GetDataGridAsText(containingGrid, ",", "\r\n", true), Encoding.UTF8);
                }
            };

            contextMenu.Items.Add(saveAsCsvMenuItem);
            #endregion

            #region Save as XML
            var saveAsXmlMenuItem = new MenuItem()
            {
                Header = "Save as XML"
            };

            saveAsXmlMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                var saveFileDialog = new SaveFileDialog()
                {
                    DefaultExt = ".xml",
                    Filter = "Extensible Markup Language (*.xml)|*.xml",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, UIHelper.GetDataGridAsXml(containingGrid), Encoding.UTF8);
                }
            };

            contextMenu.Items.Add(saveAsXmlMenuItem);
            #endregion

            #region Save as SQL
            var saveAsSqlMenuItem = new MenuItem()
            {
                Header = "Save as SQL"
            };

            saveAsSqlMenuItem.Click += (sender, eventInfo) =>
            {
                var containingGrid = (((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as DataGrid);

                if (containingGrid == null)
                {
                    return;
                }

                var saveFileDialog = new SaveFileDialog()
                {
                    DefaultExt = ".sql",
                    Filter = "Structured Query Language (*.sql)|*.sql",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, string.Format("/*{0}*/\r\n{1}",
                        ((TabItem)containingGrid.Parent).Tag.ToString(),
                        UIHelper.GetDataGridAsSql(containingGrid)), Encoding.UTF8);
                }
            };

            contextMenu.Items.Add(saveAsSqlMenuItem);
            #endregion
        }

        private void ExecuteSqlCommand()
        {
            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == Project.Text);

            if (plugin == null)
            {
                return;
            }

            var sqlCommand = string.IsNullOrEmpty(SqlCommandText.SelectedText)
                ? SqlCommandText.Text
                : SqlCommandText.SelectedText;

            Logger.Info(string.Format("[{1} ({2})] Executing SQL command: {0}", sqlCommand, Project.Text, plugin.Environment));

            var data = plugin.ExecuteSqlScript(sqlCommand);

            foreach (var table in data)
            {
                var dataGridSqlResults = new DataGrid();
                //dataGridSqlResults.AutoGenerateColumns = true;
                dataGridSqlResults.BeginningEdit += BeginEditSqlResult;

                dataGridSqlResults.ContextMenu = new ContextMenu();

                PopulateGridContextMenu(dataGridSqlResults);

                dataGridSqlResults.LoadingRow += (sender, eventInfo) =>
                {
                    eventInfo.Row.Header = (eventInfo.Row.GetIndex() + 1).ToString();
                };

                dataGridSqlResults.CopyingRowClipboardContent += (sender, args) =>
                {
                    args.ClipboardRowContent.Clear();
                    args.ClipboardRowContent.Add(new DataGridClipboardCellContent(args.Item, (sender as DataGrid).Columns[0], UIHelper.GetSelectedCellAsText(sender as DataGrid)));

                };

                var tabIndexSqlResults = SqlResults.Items.Add(new TabItem()
                {
                    Header = new ContentControl()
                    {
                        Content = string.Format("{0} - {1:yyyy-MM-dd\"T\"HH:mm:ss}", plugin.Name, DateTime.Now)
                    }
                });

                var tabItem = (TabItem)SqlResults.Items[tabIndexSqlResults];

                tabItem.Content = dataGridSqlResults;
                tabItem.Tag = table.ExtendedProperties["SqlCommand"];

                ((ContentControl)tabItem.Header).MouseRightButtonDown += (element, mouseEvent) =>
                {
                    SqlResults.Items.Remove((TabItem)((ContentControl)element).Parent);
                };

                ((ContentControl)tabItem.Header).MouseDown += (element, mouseEvent) =>
                {
                    if (mouseEvent.ChangedButton == MouseButton.Middle && mouseEvent.ButtonState == MouseButtonState.Pressed)
                    {
                        MessageBox.Show(((TabItem)((ContentControl)element).Parent).Tag.ToString());
                    }
                };

                foreach (DataColumn column in table.Columns)
                {
                    dataGridSqlResults.Columns.Add(new DataGridTextColumn() { Header = column.ColumnName, Binding = new Binding(column.ColumnName) });
                }

                //dataGridSqlResults.DataContext = data.DefaultView;

                foreach (var row in table.DefaultView)
                {
                    dataGridSqlResults.Items.Add(row);
                }

                SqlResults.SelectedIndex = tabIndexSqlResults;
            }
        }

        private void BeginEditSqlResult(object sender, DataGridBeginningEditEventArgs e)
        {
            //throw new NotImplementedException();

            e.Cancel = true;
        }

        private void ExecuteSql_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSqlCommand();
        }

        private void SqlCommandText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ExecuteSqlCommand();
            }
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseSchema.Visibility == Visibility.Collapsed)
            {
                DatabaseSchema.Visibility = Visibility.Visible;
                (sender as Button).Content = "<";
            }
            else
            {
                DatabaseSchema.Visibility = Visibility.Collapsed;
                (sender as Button).Content = ">";
            }
        }

        private IList<Type> GetEntityTypes(string chosenProject)
        {
            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            return PluginHelper.GetEntityTypes(plugin);
        }

        private IList<string> GetEntityFields(string chosenProject, string entityName)
        {
            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            return PluginHelper.GetEntityFields(plugin, entityName);
        }

        private IList<SqlScript> GetSqlScripts(string chosenProject)
        {
            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            return PluginHelper.GetSqlScripts(plugin);
        }

        private IList<string> GetEnvironments(string chosenProject)
        {
            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            return PluginHelper.GetEnvironments(plugin);
        }

        private void ChangeProjectConnection(string chosenProject, string chosenEnvironment = "Default")
        {

            var dataEntities = GetEntityTypes(chosenProject);
            var sqlScripts = GetSqlScripts(chosenProject);

            if (dataEntities == null)
            {
                return;
            }

            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            if (plugin == null)
            {
                return;
            }

            DatabaseSchema.Items.Clear();

            var tablesNodeIndex = DatabaseSchema.Items.Add(new TreeViewItem() { Header = "Tables" });
            var tablesNode = (TreeViewItem)DatabaseSchema.Items[tablesNodeIndex];

            foreach (var entity in dataEntities)
            {
                var entityNodeIndex = tablesNode.Items.Add(new TreeViewItem() { Header = entity.Name, Tag = entity });

                var fields = GetEntityFields(chosenProject, entity.Name);

                foreach (var field in fields)
                {
                    ((TreeViewItem)tablesNode.Items[entityNodeIndex]).Items.Add(field);
                }
            }

            var storedProceduresNodeIndex = DatabaseSchema.Items.Add(new TreeViewItem() { Header = "Stored Procedures" });
            var storedProceduresNode = (TreeViewItem)DatabaseSchema.Items[storedProceduresNodeIndex];

            if (System.Configuration.ConfigurationManager.ConnectionStrings[plugin.ConnectionStringName].ProviderName == "System.Data.SqlClient")
            {
                // this works only for Microsoft SQL Server

                var sqlCommand = "SELECT OBJECT_NAME(OBJECT_ID) AS name, " +
                                 "definition " +
                                 "FROM sys.sql_modules " +
                                 "WHERE objectproperty(OBJECT_ID, 'IsProcedure') = 1 " +
                                 "ORDER BY OBJECT_NAME(OBJECT_ID)";

                var data = plugin.ExecuteSql(sqlCommand);

                foreach (DataRowView record in data.DefaultView)
                {
                    storedProceduresNode.Items.Add(new TreeViewItem()
                    {
                        Header = record.Row["name"],
                        Tag = new StoredProcedure
                        {
                            Name = record.Row["name"].ToString(),
                            BodyDefinition = record.Row["definition"].ToString()
                        }
                    });
                }
            }

            var storedFunctionsNodeIndex = DatabaseSchema.Items.Add(new TreeViewItem() { Header = "Stored Functions" });
            var storedFunctionsNode = (TreeViewItem)DatabaseSchema.Items[storedFunctionsNodeIndex];

            if (System.Configuration.ConfigurationManager.ConnectionStrings[plugin.ConnectionStringName].ProviderName == "System.Data.SqlClient")
            {
                // this works only for Microsoft SQL Server

                var sqlCommand = "SELECT DISTINCT " +
                    "s.name + '.' + o.name AS object_name, " +
                    "o.type_desc, " +
                    "m.definition " +
                    "FROM sys.sql_modules m " +
                    "INNER JOIN sys.objects o ON m.object_id = o.object_id " +
                    "INNER JOIN sys.schemas s ON o.schema_id = s.schema_id " +
                    "WHERE o.type_desc = 'SQL_SCALAR_FUNCTION'";

                var data = plugin.ExecuteSql(sqlCommand);

                foreach (DataRowView record in data.DefaultView)
                {
                    storedFunctionsNode.Items.Add(new TreeViewItem()
                    {
                        Header = record.Row["object_name"],
                        Tag = new StoredFunction
                        {
                            Name = record.Row["object_name"].ToString(),
                            BodyDefinition = record.Row["definition"].ToString()
                        }
                    });
                }
            }

            var viewsNodeIndex = DatabaseSchema.Items.Add(new TreeViewItem() { Header = "Views" });
            var viewsNode = (TreeViewItem)DatabaseSchema.Items[viewsNodeIndex];

            if (System.Configuration.ConfigurationManager.ConnectionStrings[plugin.ConnectionStringName].ProviderName == "System.Data.SqlClient")
            {
                // this works only for Microsoft SQL Server

                var sqlCommand = "SELECT name, definition " +
                                 "FROM sys.objects obj " +
                                 "JOIN sys.sql_modules mod ON mod.object_id = obj.object_id " +
                                 "WHERE obj.type = 'V'";

                var data = plugin.ExecuteSql(sqlCommand);

                foreach (DataRowView record in data.DefaultView)
                {
                    viewsNode.Items.Add(new TreeViewItem()
                    {
                        Header = record.Row["name"],
                        Tag = new View
                        {
                            Name = record.Row["name"].ToString(),
                            BodyDefinition = record.Row["definition"].ToString()
                        }
                    });
                }
            }

            var scriptsNodeIndex = DatabaseSchema.Items.Add(new TreeViewItem() { Header = "Scripts" });
            var scriptsNode = (TreeViewItem)DatabaseSchema.Items[scriptsNodeIndex];

            foreach (var sqlScript in sqlScripts)
            {
                scriptsNode.Items.Add(new TreeViewItem() { Header = sqlScript.Name, Tag = sqlScript });
            }
        }

        private void Project_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var chosenProject = (sender as ComboBox).SelectedItem as string;
            var environments = GetEnvironments(chosenProject);

            Environments.Items.Clear();

            foreach (var environment in environments)
            {
                Environments.Items.Add(environment);
            }

            ChangeProjectConnection(chosenProject);
        }

        private void SelectTopThousand_Click(object sender, RoutedEventArgs e)
        {
            if (!((DatabaseSchema.SelectedItem as TreeViewItem)?.Tag is Type))
            {
                MessageBox.Show("Selected Item is no table");

                return;
            }
            var chosenProject = Project.Text;

            GetSqlForTable(chosenProject);
        }

        private void DatabaseSchema_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private void PrepareText_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PrepareText();

            dialog.Show();
        }

        private void GetSqlForTable(string chosenProject)
        {
            var dataEntities = GetEntityTypes(chosenProject);

            if (dataEntities == null)
            {
                return;
            }

            var tableName = DatabaseSchema.SelectedItem is TreeViewItem item
                ? item.Header.ToString()
                : DatabaseSchema.SelectedItem.ToString();

            var sqlTableName = tableName;

            var tableAttribute = dataEntities.FirstOrDefault(x => x.Name == tableName)
                ?.GetCustomAttributes(typeof(TableAttribute)).FirstOrDefault();

            if (tableAttribute != null)
            {
                sqlTableName = ((TableAttribute)tableAttribute).Name;
            }

            var sqlTableNameElements = sqlTableName.Split('.');

            if (sqlTableNameElements.Length > 1)
            {
                sqlTableName = string.Join(".", sqlTableNameElements.Select(x => string.Format("\"{0}\"", x)));
            }

            SqlCommandText.Text += (string.IsNullOrEmpty(SqlCommandText.Text) ? string.Empty : "\r\n") +
                                   string.Format("SELECT TOP 1000 * FROM {0}", sqlTableName);
        }

        private void GetSqlForScript(TreeViewItem selectedItem)
        {
            if (selectedItem == null)
            {
                throw new ArgumentNullException(nameof(selectedItem));
            }

            var script = selectedItem.Tag as SqlScript;

            if (script == null)
            {
                return;
            }

            SqlCommandText.Text += string.Format("\r\n--{2}: {0}\r\n{1}", script.Description, script.CommandText, script.Name);
        }

        private void ShowDatabaseElementInfo(string type, TreeViewItem selectedItem, ISqlDevPlugin plugin)
        {
            if (selectedItem == null)
            {
                throw new ArgumentNullException(nameof(selectedItem));
            }

            var databaseElement = selectedItem.Tag as IDatabaseElement;
            var formatter = new SqlFormatter();

            if (databaseElement == null)
            {
                return;
            }

            Logger.Info(string.Format("[{1} ({2})] Displaying {3}: {0}", databaseElement.Name, plugin.Name, plugin.Environment, type));

            var dialog = new ShowText();
            dialog.Text = formatter.FormatString(databaseElement.BodyDefinition);

            dialog.Show();
        }

        private void ExecuteMainAction()
        {
            var chosenProject = Project.Text;

            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            if (plugin == null)
            {
                return;
            }

            var selectedItem = DatabaseSchema.SelectedItem as TreeViewItem;

            if (selectedItem == null)
            {
                return;
            }

            if (selectedItem.Tag is Type)
            {
                GetSqlForTable(chosenProject);
            }
            else if (selectedItem.Tag is SqlScript)
            {
                GetSqlForScript(selectedItem);
            }
            else if (selectedItem.Tag is StoredProcedure)
            {
                ShowDatabaseElementInfo("Stored Procedure", selectedItem, plugin);
            }
            else if (selectedItem.Tag is StoredFunction)
            {
                ShowDatabaseElementInfo("Stored Function", selectedItem, plugin);
            }
            else if (selectedItem.Tag is View)
            {
                ShowDatabaseElementInfo("View", selectedItem, plugin);
            }
        }

        private void DatabaseSchema_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExecuteMainAction();
        }

        private void Environments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var chosenProject = (sender as ComboBox).SelectedItem as string;
            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == Project.SelectedItem as string);

            if (plugin == null)
            {
                return;
            }

            plugin.Environment = (sender as ComboBox)?.SelectedItem as string;

            ChangeProjectConnection(chosenProject);
        }

        private void FileCloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExtrasOptionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Options();

            dialog.Show();
        }

        private void ShowCodeComplete()
        {
            var chosenProject = Project.SelectedItem as string;

            if (string.IsNullOrEmpty(chosenProject))
                return;

            var plugin = ((App)Application.Current).SqlPlugins.FirstOrDefault(x => x.Name == chosenProject);

            // Open code completion after the user has pressed dot:
            var completionWindow = new CompletionWindow(SqlCommandText.TextArea);

            IList<ICompletionData> completionData = completionWindow.CompletionList.CompletionData;

            foreach (var databaseElement in GetEntityTypes(chosenProject))
            {
                completionData.Add(new CompletionData(databaseElement.Name));
            }

            completionWindow.Show();
            completionWindow.Closed += delegate {
                completionWindow = null;
            };
        }

        private void SqlCommandText_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                ShowCodeComplete();
            }
        }
    }
}
