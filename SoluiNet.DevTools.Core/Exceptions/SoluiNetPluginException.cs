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
    public class SoluiNetPluginException : SoluiNetException
    {
        /// <inheritdoc cref="SoluiNetException"/>
        public SoluiNetPluginException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="SoluiNetException"/>
        public SoluiNetPluginException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc cref="SoluiNetException"/>
        public SoluiNetPluginException()
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
        /// Gets or sets the plugin name.
        /// </summary>
        private string Plugin { get; set; }
    }
}
