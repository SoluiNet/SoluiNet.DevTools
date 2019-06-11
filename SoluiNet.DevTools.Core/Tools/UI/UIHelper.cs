﻿// <copyright file="UIHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using SoluiNet.DevTools.Core.Extensions;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides functions for working with the UI.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Fill the SQL result tab.
        /// </summary>
        /// <param name="key">The key which should be used.</param>
        /// <param name="containingGrid">The grid in which the result tab can be found.</param>
        /// <param name="title">The title which should be used.</param>
        /// <param name="results">The result dataset.</param>
        /// <param name="fields">A list of field names.</param>
        public static void FillResultsTab(string key, Grid containingGrid, string title, IQueryable<object> results, IEnumerable<string> fields)
        {
            var tableKey = title.Replace(" ", string.Empty);

            var dataGrid = new DataGrid();
            dataGrid.IsReadOnly = true;

            var tabIndexCdv = containingGrid.FindChild<TabControl>(key + "_TabItem_Tabs").Items.Add(new TabItem() { Name = key + "_Grid_" + tableKey, Header = title });
            ((TabItem)containingGrid.FindChild<TabControl>(key + "_TabItem_Tabs").Items[tabIndexCdv]).Content = dataGrid;

            foreach (var field in fields)
            {
                dataGrid.Columns.Add(new DataGridTextColumn() { Header = StringHelper.PrepareHeaderLabel(field), Binding = new Binding(field) });
            }

            foreach (var item in results)
            {
                dataGrid.Items.Add(item);
            }
        }

        /// <summary>
        /// Get the data which is stored in a data grid as data table.
        /// </summary>
        /// <param name="dataGrid">The data grid.</param>
        /// <param name="columnNames">A list of column names which should be exported. If not provided all columns will be exported.</param>
        /// <returns>The contents of the <see cref="DataGrid"/> as <see cref="DataTable"/>.</returns>
        public static DataTable GetDataGridData(DataGrid dataGrid, List<string> columnNames = null)
        {
            if (dataGrid.ItemsSource != null)
            {
                return (dataGrid.ItemsSource as DataView)?.ToTable();
            }

            var dataTable = new DataTable();

            var columnsToAdd = new Dictionary<string, int>();

            if (columnNames == null)
            {
                for (var i = 0; i < dataGrid.Columns.Count; i++)
                {
                    columnsToAdd.Add(dataGrid.Columns[i].Header.ToString(), i);
                    dataTable.Columns.Add(dataGrid.Columns[i].Header.ToString());
                }
            }
            else
            {
                for (var i = 0; i < dataGrid.Columns.Count; i++)
                {
                    var columnName = dataGrid.Columns[i].Header.ToString();

                    if (!columnNames.Contains(columnName))
                    {
                        continue;
                    }

                    columnsToAdd.Add(columnName, i);
                    dataTable.Columns.Add(columnName);
                }
            }

            if (columnNames == null)
            {
                foreach (var row in dataGrid.Items)
                {
                    dataTable.Rows.Add(((DataRowView)row).Row.ItemArray);
                }
            }
            else
            {
                foreach (var row in dataGrid.Items)
                {
                    var itemArray = ((DataRowView)row).Row.ItemArray;

                    var rowArray = itemArray.Where((t, i) => columnsToAdd.ContainsValue(i)).ToList();

                    dataTable.Rows.Add(rowArray.ToArray());
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Get the data of the selected rows which is stored in a data grid as data table.
        /// </summary>
        /// <param name="dataGrid">The data grid.</param>
        /// <param name="columnNames">A list of column names which should be exported. If not provided all columns will be exported.</param>
        /// <returns>The contents of the selected rocws in the <see cref="DataGrid"/> as <see cref="DataTable"/>.</returns>
        public static DataTable GetDataGridSelectedRowsData(DataGrid dataGrid, List<string> columnNames = null)
        {
            if (dataGrid.ItemsSource != null)
            {
                return (dataGrid.ItemsSource as DataView)?.ToTable();
            }

            var dataTable = new DataTable();

            var columnsToAdd = new Dictionary<string, int>();

            if (columnNames == null)
            {
                for (var i = 0; i < dataGrid.Columns.Count; i++)
                {
                    columnsToAdd.Add(dataGrid.Columns[i].Header.ToString(), i);
                    dataTable.Columns.Add(dataGrid.Columns[i].Header.ToString());
                }
            }
            else
            {
                for (var i = 0; i < dataGrid.Columns.Count; i++)
                {
                    var columnName = dataGrid.Columns[i].Header.ToString();

                    if (!columnNames.Contains(columnName))
                    {
                        continue;
                    }

                    columnsToAdd.Add(columnName, i);
                    dataTable.Columns.Add(columnName);
                }
            }

            if (columnNames == null)
            {
                foreach (var row in dataGrid.SelectedItems)
                {
                    dataTable.Rows.Add(((DataRowView)row).Row.ItemArray);
                }
            }
            else
            {
                foreach (var row in dataGrid.SelectedItems)
                {
                    var itemArray = ((DataRowView)row).Row.ItemArray;

                    var rowArray = itemArray.Where((t, i) => columnsToAdd.ContainsValue(i)).ToList();

                    dataTable.Rows.Add(rowArray.ToArray());
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Get the data from a data grid as string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <param name="separator">The field separator. If not provided a tabulator character will be used.</param>
        /// <param name="rowSeparator">The row separator. If not provided a CRLF will be used.</param>
        /// <param name="quoteTexts">A value which indicates if text data should be quoted. If not provided no quoting will be done.</param>
        /// <param name="textQuote">The text quote character(s). If not provided a quotation mark will be used.</param>
        /// <returns>A <see cref="string"/> which represents the data from the <see cref="DataGrid"/>.</returns>
        public static string GetDataGridAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        /// <summary>
        /// Get the data from a data grid as XML string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <param name="rootElementName">The root element name. If not provided "Table" will be used.</param>
        /// <param name="recordElementName">The record element name. If not provided "Record" will be used.</param>
        /// <returns>A XML <see cref="string"/> which represents the data from the <see cref="DataGrid"/>.</returns>
        public static string GetDataGridAsXml(DataGrid dataGrid, string rootElementName = "Table", string recordElementName = "Record")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetXmlStringForDataTable(dataTable, rootElementName, recordElementName);
        }

        /// <summary>
        /// Get the data from a data grid as SQL string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <param name="tableName">The table name. If not provided "Table" will be used.</param>
        /// <returns>A XML <see cref="string"/> which represents the data from the <see cref="DataGrid"/>.</returns>
        public static string GetDataGridAsSql(DataGrid dataGrid, string tableName = "Table")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetSqlStringForDataTable(dataTable, tableName);
        }

        /// <summary>
        /// Get the data from selected rows in a data grid as string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <param name="separator">The field separator. If not provided a tabulator character will be used.</param>
        /// <param name="rowSeparator">The row separator. If not provided a CRLF will be used.</param>
        /// <param name="quoteTexts">A value which indicates if text data should be quoted. If not provided no quoting will be done.</param>
        /// <param name="textQuote">The text quote character(s). If not provided a quotation mark will be used.</param>
        /// <returns>A <see cref="string"/> which represents the data from the selected rows in a <see cref="DataGrid"/>.</returns>
        public static string GetDataGridSelectedRowsAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridSelectedRowsData(dataGrid);

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        /// <summary>
        /// Get the data from selected columns in a data grid as string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <param name="separator">The field separator. If not provided a tabulator character will be used.</param>
        /// <param name="rowSeparator">The row separator. If not provided a CRLF will be used.</param>
        /// <param name="quoteTexts">A value which indicates if text data should be quoted. If not provided no quoting will be done.</param>
        /// <param name="textQuote">The text quote character(s). If not provided a quotation mark will be used.</param>
        /// <returns>A <see cref="string"/> which represents the data from the selected columns in a <see cref="DataGrid"/>.</returns>
        public static string GetDataGridColumnsAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridData(dataGrid, new List<string> { dataGrid.CurrentCell.Column.Header.ToString() });

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        /// <summary>
        /// Get the data from selected cells in a data grid as string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <param name="separator">The field separator. If not provided a tabulator character will be used.</param>
        /// <param name="rowSeparator">The row separator. If not provided a CRLF will be used.</param>
        /// <param name="quoteTexts">A value which indicates if text data should be quoted. If not provided no quoting will be done.</param>
        /// <param name="textQuote">The text quote character(s). If not provided a quotation mark will be used.</param>
        /// <returns>A <see cref="string"/> which represents the data from the selected cells in a <see cref="DataGrid"/>.</returns>
        public static string GetDataGridSelectedColumnsAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridSelectedRowsData(dataGrid, new List<string> { dataGrid.CurrentCell.Column.Header.ToString() });

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        /// <summary>
        /// Get the data from a single cell in a data grid as string.
        /// </summary>
        /// <param name="dataGrid">The data grid which should be exported.</param>
        /// <returns>A <see cref="string"/> which represents the data from a single cell in a <see cref="DataGrid"/>.</returns>
        public static string GetSelectedCellAsText(DataGrid dataGrid)
        {
            var cellValue = string.Empty;

            try
            {
                var dataRow = (DataRowView)dataGrid.SelectedItem;
                var index = dataGrid.CurrentCell.Column.DisplayIndex;
                cellValue = dataRow.Row.ItemArray[index].ToString();
            }
            catch
            {
                cellValue = "CELL_VALUE_ERROR";
            }

            return cellValue;
        }

        /// <summary>
        /// Get the highlighting definition for the overgiven resource name.
        /// </summary>
        /// <param name="type">A type in which assembly the resource should be contained.</param>
        /// <param name="resourceName">The resource name.</param>
        /// <returns>The <see cref="IHighlightingDefinition"/> which was saved with the overgiven resource name.</returns>
        public static IHighlightingDefinition LoadHighlightingDefinition(Type type, string resourceName)
        {
            var fullName = type.Namespace + "." + resourceName;

            using (var stream = type.Assembly.GetManifestResourceStream(fullName))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        /// <summary>
        /// Get a menu item by name.
        /// </summary>
        /// <param name="parentMenuItem">The parent menu item.</param>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="MenuItem"/> which has the overgiven name.</returns>
        public static MenuItem GetMenuItemByName(MenuItem parentMenuItem, string name)
        {
            MenuItem menuItem = null;

            foreach (var item in parentMenuItem.Items)
            {
                var childMenuItem = item as MenuItem;

                if (childMenuItem == null || childMenuItem.Header.ToString() != name)
                {
                    continue;
                }

                menuItem = item as MenuItem;
                break;
            }

            if (menuItem == null)
            {
                menuItem = new MenuItem()
                {
                    Header = name,
                };

                parentMenuItem.Items.Add(menuItem);
            }

            return menuItem;
        }

        /// <summary>
        /// Find an element which is a <see cref="TreeViewItem" /> and is a parent for the <paramref name="source"/> item.
        /// </summary>
        /// <param name="source">The source item.</param>
        /// <returns>Returns the <see cref="TreeViewItem" /> parent of <paramref name="source"/>.</returns>
        public static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        /// <summary>
        /// Provide a default implementation for the assembly resolve event.
        /// </summary>
        /// <param name="sender">The sender which triggered the event.</param>
        /// <param name="args">A <see cref="ResolveEventArgs"/> with additional arguments about the resolve event.</param>
        /// <returns>Returns an instance of <see cref="Assembly"/> which contains the assembly for which the event was looking for.</returns>
        public static Assembly LoadUiElementAssembly(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath))
            {
                return null;
            }

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            var assemblyUiPath = Path.Combine(folderPath, "UI", new AssemblyName(args.Name).Name + ".dll");

            if (!System.IO.File.Exists(assemblyPath) && !System.IO.File.Exists(assemblyUiPath))
            {
                return null;
            }

            Assembly assembly = null;

            if (System.IO.File.Exists(assemblyPath))
            {
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            else if (System.IO.File.Exists(assemblyUiPath))
            {
                assembly = Assembly.LoadFrom(assemblyUiPath);
            }

            return assembly;
        }

        /// <summary>
        /// Gets the list of routed event handlers subscribed to the specified routed event.
        /// Taken from: https://stackoverflow.com/questions/9434817/how-to-remove-all-click-event-handlers
        /// </summary>
        /// <param name="element">The UI element on which the event is defined.</param>
        /// <param name="routedEvent">The routed event for which to retrieve the event handlers.</param>
        /// <returns>The list of subscribed routed event handlers.</returns>
        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
            // for getting an array of the subscribed event handlers.
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            return routedEventHandlers;
        }

        /// <summary>
        /// Remove event from UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="eventName">The event name.</param>
        public static void RemoveEvent(this Control uiElement, string eventName = "Click")
        {
            /* FieldInfo fieldInfo = typeof(Control).GetField(string.Format("Event{0}", eventName), BindingFlags.Static | BindingFlags.NonPublic);
            object eventClick = fieldInfo.GetValue(uiElement);

            PropertyInfo property = uiElement.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);

            EventHandlerList list = (EventHandlerList)property.GetValue(uiElement, null);

            list.RemoveHandler(eventClick, list[eventClick]); */

            var routedEventHandlers = GetRoutedEventHandlers(uiElement, ButtonBase.ClickEvent);

            foreach (var routedEventHandler in routedEventHandlers)
            {
                if (uiElement is Button)
                {
                    (uiElement as Button).Click -= (RoutedEventHandler)routedEventHandler.Handler;
                }
            }
        }

        /// <summary>
        /// Get the data from a data table as string.
        /// </summary>
        /// <param name="dataTable">The data table which should be exported.</param>
        /// <param name="separator">The field separator. If not provided a tabulator character will be used.</param>
        /// <param name="rowSeparator">The row separator. If not provided a CRLF will be used.</param>
        /// <param name="quoteTexts">A value which indicates if text data should be quoted. If not provided no quoting will be done.</param>
        /// <param name="textQuote">The text quote character(s). If not provided a quotation mark will be used.</param>
        /// <returns>A <see cref="string"/> which represents the data from the <see cref="DataTable"/>.</returns>
        private static string GetStringForDataTable(DataTable dataTable, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var data = string.Empty;
            var whitespaceRegex = new Regex("\\s");

            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];

                if (quoteTexts && whitespaceRegex.IsMatch(column.ColumnName))
                {
                    data += textQuote;
                }

                data += column.ColumnName;

                if (quoteTexts && whitespaceRegex.IsMatch(column.ColumnName))
                {
                    data += textQuote;
                }

                if (i < dataTable.Columns.Count - 1)
                {
                    data += separator;
                }
                else
                {
                    data += rowSeparator;
                }
            }

            foreach (var row in dataTable.Rows)
            {
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];
                    var columnData = ((DataRow)row)[column.ColumnName].ToString();

                    if (quoteTexts && whitespaceRegex.IsMatch(columnData))
                    {
                        data += textQuote;
                    }

                    data += columnData;

                    if (quoteTexts && whitespaceRegex.IsMatch(columnData))
                    {
                        data += textQuote;
                    }

                    if (i < dataTable.Columns.Count - 1)
                    {
                        data += separator;
                    }
                    else
                    {
                        data += rowSeparator;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Get the data from a data table as XML string.
        /// </summary>
        /// <param name="dataTable">The data table which should be exported.</param>
        /// <param name="rootElementName">The root element name. If not provided "Table" will be used.</param>
        /// <param name="recordElementName">The record element name. If not provided "Record" will be used.</param>
        /// <returns>A XML <see cref="string"/> which represents the data from the <see cref="DataTable"/>.</returns>
        private static string GetXmlStringForDataTable(DataTable dataTable, string rootElementName = "Table", string recordElementName = "Record")
        {
            var data = string.Empty;

            data += "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n";
            data += string.Format("<{0}>", rootElementName);

            foreach (var row in dataTable.Rows)
            {
                data += string.Format("<{0}>", recordElementName);

                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];

                    data += string.Format("<{0}>", column.ColumnName);

                    var columnData = ((DataRow)row)[column.ColumnName].ToString();
                    data += columnData;

                    data += string.Format("</{0}>", column.ColumnName);
                }

                data += string.Format("</{0}>", recordElementName);
            }

            data += string.Format("</{0}>", rootElementName);

            return data;
        }

        /// <summary>
        /// Get the data from a data table as SQL string.
        /// </summary>
        /// <param name="dataTable">The data table which should be exported.</param>
        /// <param name="tableName">The table name. If not provided "Table" will be used.</param>
        /// <returns>A XML <see cref="string"/> which represents the data from the <see cref="DataTable"/>.</returns>
        private static string GetSqlStringForDataTable(DataTable dataTable, string tableName = "Table")
        {
            var intRegex = new Regex("^\\d+$");
            var floatRegex = new Regex("^\\d+.\\d+$");
            var dateRegex = new Regex(@"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))(\s+|T)(([01]\d|2[0-4]):([0-5]\d):([0-5]\d))$");
            var germanDateRegex = new Regex(@"^((0[1-9]|[12]\d|3[01])\.(0[1-9]|1[0-2])\.[12]\d{3})(\s+|T)(([01]\d|2[0-4]):([0-5]\d):([0-5]\d))$");

            var data = string.Empty;

            data += string.Format("INSERT INTO {0} (", tableName);

            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];

                data += column.ColumnName;

                if (i < dataTable.Columns.Count - 1)
                {
                    data += ", ";
                }
            }

            data += ") VALUES ";

            foreach (var row in dataTable.Rows)
            {
                data += "(";

                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];

                    var columnData = ((DataRow)row)[column.ColumnName];

                    if (string.IsNullOrEmpty(columnData.ToString()))
                    {
                        data += "NULL";
                    }
                    else if (dateRegex.IsMatch(columnData.ToString()) || germanDateRegex.IsMatch(columnData.ToString()))
                    {
                        var dateValue = Convert.ToDateTime(columnData);

                        data += string.Format("CONVERT(DATETIME, '{0}', 126)", dateValue.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                    }
                    else if (intRegex.IsMatch(columnData.ToString()) || floatRegex.IsMatch(columnData.ToString()))
                    {
                        data += columnData;
                    }
                    else
                    {
                        switch (columnData)
                        {
                            case int _:
                            case float _:
                                data += columnData;
                                break;
                            case DateTime _:
                                data += string.Format("CAST('{0}' AS DATETIME)", ((DateTime)columnData).ToString("yyyy-MM-ddTHH:mm:ss.ffffzzz"));
                                break;
                            default:
                                data += string.Format("'{0}'", columnData.ToString().Replace("'", "''"));
                                break;
                        }
                    }

                    if (i < dataTable.Columns.Count - 1)
                    {
                        data += ", ";
                    }
                }

                data += "), ";
            }

            data = data.Substring(0, data.Length - 2);
            data += ";";

            return data;
        }
    }
}
