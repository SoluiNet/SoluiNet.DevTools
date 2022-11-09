// <copyright file="WebResponse.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Tools.Stream;

    /// <summary>
    /// A class which represents a web response.
    /// </summary>
    public class WebResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebResponse"/> class.
        /// </summary>
        public WebResponse()
        {
            this.StatusCode = HttpStatusCode.OK;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResponse"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public WebResponse(Encoding encoding)
            : this()
        {
            this.Encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResponse"/> class.
        /// </summary>
        /// <param name="response">The response text.</param>
        /// <param name="encoding">The encoding.</param>
        public WebResponse(string response, Encoding encoding)
            : this(encoding)
        {
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResponse"/> class.
        /// </summary>
        /// <param name="responseText">The response text.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="encoding">The encoding.</param>
        public WebResponse(string responseText, string contentType, Encoding encoding)
            : this(responseText, encoding)
        {
            this.ContentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResponse"/> class.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="useStream">Set to <c>true</c> if the stream should be used instead of the response text.</param>
        public WebResponse(string contentType, Encoding encoding, bool useStream = false)
            : this(encoding)
        {
            this.ContentType = contentType;
            this.UseStream = useStream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebResponse"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="useStream">Set to <c>true</c> if the stream should be used instead of the response text.</param>
        public WebResponse(Stream stream, string contentType, Encoding encoding, bool useStream = false)
            : this(contentType, encoding, useStream)
        {
            this.ResponseStream = stream;
        }

        /// <summary>
        /// Gets or sets the response stream.
        /// </summary>
        public Stream ResponseStream { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stream value or the response text should be used.
        /// </summary>
        public bool UseStream { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        private Encoding Encoding { get; set; }

        /// <summary>
        /// Get the response byte array.
        /// </summary>
        /// <returns>Returns the web response as byte array.</returns>
        public byte[] GetResponseBytes()
        {
            byte[] responseTextBytes;

            if (this.UseStream)
            {
                responseTextBytes = this.ResponseStream.ToByteArray();
            }
            else
            {
                responseTextBytes = this.Encoding.GetBytes(this.Response);
            }

            var headersByteArray = this.Encoding.GetBytes(this.AddHttpHeaders(responseTextBytes.Length, this.ContentType, this.Encoding));

            var resultByteArray = new byte[headersByteArray.Length + responseTextBytes.Length];

            Buffer.BlockCopy(headersByteArray, 0, resultByteArray, 0, headersByteArray.Length);
            Buffer.BlockCopy(responseTextBytes, 0, resultByteArray, headersByteArray.Length, responseTextBytes.Length);

            return resultByteArray;
        }

        /// <summary>
        /// Get the response text.
        /// </summary>
        /// <returns>Returns the web response as text.</returns>
        public string GetResponseText()
        {
            var result = string.Empty;

            byte[] responseTextBytes;

            if (this.UseStream)
            {
                responseTextBytes = this.ResponseStream.ToByteArray();
            }
            else
            {
                responseTextBytes = this.Encoding.GetBytes(this.Response);
            }

            result += this.AddHttpHeaders(responseTextBytes.Length, this.ContentType, this.Encoding);

            if (this.UseStream)
            {
                result += this.ResponseStream.ReadStringFromStream();
            }
            else
            {
                result += this.Response;
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Allow multi-line parameters for readability")]
        private string AddHttpHeaders(int contentLength, string mimeType = "text/html", Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            string headers = string.Format(
                CultureInfo.InvariantCulture,
                "HTTP/1.1 {2} {3}\r\n"
                + "Server: SoluiNet.WebServer\r\n"
                + "Content-Type: {1}\r\n"
                + "Accept-Ranges: bytes\r\n"
                + "Content-Length: {0}\r\n"
                + "\r\n",
                contentLength,
                mimeType,
                (int)this.StatusCode,
                this.StatusCode.ToString());

            return headers;
        }
    }
}
