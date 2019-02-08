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

                        //data += string.Format("CAST('{0}' AS DATETIME)", dateValue.ToString("yyyy-MM-ddTHH:mm:ss.ffffzzz"));
                        //data += string.Format("CONVERT(DATETIME, '{0}', 127)", dateValue.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                        data += string.Format("CONVERT(DATETIME, '{0}', 126)", dateValue.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                    }
                    else if (intRegex.IsMatch(columnData.ToString()) || floatRegex.IsMatch(columnData.ToString()))
                    {
                        data += columnData;
                    }
                    else switch (columnData)
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

        public static string GetDataGridAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n", bool quoteTexts = false, string textQuote = "\"")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetStringForDataTable(dataTable, separator, rowSeparator, quoteTexts, textQuote);
        }

        public static string GetDataGridAsXml(DataGrid dataGrid, string rootElementName = "Table", string recordElementName = "Record")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetXmlStringForDataTable(dataTable, rootElementName, recordElementName);
        }

        public static string GetDataGridAsSql(DataGrid dataGrid, string tableName = "Table")
        {
            var dataTable = GetDataGridData(dataGrid);

            return GetSqlStringForDataTable(dataTable, tableName);
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

        public static string GetSelectedCellAsText(DataGrid dataGrid)
        {
            var dataRow = (DataRowView)dataGrid.SelectedItem;
            var index = dataGrid.CurrentCell.Column.DisplayIndex;
            var cellValue = dataRow.Row.ItemArray[index].ToString();

            return cellValue;
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
