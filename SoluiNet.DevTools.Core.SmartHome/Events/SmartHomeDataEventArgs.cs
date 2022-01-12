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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2227:Collection properties should be read only",
            Justification = "We want to set the value while passing a new object. For better readability it should be working without constructor parameters.")]
        public SmartHomeDictionary Data { get; set; }
    }
}