// <copyright file="SoluiNetWebServer.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;
    using SoluiNet.DevTools.Core.Tools.Object;

    /// <summary>
    /// The SoluiNet Web Server.
    /// </summary>
    public class SoluiNetWebServer
    {
        private readonly TcpListener webListener;
        private Thread webCommunicationHandlerThread;

        private bool started = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetWebServer"/> class.
        /// </summary>
        public SoluiNetWebServer()
        {
            try
            {
                var ipAddress = Dns.GetHostEntry("localhost").AddressList.First(x => x.AddressFamily != AddressFamily.InterNetworkV6);

                Console.WriteLine("Starting SoluiNetWebServer, listening on {0}:{1}...", ipAddress.ToString(), 31337);
                this.webListener = new TcpListener(ipAddress, 31337);
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer", null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the web server has been started.
        /// </summary>
        public bool Started
        {
            get
            {
                return this.started;
            }
        }

        private Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <summary>
        /// Start web server.
        /// </summary>
        public void Start()
        {
            if (this.started)
            {
                throw new Exception("SoluiNetWebServer already started.");
            }

            try
            {
                this.webListener.Start();

                this.webCommunicationHandlerThread = new Thread(new ThreadStart(this.HandleWebCommunication));
                this.webCommunicationHandlerThread.Start();

                Console.WriteLine("SoluiNetWebServer started.");

                this.started = true;
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer [Start]", null);
                throw;
            }
        }

        /// <summary>
        /// Start web server.
        /// </summary>
        public void Stop()
        {
            if (!this.started)
            {
                throw new Exception("SoluiNetWebServer can't be stopped if it hasn't been started.");
            }

            try
            {
                this.webCommunicationHandlerThread.Abort();

                this.webListener.Stop();

                Console.WriteLine("SoluiNetWebServer stopped...");

                this.started = false;
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer [Stop]", null);
                throw;
            }
        }

        private void AddHttpHeaders(int contentLength, ref Socket respondingSocket, string mimeType = "text/html")
        {
            try
            {
                if (respondingSocket.Connected)
                {
                    string headers = string.Format(
                        "HTTP/1.1\r\n"
                        + "Server: localhost\r\n"
                        + "Content-Type: {1}\r\n"
                        + "Accept-Ranges: bytes\r\n"
                        + "Content-Length: {0}\r\n"
                        + "\r\n",
                        contentLength,
                        mimeType);

                    var headerBytes = Encoding.UTF8.GetBytes(headers);

                    respondingSocket.Send(headerBytes, headerBytes.Length, SocketFlags.None);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer [AddHeaders]", null);
            }
        }

        private void Respond(string returningString, ref Socket respondingSocket, string mimeType = "text/html")
        {
            try
            {
                if (respondingSocket.Connected)
                {
                    var returningBytes = Encoding.UTF8.GetBytes(returningString);

                    this.AddHttpHeaders(returningBytes.Length, ref respondingSocket, mimeType);

                    respondingSocket.Send(returningBytes, returningBytes.Length, SocketFlags.None);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer [Respond]", null);
            }
        }

        private void HandleWebCommunication()
        {
            try
            {
                while (true)
                {
                    Socket webSocket = this.webListener.AcceptSocket();

                    if (webSocket.Connected)
                    {
                        var receivingBytes = new byte[1024];

                        webSocket.Receive(receivingBytes, receivingBytes.Length, SocketFlags.None);

                        var receivingString = Encoding.UTF8.GetString(receivingBytes);

                        if (receivingString.StartsWith("GET /Status"))
                        {
                            this.Respond(string.Format("Status: {0}\r\nVersion: {1}", "OK", this.GetType().Assembly.GetName().Version.ToString()), ref webSocket);
                        }

                        if (receivingString.StartsWith("GET /favicon.ico"))
                        {
                            this.Respond(this.GetEmbeddedResourceContent("favicon.ico"), ref webSocket, "image/x-icon");
                        }

                        webSocket.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer [HandleRequest]", null);
            }
        }
    }
}
