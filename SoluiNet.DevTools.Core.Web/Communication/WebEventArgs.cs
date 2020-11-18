// <copyright file="WebEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Web.Communication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A class which represents arguments for handling a web communication.
    /// </summary>
    public class WebEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the event has been handled or not.
        /// </summary>
        public bool Handled { get; set; }
    }
}
