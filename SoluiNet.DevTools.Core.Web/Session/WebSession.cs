// <copyright file="WebSession.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The session.
    /// </summary>
    public class WebSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSession"/> class.
        /// </summary>
        public WebSession()
        {
            this.Storage = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the storage.
        /// </summary>
        public Dictionary<string, object> Storage { get; private set; }
    }
}
