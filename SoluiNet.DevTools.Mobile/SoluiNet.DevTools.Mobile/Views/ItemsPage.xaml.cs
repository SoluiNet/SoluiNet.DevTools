// <copyright file="ItemsPage.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.Views
{
    using SoluiNet.DevTools.Mobile.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// The items page.
    /// </summary>
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsPage"/> class.
        /// </summary>
        public ItemsPage()
        {
            this.InitializeComponent();
            this.BindingContext = this.viewModel = new ItemsViewModel();
        }

        /// <summary>
        /// The appearing event handler.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.viewModel.OnAppearing();
        }
    }
}