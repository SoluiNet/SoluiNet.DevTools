// <copyright file="SimpleWebServer.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Provides a simple web server (taken from https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server).
    /// Copyright (c) 2013 David's Blog (www.codehosting.net).
    /// </summary>
    public class SimpleWebServer
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> responderMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebServer"/> class.
        /// </summary>
        /// <param name="prefixes">The prefixes.</param>
        /// <param name="method">The method.</param>
        public SimpleWebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
            }

            // URI prefixes are required, for example
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("prefixes");
            }

            foreach (string s in prefixes)
            {
                this.listener.Prefixes.Add(s);
            }

            this.responderMethod = method ?? throw new ArgumentException("method");
            this.listener.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebServer"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="prefixes">The prefixes.</param>
        public SimpleWebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method)
        {
            // do nothing else
        }

        /// <summary>
        /// Run the web server.
        /// </summary>
        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (this.listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(
                            (c) =>
                            {
                                var ctx = c as HttpListenerContext;
                                try
                                {
                                    string rstr = this.responderMethod(ctx.Request);
                                    byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                    ctx.Response.ContentLength64 = buf.Length;
                                    ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                                }
                                catch
                                {
                                    // suppress any exceptions
                                }
                                finally
                                {
                                    // always close the stream
                                    ctx.Response.OutputStream.Close();
                                }
                            },
                            this.listener.GetContext());
                    }
                }
                catch
                {
                    // suppress any exceptions
                }
            });
        }

        /// <summary>
        /// Stop the web server.
        /// </summary>
        public void Stop()
        {
            this.listener.Stop();
            this.listener.Close();
        }
    }
}
