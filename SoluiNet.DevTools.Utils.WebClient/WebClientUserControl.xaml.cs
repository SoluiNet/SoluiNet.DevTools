using System;
using System.IO;
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

                if (!string.IsNullOrEmpty(SoapAction.Text))
                {
                    request.ContentType = ContentType.Text;
                    request.Headers.Add("SOAPAction", SoapAction.Text);

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
    }
}
