// <copyright file="AppShell.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile
{
    using System;
    using SoluiNet.DevTools.Mobile.Views;
    using Xamarin.Forms;

    /// <summary>
    /// Provides an app shell.
    /// </summary>
    public partial class AppShell : Xamarin.Forms.Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppShell"/> class.
        /// </summary>
        public AppShell()
        {
            this.InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
