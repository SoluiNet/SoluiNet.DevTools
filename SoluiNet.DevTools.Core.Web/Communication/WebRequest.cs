// <copyright file="WebRequest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// A class which represents a web request.
    /// </summary>
    public class WebRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequest"/> class.
        /// </summary>
        /// <param name="webRequestString">The web request string.</param>
        public WebRequest(string webRequestString)
        {
            var requestRegEx = new Regex("(GET|POST|PUT|DELETE)\\s+(.+?)\\s+");

            var regExMatch = requestRegEx.Match(webRequestString);

            if (!regExMatch.Success)
            {
                throw new Exception("no valid HTTP request");
            }

            if (regExMatch.Groups[1].Success && regExMatch.Groups[1].Value.Equals("GET"))
            {
                this.Method = HttpMethod.Get;
            }

            if (regExMatch.Groups[2].Success)
            {
                this.Route = regExMatch.Groups[2].Value;
            }
        }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// Gets the route.
        /// </summary>
        public string Route { get; private set; }
    }
}
