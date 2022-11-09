// <copyright file="ICommunicationSender.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Plugin.Data;

    /// <summary>
    /// Provides an interface for sending communication data.
    /// </summary>
    public interface ICommunicationSender : ICommunication
    {
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="entity">The entity (optional). If empty it should send data for all entities.</param>
        /// <param name="receiver">The receiver (optional). If empty it should be sent to all registered receivers.</param>
        /// <param name="additionalParameters">The additional parameters (optional).</param>
        void Send(string message, string entity = "", string receiver = "", Dictionary<string, object> additionalParameters = null);
    }
}
