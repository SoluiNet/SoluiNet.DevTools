// <copyright file="WebClientUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;
    using SoluiNet.DevTools.Core.Extensions;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.String;
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
        private IContainsSettings ChosenPlugin { get; set; }

        private bool RequestShowAdditionalOptions { get; set; }

        /// <summary>
        /// Extract SSL protocol from stream (taken from https://stackoverflow.com/questions/48589590/which-tls-version-was-negotiated/48675492).
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns the SSL protocol.</returns>
        private static SslProtocols ExtractSslProtocol(Stream stream)
        {
            if (stream is null)
            {
                return SslProtocols.None;
            }

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var metaStream = stream;

            if (stream.GetType().BaseType == typeof(GZipStream))
            {
                metaStream = (stream as GZipStream).BaseStream;
            }
            else if (stream.GetType().BaseType == typeof(DeflateStream))
            {
                metaStream = (stream as DeflateStream).BaseStream;
            }

            var connection = metaStream?.GetType().GetProperty("Connection", bindingFlags)?.GetValue(metaStream);

            if (connection == null)
            {
                return SslProtocols.None;
            }

            var usingSecureStream = connection.GetType().GetProperty("UsingSecureStream", bindingFlags);

            if (usingSecureStream != null && !(bool)usingSecureStream.GetValue(connection))
            {
                // Not a Https connection
                return SslProtocols.None;
            }

            var tlsStream = connection.GetType().GetProperty("NetworkStream", bindingFlags)?.GetValue(connection);
            var tlsState = tlsStream?.GetType().GetField("m_Worker", bindingFlags)?.GetValue(tlsStream);
            if (tlsState != null)
            {
                return (SslProtocols)tlsState.GetType()?.GetProperty("SslProtocol", bindingFlags)?.GetValue(tlsState);
            }

            return SslProtocols.None;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intended exception handling has been added to method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5397:Do not use deprecated SslProtocols values", Justification = "Even older protocols should be supported by this method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5398:Avoid hardcoded SslProtocols values", Justification = "We want to identify protocols by ther versio no.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5386:Avoid hardcoding SecurityProtocolType value", Justification = "We want to use TLS 1.2 explicitly")]
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

                var uri = new Uri(url);

                var request = (HttpWebRequest)WebRequest.Create(uri);
                var content = this.Input.Text.SetEnvironment(environment).InjectCommonValues().InjectSettings(settings);

                request.Method = this.HttpMethod.Text;

                var isSoapRequest = this.AdditionalOptions.Options.Any(x => x.Key == "SOAPAction");
                var useTls12 = this.AdditionalOptions.Options.Any(x => x.Key == "UseTls12") &&
                               this.AdditionalOptions.Options.First(x => x.Key == "UseTls12").Value.IsAffirmative();

                if (useTls12)
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                }

                if (isSoapRequest)
                {
                    var injectionDictionary = new Dictionary<string, string>()
                    {
                        { "ContentLength", content.Length.ToString(CultureInfo.InvariantCulture) },
                    };

                    request.ContentType = this.ContentType.Text;

                    foreach (var element in this.AdditionalOptions.Options)
                    {
                        request.Headers.Add(element.Key, element.Value.SetEnvironment(environment).InjectCommonValues().InjectSettings(settings).Inject(injectionDictionary));
                    }

                    var soapEnvelopeXml = new XmlDocument() { XmlResolver = null };

                    var stringReader = new StringReader(content);
                    var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings() { XmlResolver = null });

                    try
                    {
                        soapEnvelopeXml.Load(xmlReader);

                        WebClientTools.InsertSoapEnvelope(soapEnvelopeXml, request);
                    }
                    finally
                    {
                        xmlReader.Dispose();
                    }
                }

                if (!string.IsNullOrEmpty(this.RequestAuthenticationUser.Text))
                {
                    request.Headers.Add("Authorization", "Basic " + (this.RequestAuthenticationUser.Text + ":" + this.RequestAuthenticationPassword.Password).ToBase64());
                }

                // begin async call to web request.
                var asyncResult = request.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something useful here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                string result;

                using (var response = request.EndGetResponse(asyncResult))
                {
                    var sslProtocols = this.ExtractSslProtocol(response.GetResponseStream());

                    if (sslProtocols >= SslProtocols.Tls12)
                    {
                        this.SslVersion.Content = "TLS 1.2";
                    }
                    else if (sslProtocols >= SslProtocols.Tls11)
                    {
                        this.SslVersion.Content = "TLS 1.1";
                    }
                    else if (sslProtocols >= SslProtocols.Tls)
                    {
                        this.SslVersion.Content = "TLS";
                    }
                    else if (sslProtocols >= SslProtocols.Ssl3)
                    {
                        this.SslVersion.Content = "SSL 3";
                    }
                    else if (sslProtocols >= SslProtocols.Ssl2)
                    {
                        this.SslVersion.Content = "SSL 2";
                    }
                    else if (sslProtocols >= SslProtocols.None)
                    {
                        this.SslVersion.Content = "None";
                    }

                    this.ReturnCode.Content = (response as HttpWebResponse)?.StatusCode.ToString() ?? "NONE";
                    this.ReturnType.Content = response.ContentType;

                    using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException("Response is null or empty")))
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
                if (webEx.Response == null)
                {
                    this.Output.Text = string.Format(CultureInfo.InvariantCulture, "##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##", webEx.Message, webEx.InnerException?.Message);
                }
                else
                {
                    var errResp = webEx.Response;

                    using (var responseStream = errResp.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream);

                        try
                        {
                            var responseText = reader.ReadToEnd();

                            this.Output.Text = string.Format(
                                CultureInfo.InvariantCulture,
                                "##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##\r\n\r\n{2}",
                                webEx.Message,
                                webEx.InnerException?.Message,
                                XmlHelper.IsXml(responseText) ? XmlHelper.Format(responseText) : responseText);
                        }
                        finally
                        {
                            reader.Dispose();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.Output.Text = string.Format(CultureInfo.InvariantCulture, "##EXCEPTION##\r\n{0}\r\n{1}\r\n##EXCEPTION##", exception.Message, exception.InnerException?.Message);
            }
        }

        private void AdditionalOptions_Click(object sender, RoutedEventArgs e)
        {
            var tagInfo = Convert.ToString(this.ToggleAdditionalOptions.Tag, CultureInfo.InvariantCulture);

            var expanded = Convert.ToBoolean(string.IsNullOrEmpty(tagInfo) ? "false" : tagInfo, CultureInfo.InvariantCulture);

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
            var plugins = PluginHelper.GetPlugins<ISupportsWebClient>();

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

        private void RequestShowAuthentication_Click(object sender, RoutedEventArgs e)
        {
            if (!this.RequestShowAdditionalOptions)
            {
                this.RequestAdditionalOptionRow.Height = new GridLength(150.0);

                this.RequestAuthenticationUser.Visibility = Visibility.Visible;
                this.RequestAuthenticationPassword.Visibility = Visibility.Visible;

                this.RequestShowAdditionalOptions = true;
            }
            else
            {
                this.RequestAdditionalOptionRow.Height = new GridLength(40.0);

                this.RequestAuthenticationUser.Visibility = Visibility.Hidden;
                this.RequestAuthenticationPassword.Visibility = Visibility.Hidden;

                this.RequestShowAdditionalOptions = false;
            }
        }
    }
}
