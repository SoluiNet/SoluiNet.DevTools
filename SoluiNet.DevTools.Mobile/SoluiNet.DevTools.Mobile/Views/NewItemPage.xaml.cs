// <copyright file="NewItemPage.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.Views
{
    using SoluiNet.DevTools.Mobile.Models;
    using SoluiNet.DevTools.Mobile.ViewModels;
    using Xamarin.Forms;

    /// <summary>
    /// The new item page.
    /// </summary>
    public partial class NewItemPage : ContentPage
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NewItemPage"/> class.
        /// </summary>
        public NewItemPage()
        {
            this.InitializeComponent();
            this.BindingContext = new NewItemViewModel();
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public Item Item { get; set; }
    }
}