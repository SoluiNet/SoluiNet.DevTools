// <copyright file="WebClientTypeEnum.cs" company="SoluiNet">
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "It should be clear that this type is an enum.")]
    public enum WebClientTypeEnum
    {
        /// <summary>
        /// Represents a client without more specifics.
        /// </summary>
        None = 0,

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
