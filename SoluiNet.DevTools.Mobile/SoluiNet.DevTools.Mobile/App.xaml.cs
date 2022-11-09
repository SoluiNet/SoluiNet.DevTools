// <copyright file="App.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile
{
    using System.Collections.Generic;
    using SoluiNet.DevTools.Mobile.Services;
    using Xamarin.Forms;

    /// <summary>
    /// The mobile SoluiNet.DevTools app.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            DependencyService.Register<MockDataStore>();
            this.MainPage = new AppShell();
        }

        /// <summary>
        /// Event handler for the start of the application.
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// Event handler for the sleeping of the application.
        /// </summary>
        protected override void OnSleep()
        {
        }

        /// <summary>
        /// Event handler for the resuming of the application.
        /// </summary>
        protected override void OnResume()
        {
        }
    }
}
