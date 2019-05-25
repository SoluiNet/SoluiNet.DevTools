// <copyright file="WebClientTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides a collection of methods for web clients.
    /// </summary>
    public static class WebClientTools
    {
        /// <summary>
        /// Insert a soap envelope into a web request.
        /// </summary>
        /// <param name="soapEnvelope">The soap envelope.</param>
        /// <param name="webRequest">The web request.</param>
        public static void InsertSoapEnvelope(XmlDocument soapEnvelope, HttpWebRequest webRequest)
        {
            using (var stream = webRequest.GetRequestStream())
            {
                soapEnvelope.Save(stream);
            }
        }
    }
}
