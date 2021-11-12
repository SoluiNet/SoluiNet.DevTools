// <copyright file="LoginViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using SoluiNet.DevTools.Mobile.Views;
    using Xamarin.Forms;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel()
        {
            this.LoginCommand = new Command(this.OnLoginClicked);
        }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public Command LoginCommand { get; }

        /// <summary>
        /// The event handler for the click of the login element.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
    }
}
