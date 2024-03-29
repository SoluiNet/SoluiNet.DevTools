﻿// <copyright file="MainWindow.xaml.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.Tools.Plugin;
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
#pragma warning disable 0649
        [System.Diagnostics.CodeAnalysis.SuppressMessage("General", "CS0649:Field is never assigned to, and will always have its default value 'null'", Justification = "Prepared for later usage.")]
        private readonly GlobalHotKey hotKey;
#pragma warning restore 0649

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
                        if (utilityPlugin is IGroupable groupedUtilityPlugin)
                        {
                            var groupPluginMenuItem = UIHelper.GetMenuItemByName(
                                this.ExtrasMenuItem,
                                groupedUtilityPlugin.Group);

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
                                            string.Format(
                                                CultureInfo.InvariantCulture,
                                                "{0} / {1}",
                                                (utilityPlugin as IGroupable)?.Group,
                                                utilityPlugin.MenuItemLabel)
                                        },
                                    });

                                    pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                    pluginVisualizeWindow.Show();

                                    pluginVisualizeWindow.Closed += (closedSender, closedEventArgs) =>
                                    {
                                        if (x is IDisposable disposableObject)
                                        {
                                            disposableObject.Dispose();
                                        }
                                    };
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
                                        { { "0", utilityPlugin.MenuItemLabel } });

                                    pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                    pluginVisualizeWindow.Show();

                                    pluginVisualizeWindow.Closed += (closedSender, closedEventArgs) =>
                                    {
                                        if (x is IDisposable disposableObject)
                                        {
                                            disposableObject.Dispose();
                                        }
                                    };
                                });
                            };

                            this.ExtrasMenuItem.Items.Add(utilityPluginMenuItem);
                        }
                    }
                    catch (Exception exception)
                    {
                        MainWindow.Logger.Error(exception);

                        throw;
                    }
                }
            }

            if (Application.Current != null &&
                Application.Current is ISoluiNetUiApp app &&
                app.UiElements != null)
            {
                foreach (var uiElement in app.UiElements)
                {
                    var tabItem = new TabItem() { Header = uiElement.Label, Name = uiElement.TechnicalName + "_TabItem" };

                    this.UiElements.Items.Add(tabItem);

                    tabItem.Content = uiElement;
                    uiElement.TitleChanged += (sender, titleParts) => { this.SetTitleParts(titleParts); };
                }
            }
            else
            {
                Logger.Warn("No UI Elements defined.");
            }

            this.LoggingPath = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoluiNet.DevTools.UI");
        }

        /// <summary>
        /// Gets the Logging Path.
        /// </summary>
        public string LoggingPath { get; }

        private static Logger Logger
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
