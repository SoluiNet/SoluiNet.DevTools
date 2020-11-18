// <copyright file="SoluiNetPluginException.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// An exception type for SoluiNet Plugins.
    /// </summary>
    [Serializable]
    public class SoluiNetPluginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        public SoluiNetPluginException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SoluiNetPluginException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="plugin">The plugin.</param>
        public SoluiNetPluginException(string message, string plugin)
            : base(message)
        {
            this.Plugin = plugin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="plugin">The plugin object.</param>
        public SoluiNetPluginException(string message, IBasePlugin plugin)
            : base(message)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            this.Plugin = plugin.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SoluiNetPluginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="plugin">The plugin.</param>
        public SoluiNetPluginException(string message, Exception innerException, string plugin)
            : base(message, innerException)
        {
            this.Plugin = plugin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="plugin">The plugin object.</param>
        public SoluiNetPluginException(string message, Exception innerException, IBasePlugin plugin)
            : base(message, innerException)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            this.Plugin = plugin.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetPluginException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected SoluiNetPluginException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the plugin name.
        /// </summary>
        private string Plugin { get; set; }
    }
}
