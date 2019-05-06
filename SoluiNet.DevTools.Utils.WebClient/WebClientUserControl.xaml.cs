﻿using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Extensions;
using SoluiNet.DevTools.Core.Tools;
using SoluiNet.DevTools.Core.Tools.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace SoluiNet.DevTools.Utils.WebClient
{
    /// <summary>
    /// Interaktionslogik für WebClientUserControl.xaml
    /// </summary>
    public partial class WebClientUserControl : UserControl
    {
        private IPluginWithSettings ChosenPlugin { get; set; }

        public WebClientUserControl()
        {
            InitializeComponent();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = PluginHelper.GetSettings(ChosenPlugin);

                var request = (HttpWebRequest)WebRequest.Create(TargetUrl.Text);
                var content = Input.Text.SetEnvironment().InjectCommonValues().InjectSettings(settings);

                //request.Accept = "text/xml";
                request.Method = HttpMethod.Text;

                var isSoapRequest = AdditionalOptions.Options.Any(x => x.Key == "SOAPAction");

                if (isSoapRequest)
                {
                    var injectionDictionary = new Dictionary<string, string>()
                    {
                        { "ContentLength", content.Length.ToString() }
                    };

                    request.ContentType = ContentType.Text;

                    foreach (var element in AdditionalOptions.Options)
                    {
                        request.Headers.Add(element.Key, element.Value.SetEnvironment().InjectCommonValues().InjectSettings(settings).Inject(injectionDictionary));
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

                Output.Text = result;
            }
            catch (WebException webEx)
            {
                var errResp = webEx.Response;

                using (var responseStream = errResp.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);

                    var responseText = reader.ReadToEnd();

                    Output.Text = string.Format("##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##\r\n\r\n{2}", webEx.Message, webEx.InnerException?.Message, XmlHelper.IsXml(responseText) ? XmlHelper.Format(responseText) : responseText);
                }
            }
            catch (Exception exception)
            {
                Output.Text = string.Format("##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##", exception.Message, exception.InnerException?.Message);
            }
        }

        private void AdditionalOptions_Click(object sender, RoutedEventArgs e)
        {
            var tagInfo = Convert.ToString(ToggleAdditionalOptions.Tag);

            var expanded = Convert.ToBoolean(string.IsNullOrEmpty(tagInfo) ? "false" : tagInfo);

            if (!expanded)
            {
                WebClientMainGrid.RowDefinitions[1].Height = new GridLength(200);
                ToggleAdditionalOptions.Content = "^";
                ToggleAdditionalOptions.Tag = true;
            }
            else
            {
                WebClientMainGrid.RowDefinitions[1].Height = new GridLength(0);
                ToggleAdditionalOptions.Content = "v";
                ToggleAdditionalOptions.Tag = false;
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
                        Input.Text = content;

                        TargetUrl.Items.Clear();
                        foreach (var comboBoxItem in endpoints.Select(x => new ComboBoxItem() { Content = x }))
                        {
                            TargetUrl.Items.Add(comboBoxItem);
                        }

                        HttpMethod.SelectedItem = HttpMethod.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content != null && x.Content.ToString() == methods.First());
                        ContentType.SelectedItem = ContentType.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content != null && x.Content.ToString() == contentTypes.First());

                        foreach (var option in options)
                        {
                            AdditionalOptions.AddOption(option.Key, option.Value);
                        }

                        ChosenPlugin = chosenPlugin;
                    }
                }
            };

            window.ShowDialog();
        }
    }
}
