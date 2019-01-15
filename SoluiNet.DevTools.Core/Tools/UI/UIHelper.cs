using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public static DataTable GetDataGridData(DataGrid dataGrid)
        {
            if (dataGrid.ItemsSource != null)
                return (dataGrid.ItemsSource as DataView)?.ToTable();

            var dataTable = new DataTable();

            foreach (var column in dataGrid.Columns)
            {
                dataTable.Columns.Add(column.Header.ToString());
            }

            foreach (var row in dataGrid.Items)
            {
                dataTable.Rows.Add(((DataRowView)row).Row.ItemArray);
            }

            return dataTable;
        }

        public static string GetDataGridAsText(DataGrid dataGrid, string separator = "\t", string rowSeparator = "\r\n")
        {
            var data = string.Empty;
            var dataTable = GetDataGridData(dataGrid);

            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];

                data += column.ColumnName;

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

                    data += ((DataRow)row)[column.ColumnName].ToString();

                    if (i < dataGrid.Columns.Count - 1)
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
