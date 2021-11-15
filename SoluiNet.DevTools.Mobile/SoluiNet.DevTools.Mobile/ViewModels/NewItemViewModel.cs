// <copyright file="NewItemViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using System;
    using SoluiNet.DevTools.Mobile.Models;
    using Xamarin.Forms;

    /// <summary>
    /// The model view for new items.
    /// </summary>
    public class NewItemViewModel : BaseViewModel
    {
        private string text;
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewItemViewModel"/> class.
        /// </summary>
        public NewItemViewModel()
        {
            this.SaveCommand = new Command(this.OnSave, this.ValidateSave);
            this.CancelCommand = new Command(this.OnCancel);
            this.PropertyChanged +=
                (_, __) => this.SaveCommand.ChangeCanExecute();
        }

        /// <summary>
        /// Gets or sets the text.
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
        /// Gets the save command.
        /// </summary>
        public Command SaveCommand { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public Command CancelCommand { get; }

        /// <summary>
        /// Validate the save.
        /// </summary>
        /// <returns>Returns true if the data is valid, otherwise false.</returns>
        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(this.text)
                && !string.IsNullOrWhiteSpace(this.description);
        }

        /// <summary>
        /// The cancel event handler.
        /// </summary>
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// The save event handler.
        /// </summary>
        private async void OnSave()
        {
            var newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Text = this.Text,
                Description = this.Description,
            };

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
