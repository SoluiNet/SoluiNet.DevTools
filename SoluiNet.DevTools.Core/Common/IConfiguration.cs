// <copyright file="IConfiguration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides an interface for configurations.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Get a configuration entry by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="plugin">The plugin name.</param>
        /// <param name="environment">The environment name.</param>
        /// <returns>Returns an <see cref="object"/> with the value of the configuration entry. May need casting to right data type.</returns>
        object GetByKey(string key, string plugin = "", string environment = "");
    }
}
