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

namespace SoluiNet.DevTools.Utils.WebClient
{
    /// <summary>
    /// Interaktionslogik für WebClientUserControl.xaml
    /// </summary>
    public partial class WebClientUserControl : UserControl
    {
        public WebClientUserControl()
        {
            InitializeComponent();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(TargetUrl.Text);

                //request.Accept = "text/xml";
                request.Method = HttpMethod.Text;

                var isSoapRequest = AdditionalOptions.Options.Any(x => x.Key == "SOAPAction");

                if (isSoapRequest)
                {
                    var injectionDictionary = new Dictionary<string, string>()
                    {
                        { "ContentLength", Input.Text.Length.ToString() }
                    };

                    request.ContentType = ContentType.Text;

                    foreach (var element in AdditionalOptions.Options)
                    {
                        request.Headers.Add(element.Key, element.Value.InjectCommonValues().Inject(injectionDictionary));
                    }

                    var soapEnvelopeXml = new XmlDocument();
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
                    }
                }

                Output.Text = result;
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

            foreach (var plugin in plugins)
            {
                // do something
            }
        }
    }
}
