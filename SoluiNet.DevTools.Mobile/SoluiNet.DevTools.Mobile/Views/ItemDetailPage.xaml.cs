// <copyright file="ItemDetailPage.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.Views
{
    using SoluiNet.DevTools.Mobile.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// The item detail page.
    /// </summary>
    public partial class ItemDetailPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDetailPage"/> class.
        /// </summary>
        public ItemDetailPage()
        {
            this.InitializeComponent();
            this.BindingContext = new ItemDetailViewModel();
        }
    }
}