// <copyright file="IHandlesEvent.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin.Events;

    /// <summary>
    /// Provides an interface for event handling.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public interface IHandlesEvent<T>
        where T : IEventType
    {
        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <typeparam name="TEventType">The event type.</typeparam>
        void HandleEvent<TEventType>(Dictionary<string, object> eventArgs)
            where TEventType : IEventType;
    }
}
