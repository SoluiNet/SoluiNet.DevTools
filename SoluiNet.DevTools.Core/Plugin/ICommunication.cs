// <copyright file="ICommunication.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Plugin.Data;

    /// <summary>
    /// Provides an interface for working with communication data.
    /// </summary>
    public interface ICommunication : IBasePlugin
    {
        /// <summary>
        /// Gets the communication channel.
        /// </summary>
        string CommunicationChannel { get; }

        /// <summary>
        /// Gets the supported entities.
        /// </summary>
        ICollection<string> SupportedCommunicationEntities { get; }
    }
}
