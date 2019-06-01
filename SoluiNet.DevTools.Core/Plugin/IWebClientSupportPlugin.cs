// <copyright file="IWebClientSupportPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Enums;

    /// <summary>
    /// Provides an interface for a plugin which provides information for web client usage.
    /// </summary>
    public interface IWebClientSupportPlugin : IBasePlugin
    {
        /// <summary>
        /// Gets the web client format.
        /// </summary>
        WebClientFormatEnum Format { get; }

        /// <summary>
        /// Gets the web client type.
        /// </summary>
        WebClientTypeEnum Type { get; }
    }
}
