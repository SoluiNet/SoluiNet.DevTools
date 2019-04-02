using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Extensions;
using SoluiNet.DevTools.Core.Tools;
using SoluiNet.DevTools.Core.Tools.Database;
using SoluiNet.DevTools.Core.Tools.UI;

namespace SoluiNet.DevTools.SqlPlugin.Example
{
    public class ExamplePlugin: ISqlDevPlugin
    {
        private Grid MainGrid { get; set; }

        public void DisplayForWpf(Grid mainGrid)
        {
            MainGrid = mainGrid;

            var tabControl = mainGrid.GetChildOfType<TabControl>();

            if (tabControl.Name == "MainTabs")
            {
                var tabItem = new TabItem() { Header = "Example", Name = "Example_TabItem", Background = new LinearGradientBrush(AccentColour1, AccentColour2, 0.00), Foreground = new SolidColorBrush(ForegroundColour) };

                tabControl.Items.Add(tabItem);

                tabItem.Content = new Grid() { Name = "Example_TabItem_Content", Background = new LinearGradientBrush(BackgroundAccentColour, BackgroundColour, 45.00) };

                ((Grid)tabItem.Content).Children.Add(new TextBox()
                {
                    Text = "Search for...",
                    Name = "Example_TabItem_SearchPhrase",
                    Height = 23,
                    Width = 391,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(10, 10, 0, 0)
                });

                ((Grid)tabItem.Content).Children.Add(new Button()
                {
                    Content = "Search",
                    Name = "Example_TabItem_SearchButton",
                    Width = 125,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(406, 10, 0, 0)
                });

                ((Grid)tabItem.Content).FindChild<Button>("Example_TabItem_SearchButton").Click += (o, i) =>
                {
                    var searchPhrase = MainGrid.FindChild<TextBox>("Example_TabItem_SearchPhrase").Text;

                    MainGrid.FindChild<TabControl>("Example_TabItem_Tabs").Items.Clear();

                    /*using (var context = new ExampleContext(Environment))
                    {
                        var customerResults = context.ConfigurationValues.Where(x => x.ConfigKey.Contains(searchPhrase));

                        UIHelper.FillResultsTab("Example", MainGrid, "Customers", customerResults, new List<string>()
                        {
                            "Created",
                            "Customernumber",
                            "Firstname",
                            "Lastname",
                            "Email",
                            "LockedOut"
                        });
                    }*/
                };

                ((Grid)tabItem.Content).Children.Add(new TabControl()
                {
                    Name = "Example_TabItem_Tabs",
                    Margin = new Thickness(10, 54, 10, 10)
                });
            }
        }

        public string Name
        {
            get { return "Example"; }
        }

        public Color AccentColour1
        {
            get { return Color.FromRgb(128, 128, 128); }
        }

        public Color AccentColour2
        {
            get { return Color.FromRgb(200, 200, 200); }
        }

        public Color ForegroundColour
        {
            get { return Colors.Black; }
        }

        public Color BackgroundColour
        {
            get { return Color.FromRgb(200, 200, 200); }
        }

        public Color BackgroundAccentColour
        {
            get { return Color.FromRgb(128, 128, 128); }
        }

        public string Environment { get; set; }

        public string ConnectionStringName
        {
            get { return PluginHelper.GetConnectionString(this, DefaultConnectionStringName); }
        }

        public string DefaultConnectionStringName
        {
            get { return "ExampleConnection"; }
        }

        public DataTable ExecuteSql(string sqlCommand)
        {
            return DbHelper.ExecuteSqlServerCommand(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, sqlCommand);
        }
        public List<DataTable> ExecuteSqlScript(string sqlCommand)
        {
            return DbHelper.ExecuteSqlServerScript(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, sqlCommand);
        }
    }
}
