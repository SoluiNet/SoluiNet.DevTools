// <copyright file="SmartHomeDataObserver.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SmartHome
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NLog;
    using SoluiNet.DevTools.Core.SmartHome.Data;
    using SoluiNet.DevTools.Core.SmartHome.Events;

    /// <summary>
    /// Observer for smart home data.
    /// </summary>
    public class SmartHomeDataObserver : IObserver<SmartHomeDictionary>
    {
        /// <summary>
        /// The event handler for receiving of new data.
        /// </summary>
        public event EventHandler<SmartHomeDataEventArgs> NewDataReceived;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        /// <summary>
        /// Call if the observing object has nothing to report anymore.
        /// </summary>
        public void OnCompleted()
        {
            Logger.Trace("Core.SmartHome - Nothing to report anymore");
        }

        /// <summary>
        /// An unexpected error has occured.
        /// </summary>
        /// <param name="error">The error.</param>
        public void OnError(Exception error)
        {
            Logger.Error(error, "Core.SmartHome - An unexpected error has occured.");
        }

        /// <summary>
        /// Get next value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnNext(SmartHomeDictionary value)
        {
            Logger.Trace("Core.SmartHome - Next smart home data");

            this.NewDataReceived?.Invoke(this, new SmartHomeDataEventArgs() { Data = value });
        }
    }
}
