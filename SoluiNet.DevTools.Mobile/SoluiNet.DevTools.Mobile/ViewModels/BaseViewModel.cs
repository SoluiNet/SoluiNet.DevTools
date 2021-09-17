// <copyright file="BaseViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using SoluiNet.DevTools.Mobile.Models;
    using SoluiNet.DevTools.Mobile.Services;
    using Xamarin.Forms;

    /// <summary>
    /// The base view model.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool isBusy = false;
        private string title = string.Empty;

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the data store.
        /// </summary>
        public IDataStore<Item> DataStore
        {
            get { return DependencyService.Get<IDataStore<Item>>(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is busy.
        /// </summary>
        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.SetProperty(ref this.isBusy, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <typeparam name="T">The data type.</typeparam>
        /// <param name="backingStore">The data store which holds the data.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="onChanged">The on change event handler.</param>
        /// <returns>Returns true if the property value has been successfully changed.</returns>
        protected bool SetProperty<T>(
            ref T backingStore,
            T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// The property change event handler.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = this.PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
