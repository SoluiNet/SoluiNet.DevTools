using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoluiNet.DevTools.Utils.WebClient
{
    public static class WebClientTools
    {
        public static void InsertSoapEnvelope(XmlDocument soapEnvelope, HttpWebRequest webRequest)
        {
            using (var stream = webRequest.GetRequestStream())
            {
                soapEnvelope.Save(stream);
            }
        }
    }
}
