// <copyright file="PluginConfiguration.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using SoluiNet.DevTools.Core.Plugin.Configuration;

    /// <summary>
    /// Interaction logic for PluginConfiguration.xaml.
    /// </summary>
    public partial class PluginConfiguration : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            this.InitializeComponent();

            this.Installation.Items.Add("General");
            this.Installation.Items.Add("Effective");

            foreach (var configEntry in SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Current.SoluiNetConfigurationEntry)
            {
                if (configEntry.Item is SoluiNetInstallationType)
                {
                    var installationEntry = configEntry.Item as SoluiNetInstallationType;

                    if (!this.Installation.Items.Contains(installationEntry.path))
                    {
                        this.Installation.Items.Add(installationEntry.path);
                    }
                }
            }

            var executingAssemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (this.Installation.Items.Contains(executingAssemblyPath))
            {
                this.Installation.SelectedIndex = this.Installation.Items.IndexOf(executingAssemblyPath);
            }
            else
            {
                this.Installation.SelectedIndex = this.Installation.Items.IndexOf("General");
            }

            this.PluginList.ItemsSource = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Effective;
        }

        private void SavePluginConfiguration_Click(object sender, RoutedEventArgs e)
        {
            SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Save();
        }

        private void Installation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Installation.SelectedValue.ToString()))
            {
                return;
            }

            this.PluginList.IsReadOnly = false;

            if (this.Installation.SelectedValue.ToString() == "General")
            {
                this.PluginList.ItemsSource = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.GetConfigurationByScope();
            }
            else if (this.Installation.SelectedValue.ToString() == "Effective")
            {
                this.PluginList.IsReadOnly = true;
                this.PluginList.ItemsSource = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.Effective;
            }
            else
            {
                this.PluginList.ItemsSource = SoluiNet.DevTools.Core.Plugin.Configuration.Configuration.GetConfigurationByScope(this.Installation.SelectedValue.ToString());
            }
        }
    }
}
