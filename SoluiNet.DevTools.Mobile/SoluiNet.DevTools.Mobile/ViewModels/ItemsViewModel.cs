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
        private Item _selectedItem;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }

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

        async Task ExecuteLoadItemsCommand()
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

        public void OnAppearing()
        {
            this.IsBusy = true;
            this.SelectedItem = null;
        }

        public Item SelectedItem
        {
            get { return this._selectedItem; }
            set
            {
                this.SetProperty(ref this._selectedItem, value);
                this.OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}