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
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.XML;
    using SoluiNet.DevTools.Core.WebClientDefinition;

    /// <summary>
    /// Interaction logic for WebClientPluginSelection.xaml.
    /// </summary>
    public partial class WebClientPluginSelection : UserControl
    {
        private readonly ICollection<ISupportsWebClient> plugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientPluginSelection"/> class.
        /// </summary>
        /// <param name="plugins">The plugins which support web client execution.</param>
        public WebClientPluginSelection(ICollection<ISupportsWebClient> plugins)
        {
            this.InitializeComponent();
            this.plugins = plugins;
        }

        /// <summary>
        /// The delegate for the return of the chosen plugin method.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <param name="content">The content.</param>
        /// <param name="supportedHttpMethods">The supported HTTP methods.</param>
        /// <param name="supportedContentTypes">The supported content types.</param>
        /// <param name="additionalOptions">The additional options.</param>
        /// <param name="chosenPlugin">The chosen technical plugin name.</param>
        public delegate void ReturnWebMethodToMainForm(Dictionary<string, string> endpoints, string content, List<string> supportedHttpMethods, List<string> supportedContentTypes = null, Dictionary<string, string> additionalOptions = null, IContainsSettings chosenPlugin = null);

        /// <summary>
        /// Gets or sets the event handler for the return of the chosen plugin method.
        /// </summary>
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
            if (!((this.Method.SelectedItem as ComboBoxItem)?.Tag is SoluiNetWebMethodType chosenMethod) 
                || !((this.Plugin.SelectedItem as ComboBoxItem)?.Tag is SoluiNetWebClientDefinition chosenWebClient))
            {
                return;
            }

            var selectedPlugin = (this.Plugin.SelectedItem as ComboBoxItem)?.Content.ToString();
            var plugin = this.plugins.FirstOrDefault(x => !string.IsNullOrEmpty(selectedPlugin) && x.Name == selectedPlugin);

            this.ReturnChosenMethod(
                chosenWebClient.Endpoints.ToDictionary(x => x.Name, x => x.Url),
                chosenMethod.RequestContent,
                chosenMethod.SupportedHttpMethods.Select(x => Enum.GetName(typeof(SoluiNetHttpMethodType), x)).ToList(),
                chosenMethod.SupportedContentTypes.Select(x => Enum.GetName(typeof(SoluiNetContentType), x)).ToList(),
                chosenMethod.PreparedHttpHeaders.ToDictionary(x => x.Name, x => x.Value),
                plugin is IContainsSettings settings ? settings : null);

            this.CloseCurrentWindow();
        }

        private void Plugin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var webClientConfiguration = (this.Plugin.SelectedItem as ComboBoxItem)?.Tag as SoluiNetWebClientDefinition;

            if (webClientConfiguration?.Methods == null)
            {
                return;
            }

            this.Method.Items.Clear();

            foreach (var method in webClientConfiguration.Methods)
            {
                this.Method.Items.Add(new ComboBoxItem() { Content = method.Name, Tag = method });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var plugin in this.plugins)
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
