// <copyright file="ICommunicationReceiver.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Plugin.Data;

    /// <summary>
    /// Provides an interface for receiving communication data.
    /// </summary>
    public interface ICommunicationReceiver : ICommunication
    {
        /// <summary>
        /// Receive messages from the communication channel and handle them.
        /// </summary>
        /// <param name="entity">The entity (optional). If empty it will receive data for all entities.</param>
        void Receive(string entity = "");
    }
}
