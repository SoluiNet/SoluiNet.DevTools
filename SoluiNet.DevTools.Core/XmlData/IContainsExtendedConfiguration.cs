// <copyright file="IContainsExtendedConfiguration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.XmlData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for elements which are working with extended configurations.
    /// </summary>
    public interface IContainsExtendedConfiguration
    {
        /// <summary>
        /// Gets or sets the extended configuration.
        /// </summary>
        string ExtendedConfiguration { get; set; }
    }
}
