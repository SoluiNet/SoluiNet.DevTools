// <copyright file="IDataExchangePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for a plugin which allows a data exchange.
    /// </summary>
    public interface IDataExchangePlugin : IBasePlugin
    {
        /// <summary>
        /// Get data via a predefined filter string.
        /// </summary>
        /// <param name="whereClause">The filter string.</param>
        /// <returns>Returns a <see cref="List{object}">List</see> which matches the filter string.</returns>
        List<object> GetData(string whereClause); // use dynamic LINQ?

        /// <summary>
        /// Get data via a predefined filter dictionary.
        /// </summary>
        /// <param name="entityName">The entity which should be filtered.</param>
        /// <param name="searchData">The filter dictionary.</param>
        /// <returns>Returns a <see cref="List{object}"/> which matches the filter dictionary and entity.</returns>
        List<object> GetData(string entityName, IDictionary<string, object> searchData);

        /// <summary>
        /// Set data for an identifiable element.
        /// </summary>
        /// <param name="identifier">The element identification.</param>
        /// <param name="valueData">The data which should be set.</param>
        /// <returns>Returns the upgraded element.</returns>
        object SetData(object identifier, IDictionary<string, object> valueData);
    }
}
