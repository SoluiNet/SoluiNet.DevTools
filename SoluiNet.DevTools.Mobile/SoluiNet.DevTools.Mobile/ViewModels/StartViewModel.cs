// <copyright file="StartViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using SoluiNet.DevTools.Mobile.Views;
    using Xamarin.Forms;

    /// <summary>
    /// The start view model.
    /// </summary>
    public class StartViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartViewModel"/> class.
        /// </summary>
        public StartViewModel()
        {
            this.AboutCommand = new Command(this.OnAboutClicked);
        }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public Command AboutCommand { get; }

        /// <summary>
        /// The event handler for the click of the about element.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OnAboutClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
    }
}
