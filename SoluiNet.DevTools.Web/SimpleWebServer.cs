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
    using NLog;

    /// <summary>
    /// Provides a simple web server (taken from https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server).
    /// Copyright (c) 2013 David's Blog (www.codehosting.net).
    /// </summary>
    public class SimpleWebServer : IDisposable
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
                throw new ArgumentException("Invalid prefixes", nameof(prefixes));
            }

            foreach (string s in prefixes)
            {
                this.listener.Prefixes.Add(s);
            }

            this.responderMethod = method ?? throw new ArgumentNullException(nameof(method));
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

        private static Logger Logger
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        /// <summary>
        /// Run the web server.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
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
                                    var rstr = this.responderMethod(ctx.Request);
                                    var buf = Encoding.UTF8.GetBytes(rstr);
                                    ctx.Response.ContentLength64 = buf.Length;
                                    ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                                }
                                catch (Exception exception)
                                {
                                    Logger.Fatal(exception);
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
                catch (Exception exception)
                {
                    Logger.Fatal(exception);
                }
            });
        }

        /// <summary>
        /// Dispose the SimpleWebServer instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);  // Violates rule
        }

        /// <summary>
        /// Stop the web server.
        /// </summary>
        public void Stop()
        {
            this.listener.Stop();
            this.listener.Close();
        }

        /// <summary>
        /// Dispose the SimpleWebServer instance.
        /// </summary>
        /// <param name="disposing">A value indicating whether the object is being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.listener != null)
                {
                    ((IDisposable)this.listener).Dispose();
                }
            }
        }
    }
}
