// <copyright file="ItemsViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Mobile.Models;
    using SoluiNet.DevTools.Mobile.Views;
    using Xamarin.Forms;

    /// <summary>
    /// The items view model.
    /// </summary>
    public class ItemsViewModel : BaseViewModel
    {
        private Item selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsViewModel"/> class.
        /// </summary>
        public ItemsViewModel()
        {
            this.Title = "Browse";
            this.Items = new ObservableCollection<Item>();
            this.LoadItemsCommand = new Command(async () => await this.ExecuteLoadItemsCommand());

            this.ItemTapped = new Command<Item>(this.OnItemSelected);

            this.AddItemCommand = new Command(this.OnAddItem);
        }

        /// <summary>
        /// Gets the items collection.
        /// </summary>
        public ObservableCollection<Item> Items { get; }

        /// <summary>
        /// Gets the load items command.
        /// </summary>
        public Command LoadItemsCommand { get; }

        /// <summary>
        /// Gets the add item command.
        /// </summary>
        public Command AddItemCommand { get; }

        /// <summary>
        /// Gets the item which has been tapped.
        /// </summary>
        public Command<Item> ItemTapped { get; }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public Item SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                this.SetProperty(ref this.selectedItem, value);
                this.OnItemSelected(value);
            }
        }

        /// <summary>
        /// The on appearing event handler.
        /// </summary>
        public void OnAppearing()
        {
            this.IsBusy = true;
            this.SelectedItem = null;
        }

        /// <summary>
        /// The execute load items command.
        /// </summary>
        /// <returns>Returns an asynchronous task which will load the items.</returns>
        private async Task ExecuteLoadItemsCommand()
        {
            this.IsBusy = true;

            try
            {
                this.Items.Clear();
                var items = await this.DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    this.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// The add item event handler.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        /// <summary>
        /// The item selected event handler.
        /// </summary>
        /// <param name="item">The item.</param>
        private async void OnItemSelected(Item item)
        {
            if (item == null)
            {
                return;
            }

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}