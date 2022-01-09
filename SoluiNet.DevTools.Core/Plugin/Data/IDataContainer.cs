// <copyright file="IDataContainer.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides a container for data.
    /// </summary>
    public interface IDataContainer
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        Dictionary<string, object> Data { get; }
    }
}
