// <copyright file="IAllowsDataExchange.cs" company="SoluiNet">
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
    public interface IAllowsDataExchange : IBasePlugin
    {
        /// <summary>
        /// Get a data object for general purposes.
        /// </summary>
        /// <returns>Returns a <see cref="Dictionary{TKey, TValue}"/> which holds a key-value combination of data for general purposes.</returns>
        IDictionary<string, object> GetGeneralData();

        /// <summary>
        /// Get data via a predefined filter string.
        /// </summary>
        /// <param name="whereClause">The filter string.</param>
        /// <returns>Returns a <see cref="List{T}">List</see> which matches the filter string.</returns>
        ICollection<object> GetData(string whereClause);

        /// <summary>
        /// Get data via a predefined filter dictionary.
        /// </summary>
        /// <param name="entityName">The entity which should be filtered.</param>
        /// <param name="searchData">The filter dictionary.</param>
        /// <returns>Returns a <see cref="List{T}"/> which matches the filter dictionary and entity.</returns>
        ICollection<object> GetData(string entityName, IDictionary<string, object> searchData);

        /// <summary>
        /// Set data for an identifiable element.
        /// </summary>
        /// <param name="identifier">The element identification.</param>
        /// <param name="valueData">The data which should be set.</param>
        /// <returns>Returns the upgraded element.</returns>
        object SetData(object identifier, IDictionary<string, object> valueData);
    }
}
