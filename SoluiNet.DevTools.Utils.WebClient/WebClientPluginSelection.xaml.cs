// <copyright file="WebClientPluginSelection.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.XML;
    using SoluiNet.DevTools.Core.WebClientDefinition;

    /// <summary>
    /// Interaktionslogik für WebClientPluginSelection.xaml
    /// </summary>
    public partial class WebClientPluginSelection : UserControl
    {
        private readonly List<IWebClientSupportPlugin> _plugins;

        public WebClientPluginSelection(List<IWebClientSupportPlugin> plugins)
        {
            this.InitializeComponent();
            this._plugins = plugins;
        }

        public delegate void ReturnWebMethodToMainForm(Dictionary<string, string> endpoints, string content, List<string> supportedHttpMethods, List<string> supportedContentTypes = null, Dictionary<string, string> additionalOptions = null, IPluginWithSettings chosenPlugin = null);

        public ReturnWebMethodToMainForm ReturnChosenMethod { get; set; }

        private void CloseCurrentWindow()
        {
            var window = Window.GetWindow(this);

            window?.Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.CloseCurrentWindow();
        }

        private void ChoosePluginMethod_Click(object sender, RoutedEventArgs e)
        {
            var chosenWebClient = (this.Plugin.SelectedItem as ComboBoxItem)?.Tag as SoluiNetWebClientDefinition;
            var chosenMethod = (this.Method.SelectedItem as ComboBoxItem)?.Tag as SoluiNetWebMethodType;

            if (chosenMethod == null || chosenWebClient == null)
            {
                return;
            }

            var selectedPlugin = (this.Plugin.SelectedItem as ComboBoxItem)?.Content.ToString();
            var plugin = this._plugins.FirstOrDefault(x => !string.IsNullOrEmpty(selectedPlugin) && x.Name == selectedPlugin);

            this.ReturnChosenMethod(
                chosenWebClient.Endpoints.ToDictionary(x => x.Name, x => x.Url),
                chosenMethod.RequestContent,
                chosenMethod.SupportedHttpMethods.Select(x => Enum.GetName(typeof(SoluiNetHttpMethodType), x)).ToList(),
                chosenMethod.SupportedContentTypes.Select(x => Enum.GetName(typeof(SoluiNetContentType), x)).ToList(),
                chosenMethod.PreparedHttpHeaders.ToDictionary(x => x.Name, x => x.Value),
                plugin is IPluginWithSettings settings ? settings : null
            );

            this.CloseCurrentWindow();
        }

        private void Plugin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var webClientConfiguration = (this.Plugin.SelectedItem as ComboBoxItem)?.Tag as SoluiNetWebClientDefinition;

            if (webClientConfiguration?.Methods == null)
                return;

            this.Method.Items.Clear();

            foreach (var method in webClientConfiguration.Methods)
            {
                this.Method.Items.Add(new ComboBoxItem() { Content = method.Name, Tag = method });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var plugin in this._plugins)
            {
                var webClientConfiguration = PluginHelper.GetWebClientDefinition(plugin);

                this.Plugin.Items.Add(new ComboBoxItem() { Content = plugin.Name, Tag = webClientConfiguration });
            }
        }

        private void Method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((this.Method.SelectedItem as ComboBoxItem)?.Tag is SoluiNetWebMethodType methodDefinition)
            {
                this.MethodDetails.Text = XmlHelper.Serialize<SoluiNetWebMethodType>(methodDefinition);

                this.ChoosePluginMethod.IsEnabled = true;
            }
        }
    }
}
