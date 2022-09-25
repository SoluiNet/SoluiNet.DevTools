// <copyright file="ExamplePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SqlPlugin.Example
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Security.Principal;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Enums;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Reference;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Database;
    using SoluiNet.DevTools.Core.Tools.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Extensions;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Reference;
    using SoluiNet.DevTools.Core.Windows.Tools.Database;
    using SoluiNet.DevTools.Core.Windows.Tools.Security;

    /// <summary>
    /// A plugin which provides methods to work with the example project.
    /// </summary>
    public class ExamplePlugin : ISqlUiPlugin, ISupportsWebClient, IContainsSettings, IGroupable, IUtilitiesDevPlugin
    {
        /// <inheritdoc/>
        public string Group
        {
            get { return "Example"; }
        }

        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "Example"; }
        }

        /// <inheritdoc/>
        public IColour AccentColour1
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(128, 128, 128); }
        }

        /// <inheritdoc/>
        public IColour AccentColour2
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(200, 200, 200); }
        }

        /// <inheritdoc/>
        public IColour ForegroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Black"); }
        }

        /// <inheritdoc/>
        public IColour BackgroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(200, 200, 200); }
        }

        /// <inheritdoc/>
        public IColour BackgroundAccentColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(128, 128, 128); }
        }

        /// <inheritdoc/>
        public string Environment { get; set; }

        /// <inheritdoc/>
        public string ConnectionStringName
        {
            get { return PluginHelper.GetConnectionString(this, this.DefaultConnectionStringName); }
        }

        /// <inheritdoc/>
        public string DefaultConnectionStringName
        {
            get { return "ExampleConnection"; }
        }

        /// <inheritdoc/>
        public WebClientFormat Format
        {
            get { return WebClientFormat.Xml; }
        }

        /// <inheritdoc/>
        public WebClientType Type
        {
            get { return WebClientType.Soap; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Utility Example"; }
        }

        private Grid MainGrid { get; set; }

        /// <inheritdoc/>
        public DataTable ExecuteSql(string sqlCommand, WindowsIdentity identity = null)
        {
            if (identity == null)
            {
                // even if identity hasn't been passed there is a possibility that settings will be available.
                var settings = this.RetrieveSettingsAsDictionary();

                if (settings.TryGetValue($"{this.Environment ?? "Default"}.Impersonation@{this.Name}.User", out var user) &&
                    settings.TryGetValue($"{this.Environment ?? "Default"}.Impersonation@{this.Name}.Password", out var password))
                {
                    identity = settings.TryGetValue($"{this.Environment ?? "Default"}.Impersonation@{this.Name}.Domain", out var domain)
                            ? SecurityTools.GetIdentityByName(userName: user.ToString(), password: password.ToString(), domainName: domain.ToString())
                            : SecurityTools.GetIdentityByName(userName: user.ToString(), password: password.ToString());
                }
            }

            return Core.Windows.Tools.Database.DbHelper.ExecuteSqlCommand(
                ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ProviderName,
                ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString,
                sqlCommand,
                impersonationIdentity: identity);
        }

        /// <inheritdoc/>
        public ICollection<DataTable> ExecuteSqlScript(string sqlCommand, WindowsIdentity identity = null)
        {
            if (identity == null)
            {
                // even if identity hasn't been passed there is a possibility that settings will be available.
                var settings = this.RetrieveSettingsAsDictionary();

                if (settings.TryGetValue($"{this.Environment ?? "Default"}.Impersonation@{this.Name}.User", out var user) &&
                    settings.TryGetValue($"{this.Environment ?? "Default"}.Impersonation@{this.Name}.Password", out var password))
                {
                    identity = settings.TryGetValue($"{this.Environment ?? "Default"}.Impersonation@{this.Name}.Domain", out var domain)
                        ? SecurityTools.GetIdentityByName(userName: user.ToString(), password: password.ToString(), domainName: domain.ToString())
                        : SecurityTools.GetIdentityByName(userName: user.ToString(), password: password.ToString());
                }
            }

            return Core.Windows.Tools.Database.DbHelper.ExecuteSqlScript(
                ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ProviderName,
                ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString,
                sqlCommand,
                impersonationIdentity: identity) as List<DataTable>;
        }

        /// <inheritdoc/>
        public void Display(Grid mainGrid)
        {
            this.MainGrid = mainGrid;

            var tabControl = mainGrid.GetChildOfType<TabControl>();

            if (tabControl.Name == "MainTabs")
            {
                var tabItem = new TabItem()
                {
                    Header = "Example",
                    Name = "Example_TabItem",
                    Background = new LinearGradientBrush(
                        (this.AccentColour1 as Colour).MediaColorValue,
                        (this.AccentColour2 as Colour).MediaColorValue,
                        0.00),
                    Foreground = new SolidColorBrush((this.ForegroundColour as Colour).MediaColorValue),
                };

                tabControl.Items.Add(tabItem);

                tabItem.Content = new Grid()
                {
                    Name = "Example_TabItem_Content",
                    Background = new LinearGradientBrush(
                        (this.BackgroundAccentColour as Colour).MediaColorValue,
                        (this.BackgroundColour as Colour).MediaColorValue,
                        45.00),
                };

                ((Grid)tabItem.Content).Children.Add(new TextBox()
                {
                    Text = "Search for...",
                    Name = "Example_TabItem_SearchPhrase",
                    Height = 23,
                    Width = 391,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(10, 10, 0, 0),
                });

                ((Grid)tabItem.Content).Children.Add(new Button()
                {
                    Content = "Search",
                    Name = "Example_TabItem_SearchButton",
                    Width = 125,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(406, 10, 0, 0),
                });

                ((Grid)tabItem.Content).FindChild<Button>("Example_TabItem_SearchButton").Click += (o, i) =>
                {
                    var searchPhrase = this.MainGrid.FindChild<TextBox>("Example_TabItem_SearchPhrase").Text;

                    this.MainGrid.FindChild<TabControl>("Example_TabItem_Tabs").Items.Clear();

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
                    Margin = new Thickness(10, 54, 10, 10),
                });
            }
        }

        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            if (displayInPluginContainer == null)
            {
                throw new ArgumentNullException(nameof(displayInPluginContainer));
            }

            displayInPluginContainer(new ExampleUserControl());
        }
    }
}
