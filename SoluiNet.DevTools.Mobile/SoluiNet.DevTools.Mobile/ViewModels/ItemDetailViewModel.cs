// <copyright file="ItemDetailViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using System;
    using System.Diagnostics;
    using Xamarin.Forms;

    /// <summary>
    /// The item detail view model.
    /// </summary>
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set { this.SetProperty(ref this.text, value); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        public string ItemId
        {
            get
            {
                return this.itemId;
            }

            set
            {
                this.itemId = value;
                this.LoadItemId(value);
            }
        }

        /// <summary>
        /// Load item by item ID.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await this.DataStore.GetItemAsync(itemId);
                this.Id = item.Id;
                this.Text = item.Text;
                this.Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
