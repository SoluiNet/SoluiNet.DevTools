// <copyright file="AboutViewModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile.ViewModels
{
    using System.Windows.Input;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    /// <summary>
    /// The about view model.
    /// </summary>
    public class AboutViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        public AboutViewModel()
        {
            this.Title = "About";
            this.OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        /// <summary>
        /// Gets the open web command.
        /// </summary>
        public ICommand OpenWebCommand { get; }
    }
}