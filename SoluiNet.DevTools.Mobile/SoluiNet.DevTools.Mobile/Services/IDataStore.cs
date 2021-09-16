// <copyright file="IDataStore.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The data store.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public interface IDataStore<T>
    {
        /// <summary>
        /// Add an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns an asynchronous task which will return a bool value if the process was successful.</returns>
        Task<bool> AddItemAsync(T item);

        /// <summary>
        /// Update an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns an asynchronous task which will return a bool value if the process was successful.</returns>
        Task<bool> UpdateItemAsync(T item);

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>Returns an asynchronous task which will return a bool value if the process was successful.</returns>
        Task<bool> DeleteItemAsync(string id);

        /// <summary>
        /// Get an item by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>Returns an asynchronous task which will return the queried item.</returns>
        Task<T> GetItemAsync(string id);

        /// <summary>
        /// Get all items.
        /// </summary>
        /// <param name="forceRefresh">Provide true as value if you want to force a refresh.</param>
        /// <returns>Returns an asynchronous task which will return a enumerable of all items.</returns>
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
