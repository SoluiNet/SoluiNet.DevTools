// <copyright file="IEventType.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the base interface for all events which can be handled in an plugin.
    /// </summary>
    public interface IEventType
    {
        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        void HandleEvent(Dictionary<string, object> eventArgs);
    }
}
