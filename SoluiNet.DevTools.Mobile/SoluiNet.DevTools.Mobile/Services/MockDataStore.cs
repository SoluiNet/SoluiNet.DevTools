// <copyright file="MockDataStore.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Mobile.Models;

    /// <summary>
    /// The mock data store.
    /// </summary>
    public class MockDataStore : IDataStore<Item>
    {
        private readonly List<Item> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockDataStore"/> class.
        /// </summary>
        public MockDataStore()
        {
            this.items = new List<Item>()
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description = "This is an item description." },
            };
        }

        /// <summary>
        /// Adds an item to the data store.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item has been successfully added.</returns>
        public async Task<bool> AddItemAsync(Item item)
        {
            this.items.Add(item);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Updates an item from the data store.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item has been successfully updated.</returns>
        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = this.items.FirstOrDefault(arg => arg.Id == item.Id);
            this.items.Remove(oldItem);
            this.items.Add(item);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Deletes an item from the data store.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <returns>Returns true if the item has been successfully deleted.</returns>
        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = this.items.FirstOrDefault(arg => arg.Id == id);
            this.items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Ges an item from the data store.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <returns>Returns the item for the passed ID.</returns>
        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(this.items.FirstOrDefault(s => s.Id == id));
        }

        /// <summary>
        /// Gets all items from the data store.
        /// </summary>
        /// <param name="forceRefresh">Set to true if you want to force a refresh.</param>
        /// <returns>Returns an enumerable of items.</returns>
        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(this.items);
        }
    }
}