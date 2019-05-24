// <copyright file="WebClientUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Extensions;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.XML;

    /// <summary>
    /// Interaction logic for WebClientUserControl.xaml.
    /// </summary>
    public partial class WebClientUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientUserControl"/> class.
        /// </summary>
        public WebClientUserControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the chosen plugin.
        /// </summary>
        private IPluginWithSettings ChosenPlugin { get; set; }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var environment = ((this.TargetUrl.SelectedItem as ComboBoxItem)?.Tag as Dictionary<string, string>)?["Environment"];
                var url = ((this.TargetUrl.SelectedItem as ComboBoxItem)?.Tag as Dictionary<string, string>)?["Url"];

                if (string.IsNullOrEmpty(environment))
                {
                    environment = "Default";
                }

                if (string.IsNullOrEmpty(url))
                {
                    url = this.TargetUrl.Text;
                }

                var settings = PluginHelper.GetSettings(this.ChosenPlugin);

                var request = (HttpWebRequest)WebRequest.Create(url);
                var content = this.Input.Text.SetEnvironment(environment).InjectCommonValues().InjectSettings(settings);

                request.Method = this.HttpMethod.Text;

                var isSoapRequest = this.AdditionalOptions.Options.Any(x => x.Key == "SOAPAction");

                if (isSoapRequest)
                {
                    var injectionDictionary = new Dictionary<string, string>()
                    {
                        { "ContentLength", content.Length.ToString() },
                    };

                    request.ContentType = this.ContentType.Text;

                    foreach (var element in this.AdditionalOptions.Options)
                    {
                        request.Headers.Add(element.Key, element.Value.SetEnvironment(environment).InjectCommonValues().InjectSettings(settings).Inject(injectionDictionary));
                    }

                    var soapEnvelopeXml = new XmlDocument();

                    soapEnvelopeXml.LoadXml(content);

                    WebClientTools.InsertSoapEnvelope(soapEnvelopeXml, request);
                }

                // begin async call to web request.
                IAsyncResult asyncResult = request.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something useful here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                var result = string.Empty;

                using (var response = request.EndGetResponse(asyncResult))
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();

                        if (response.ContentType.Contains("xml"))
                        {
                            result = XmlHelper.Format(result);
                        }
                    }
                }

                this.Output.Text = result;
            }
            catch (WebException webEx)
            {
                var errResp = webEx.Response;

                using (var responseStream = errResp.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);

                    var responseText = reader.ReadToEnd();

                    this.Output.Text = string.Format("##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##\r\n\r\n{2}", webEx.Message, webEx.InnerException?.Message, XmlHelper.IsXml(responseText) ? XmlHelper.Format(responseText) : responseText);
                }
            }
            catch (Exception exception)
            {
                this.Output.Text = string.Format("##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##", exception.Message, exception.InnerException?.Message);
            }
        }

        private void AdditionalOptions_Click(object sender, RoutedEventArgs e)
        {
            var tagInfo = Convert.ToString(this.ToggleAdditionalOptions.Tag);

            var expanded = Convert.ToBoolean(string.IsNullOrEmpty(tagInfo) ? "false" : tagInfo);

            if (!expanded)
            {
                this.WebClientMainGrid.RowDefinitions[1].Height = new GridLength(200);
                this.ToggleAdditionalOptions.Content = "^";
                this.ToggleAdditionalOptions.Tag = true;
            }
            else
            {
                this.WebClientMainGrid.RowDefinitions[1].Height = new GridLength(0);
                this.ToggleAdditionalOptions.Content = "v";
                this.ToggleAdditionalOptions.Tag = false;
            }
        }

        private void ReadFromPlugin_Click(object sender, RoutedEventArgs e)
        {
            var plugins = PluginHelper.GetPlugins<IWebClientSupportPlugin>();

            var window = new Window
            {
                Title = "Select Web Method from plugin list",
                Content = new WebClientPluginSelection(plugins)
                {
                    ReturnChosenMethod = (endpoints, content, methods, contentTypes, options, chosenPlugin) =>
                    {
                        this.Input.Text = content;

                        this.TargetUrl.Items.Clear();
                        foreach (var comboBoxItem in endpoints.Select(x => new ComboBoxItem() { Content = x.Key, Tag = new Dictionary<string, string>() { { "Url", x.Value }, { "Environment", x.Key } } }))
                        {
                            this.TargetUrl.Items.Add(comboBoxItem);
                        }

                        this.HttpMethod.SelectedItem = this.HttpMethod.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content != null && x.Content.ToString() == methods.First());
                        this.ContentType.SelectedItem = this.ContentType.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content != null && x.Content.ToString() == contentTypes.First());

                        foreach (var option in options)
                        {
                            this.AdditionalOptions.AddOption(option.Key, option.Value);
                        }

                        this.ChosenPlugin = chosenPlugin;
                    },
                },
            };

            window.ShowDialog();
        }
    }
}
