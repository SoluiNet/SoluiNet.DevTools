// <copyright file="ManagementUiElement.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.Management
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Microsoft.Win32;
    using NLog;
    using SoluiNet.DevTools.Core.Exceptions;
    using SoluiNet.DevTools.Core.Formatter;
    using SoluiNet.DevTools.Core.Models;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.ScriptEngine;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Sql;
    using SoluiNet.DevTools.Core.UI.UIElement;
    using SoluiNet.DevTools.Core.UI.WPF.Application;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Tools.UI;
    using SoluiNet.DevTools.Core.UI.WPF.UIElement.Editor;

    /// <summary>
    /// An UI element which can be used to management related functions.
    /// </summary>
    public partial class ManagementUiElement : UserControl, ISoluiNetUIElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementUiElement"/> class.
        /// </summary>
        public ManagementUiElement()
        {
            this.InitializeComponent();

            var managementUiPlugins = (Application.Current as ISoluiNetUiWpfApp)?.ManagementPlugins;

            if (managementUiPlugins != null)
            {
                foreach (var plugin in managementUiPlugins)
                {
                    try
                    {
                        plugin.Display(this.ManagementUiGrid);
                    }
                    catch (Exception exception)
                    {
                        ManagementUiElement.Logger.Error(exception);
                        throw;
                    }
                }
            }

            this.LoggingPath = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SoluiNet.DevTools.UI.Management");
        }

        /// <inheritdoc />
        public event TitleChangedHandler TitleChanged;

        /// <inheritdoc/>
        public string Label
        {
            get
            {
                return "Management";
            }
        }

        /// <inheritdoc/>
        public string TechnicalName
        {
            get
            {
                return "ManagementUi";
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <summary>
        /// Gets the logging path.
        /// </summary>
        private string LoggingPath { get; }

        private void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TitleChanged?.Invoke(this, new Dictionary<string, string>()
            {
                { "tab", this.MainTabs.SelectedContentStringFormat },
                { "tabAdditionalInfo", string.Empty },
            });
        }
    }
}
