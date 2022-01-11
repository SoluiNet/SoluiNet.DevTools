// <copyright file="SmartHomeDataEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SmartHome.Events
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.SmartHome.Data;

    /// <summary>
    /// The arguments for smart home data events.
    /// </summary>
    public class SmartHomeDataEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public SmartHomeDictionary Data { get; set; }
    }
}
