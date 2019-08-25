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
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

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
