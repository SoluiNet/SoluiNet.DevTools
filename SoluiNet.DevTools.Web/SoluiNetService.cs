// <copyright file="SoluiNetService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Tools.Object;
    using SoluiNet.DevTools.Core.Web.Renderer;

    /// <summary>
    /// The SoluiNet service.
    /// </summary>
    public partial class SoluiNetService : ServiceBase
    {
        private SoluiNetWebServer webServer = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetService"/> class.
        /// </summary>
        public SoluiNetService()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The start event handler.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnStart(string[] args)
        {
            this.webServer = new SoluiNetWebServer();

            this.webServer.HandleRequest += (webRequest, webArgs) =>
            {
                if (webRequest.Method == HttpMethod.Get)
                {
                    if (webRequest.Route.StartsWith("/favicon.ico"))
                    {
                        return new Core.Web.Communication.WebResponse(
                            this.GetEmbeddedResourceContentStream("favicon.ico", string.Empty),
                            "image/x-icon",
                            Encoding.UTF8,
                            true);
                    }
                    else if (webRequest.Route.StartsWith("/Status"))
                    {
                        return new Core.Web.Communication.WebResponse(
                            WebRenderer.RenderPage(string.Format("Status: {0}\r\nVersion: {1}", "OK", this.GetType().Assembly.GetName().Version.ToString())),
                            Encoding.UTF8);
                    }
                    else
                    {
                        return new Core.Web.Communication.WebResponse(WebRenderer.RenderPage("not found"), Encoding.UTF8) { StatusCode = System.Net.HttpStatusCode.NotFound };
                    }
                }

                return new Core.Web.Communication.WebResponse(WebRenderer.RenderPage("no supported method"), Encoding.UTF8) { StatusCode = System.Net.HttpStatusCode.MethodNotAllowed };
            };

            this.webServer.Start();
        }

        /// <summary>
        /// The stop event handler.
        /// </summary>
        protected override void OnStop()
        {
            if (this.webServer != null)
            {
                this.webServer.Stop();
            }
        }
    }
}
