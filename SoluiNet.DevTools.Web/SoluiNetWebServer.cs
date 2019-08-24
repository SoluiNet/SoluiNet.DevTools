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

    /// <summary>
    /// The SoluiNet Web Server.
    /// </summary>
    public class SoluiNetWebServer
    {
        private TcpListener webListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetWebServer"/> class.
        /// </summary>
        public SoluiNetWebServer()
        {
            try
            {
                var ipAddress = Dns.GetHostEntry("localhost").AddressList.First();

                this.webListener = new TcpListener(ipAddress, 31337);
                this.webListener.Start();

                Console.WriteLine("SoluiNetWebServer started...");

                var thread = new Thread(new ThreadStart(this.HandleWebCommunication));
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured in SoluiNetWebServer", null);
            }
        }

        private Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        private void Respond(string returningString, ref Socket respondingSocket)
        {
            try
            {
                if (respondingSocket.Connected)
                {
                    var returningBytes = Encoding.UTF8.GetBytes(returningString);

                    respondingSocket.Send(returningBytes, returningBytes.Length, SocketFlags.None);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "An exception occured while responding in SoluiNetWebServer", null);
            }
        }

        private void HandleWebCommunication()
        {
            while (true)
            {
                Socket webSocket = this.webListener.AcceptSocket();

                if (webSocket.Connected)
                {
                    var receivingBytes = new byte[1024];

                    webSocket.Receive(receivingBytes, receivingBytes.Length, SocketFlags.None);

                    var receivingString = Encoding.UTF8.GetString(receivingBytes);

                    if (receivingString.StartsWith("GET Status"))
                    {
                        this.Respond(string.Format("Status: {0}\r\nVersion: {1}", "OK", this.GetType().Assembly.GetName().Version.ToString()), ref webSocket);
                    }

                    webSocket.Close();
                }
            }
        }
    }
}
