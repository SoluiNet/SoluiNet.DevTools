// <copyright file="IAllowsGenericDataExchange.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for a plugin which allows a data exchange.
    /// </summary>
    /// <typeparam name="T">The type which can be exchanged.</typeparam>
    public interface IAllowsGenericDataExchange<T> : IAllowsDataExchange
    {
        /// <summary>
        /// Get data.
        /// </summary>
        /// <returns>Returns data of type <typeparamref name="T"/>.</returns>
        T GetGenericData();

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns data of type <typeparamref name="T"/> for the <paramref name="key"/>.</returns>
        T GetGenericDataByKey(string key);
    }
}
