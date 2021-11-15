// <copyright file="WebClientType.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An enumeration which provides a list of all possible web client types.
    /// </summary>
    public enum WebClientType
    {
        /// <summary>
        /// Represents a unknown client.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents a REST-client
        /// </summary>
        Rest = 1,

        /// <summary>
        /// Represents a SOAP-client
        /// </summary>
        Soap = 2,
    }
}
