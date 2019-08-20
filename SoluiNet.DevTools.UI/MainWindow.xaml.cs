// <copyright file="MainWindow.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI
{
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
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Editing;
    using Microsoft.Win32;
    using NLog;
    using NLog.Internal;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Formatter;
    using SoluiNet.DevTools.Core.Models;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.ScriptEngine;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Sql;
    using SoluiNet.DevTools.Core.Tools.UI;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.WPF.Window;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : SoluiNetWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            foreach (var utilityPlugin in (Application.Current as ISoluiNetApp).UtilityPlugins)
            {
                try
                {
                    if (utilityPlugin is IGroupable)
                    {
                        var groupPluginMenuItem = UIHelper.GetMenuItemByName(this.ExtrasMenuItem, (utilityPlugin as IGroupable).Group);

                        var utilityPluginMenuItem = new MenuItem()
                        {
                            Header = utilityPlugin.MenuItemLabel,
                        };

                        utilityPluginMenuItem.Click += (sender, args) =>
                        {
                            utilityPlugin.Execute(x =>
                            {
                                var pluginVisualizeWindow = new VisualPluginContainer();
                                pluginVisualizeWindow.SetTitleParts(new Dictionary<string, string>() { { "0", string.Format("{0} / {1}", (utilityPlugin as IGroupable).Group, utilityPlugin.MenuItemLabel) } });

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
                            Header = utilityPlugin.MenuItemLabel,
                        };

                        utilityPluginMenuItem.Click += (sender, args) =>
                        {
                            utilityPlugin.Execute(x =>
                            {
                                var pluginVisualizeWindow = new VisualPluginContainer();
                                pluginVisualizeWindow.SetTitleParts(new Dictionary<string, string>() { { "0", utilityPlugin.MenuItemLabel } });

                                pluginVisualizeWindow.ContentGrid.Children.Add(x);

                                pluginVisualizeWindow.Show();
                            });
                        };

                        this.ExtrasMenuItem.Items.Add(utilityPluginMenuItem);
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Error(exception);
                }
            }

            foreach (var uiElement in (Application.Current as ISoluiNetApp).UiElements)
            {
                var tabItem = new TabItem() { Header = uiElement.Label, Name = uiElement.TechnicalName + "_TabItem" };

                this.UiElements.Items.Add(tabItem);

                tabItem.Content = uiElement;
                uiElement.TitleChanged += (sender, titleParts) =>
                {
                    this.SetTitleParts(titleParts);
                };
            }

            this.LoggingPath = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoluiNet.DevTools.UI");
        }

        private string LoggingPath { get; set; }

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
    }
}
