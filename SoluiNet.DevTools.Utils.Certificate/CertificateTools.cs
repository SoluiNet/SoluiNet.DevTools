// <copyright file="CertificateTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Certificate
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CSharp;
    using Newtonsoft.Json;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides a collection of methods for certificate handling.
    /// </summary>
    public static class CertificateTools
    {
        /// <summary>
        /// Get certificate by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the certificate which has been used for the overgiven URL.</returns>
        public static X509Certificate2 GetCertificateByUrl(Uri url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var response = request.GetResponse();

            response.Close();

            var certificate = request.ServicePoint.Certificate;

            return new X509Certificate2(certificate);
        }

        /// <summary>
        /// Get certificate by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Returns the certificate which has been used for the overgiven URL.</returns>
        public static X509Certificate2 GetCertificateByUrl(string url)
        {
            return GetCertificateByUrl(new Uri(url));
        }
    }
}
