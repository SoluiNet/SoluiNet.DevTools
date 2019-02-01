using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using SoluiNet.DevTools.Core.Extensions;

namespace SoluiNet.DevTools.Core.Tools.UI
{
    public static class UIHelper
    {
        public static void FillResultsTab(string key, Grid containingGrid, string title, IQueryable<object> results, IEnumerable<string> fields)
        {
            var tableKey = title.Replace(" ", string.Empty);

            var dataGrid = new DataGrid();

            var tabIndexCdv = containingGrid.FindChild<TabControl>(key + "_TabItem_Tabs").Items.Add(new TabItem() { Name = key + "_Grid_" + tableKey, Header = title });
            ((TabItem)containingGrid.FindChild<TabControl>(key + "_TabItem_Tabs").Items[tabIndexCdv]).Content = dataGrid;

            foreach (var field in fields)
            {
                dataGrid.Columns.Add(new DataGridTextColumn() { Header = field, Binding = new Binding(field) });
            }

            foreach (var item in results)
            {
                dataGrid.Items.Add(item);
            }
        }

        public static DataTable GetDataGridData(DataGrid dataGrid, List<string> columnNames = null)
        {
            if (dataGrid.ItemsSource != null)
                return (dataGrid.ItemsSource as DataView)?.ToTable();

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
                        continue;

                    columnsToAdd.Add(columnName, i);
                    dataTable.Columns.Add(columnName);
                }
            }

            if (columnNames == null)
            {
                foreach (var row in dataGrid.Items)
                {
                    dataTable.Rows.Add(((DataRowView) row).Row.ItemArray);
                }
            }
            else
            {
                foreach (var row in dataGrid.Items)
                {
                    var itemArray = ((DataRowView) row).Row.ItemArray;

                    var rowArray = itemArray.Where((t, i) => columnsToAdd.ContainsValue(i)).ToList();

                    dataTable.Rows.Add(rowArray.ToArray());
                }
            }

            return dataTable;
        }

        public static DataTable GetDataGridSelectedRowsData(DataGrid dataGrid, List<string> columnNames = null)
        {
            if (dataGrid.ItemsSource != null)
                return (dataGrid.ItemsSource as DataView)?.ToTable();

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
                        continue;

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

        private static string GetStringForDataTable(DataTable dataTable, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var data = string.Empty;
            var whitespaceRegex = new Regex("\\s");


            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];

                if (quoteTexts && whitespaceRegex.IsMatch(column.ColumnName))
                    data += textQuote;

                data += column.ColumnName;

                if (quoteTexts && whitespaceRegex.IsMatch(column.ColumnName))
                    data += textQuote;

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
                        data += textQuote;

                    data += columnData;

                    if (quoteTexts && whitespaceRegex.IsMatch(columnData))
                        data += textQuote;

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

        public static string GetDataGridAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        public static string GetDataGridSelectedRowsAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridSelectedRowsData(dataGrid);

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        public static string GetDataGridColumnsAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridData(dataGrid, new List<string> { dataGrid.CurrentCell.Column.Header.ToString() });

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        public static string GetDataGridSelectedColumnsAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridSelectedRowsData(dataGrid, new List<string> { dataGrid.CurrentCell.Column.Header.ToString() });

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

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
    }
}
