// <copyright file="MainWindow.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using NLog;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.Events;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.UI.Application;
    using SoluiNet.DevTools.Core.UI.WPF.Application;
    using SoluiNet.DevTools.Core.UI.WPF.Tools.UI;
    using SoluiNet.DevTools.Core.UI.WPF.Window;
    using SoluiNet.DevTools.Core.Windows.Tools.HotKey;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : SoluiNetWindow
    {
        private readonly GlobalHotKey hotKey = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            var utilitiesDevPlugins = (System.Windows.Application.Current as ISoluiNetUiWpfApp)?.UtilityPlugins;

            if (utilitiesDevPlugins != null)
            {
                foreach (var utilityPlugin in utilitiesDevPlugins)
                {
                    try
                    {
                        if (utilityPlugin is IGroupable)
                        {
                            var groupPluginMenuItem = UIHelper.GetMenuItemByName(this.ExtrasMenuItem,
                                (utilityPlugin as IGroupable).Group);

                            var utilityPluginMenuItem = new System.Windows.Controls.MenuItem()
                            {
                                Header = utilityPlugin.MenuItemLabel,
                            };

                            utilityPluginMenuItem.Click += (sender, args) =>
                            {
                                utilityPlugin.Execute(x =>
                                {
                                    var pluginVisualizeWindow = new VisualPluginContainer();
                                    pluginVisualizeWindow.SetTitleParts(new Dictionary<string, string>()
                                    {
                                        {
                                            "0",
                                            string.Format(CultureInfo.InvariantCulture, "{0} / {1}",
                                                (utilityPlugin as IGroupable).Group, utilityPlugin.MenuItemLabel)
                                        }
                                    });

                                    pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                    pluginVisualizeWindow.ShowDialog();

                                    if (x is IDisposable disposableObject)
                                    {
                                        disposableObject.Dispose();
                                    }
                                });
                            };

                            groupPluginMenuItem.Items.Add(utilityPluginMenuItem);
                        }
                        else
                        {
                            var utilityPluginMenuItem = new System.Windows.Controls.MenuItem()
                            {
                                Header = utilityPlugin.MenuItemLabel,
                            };

                            utilityPluginMenuItem.Click += (sender, args) =>
                            {
                                utilityPlugin.Execute(x =>
                                {
                                    var pluginVisualizeWindow = new VisualPluginContainer();
                                    pluginVisualizeWindow.SetTitleParts(new Dictionary<string, string>()
                                        {{"0", utilityPlugin.MenuItemLabel}});

                                    pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                    pluginVisualizeWindow.ShowDialog();

                                    if (x is IDisposable disposableObject)
                                    {
                                        disposableObject.Dispose();
                                    }
                                });
                            };

                            this.ExtrasMenuItem.Items.Add(utilityPluginMenuItem);
                        }
                    }
                    catch (Exception exception)
                    {
                        this.Logger.Error(exception);

                        throw;
                    }
                }
            }

            foreach (var uiElement in (System.Windows.Application.Current as ISoluiNetUiApp).UiElements)
            {
                var tabItem = new TabItem() { Header = uiElement.Label, Name = uiElement.TechnicalName + "_TabItem" };

                this.UiElements.Items.Add(tabItem);

                tabItem.Content = uiElement;
                uiElement.TitleChanged += (sender, titleParts) =>
                {
                    this.SetTitleParts(titleParts);
                };
            }

            this.LoggingPath = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoluiNet.DevTools.UI");
        }

        private string LoggingPath { get; }

        private Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
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

        private void SoluiNetWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.hotKey?.Unregister();
        }

        private void SoluiNetWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.hotKey?.Register();

            foreach (var plugin in PluginHelper.GetPlugins<IHandlesEvent<IApplicationStartedEvent>>())
            {
                plugin.HandleEvent<IApplicationStartedEvent>(new Dictionary<string, object>());
            }
        }
    }
}
