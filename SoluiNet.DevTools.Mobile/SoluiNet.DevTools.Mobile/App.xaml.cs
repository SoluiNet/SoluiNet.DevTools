// <copyright file="App.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Mobile
{
    using System.Collections.Generic;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Mobile.Services;
    using Xamarin.Forms;

    /// <summary>
    /// The mobile SoluiNet.DevTools app.
    /// </summary>
    public partial class App : Application, ISoluiNetApp, IHoldsBaseApp
    {
        private BaseSoluiNetApp baseApp;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            this.baseApp = new BaseSoluiNetMobileApp();

            ApplicationContext.Application = this;

            DependencyService.Register<MockDataStore>();
            this.MainPage = new AppShell();

            this.baseApp.Initialize();
        }

        /// <inheritdoc/>
        public ICollection<IBasePlugin> Plugins
        {
            get
            {
                return this.baseApp.Plugins;
            }
        }

        /// <inheritdoc/>
        public ICollection<IRunsBackgroundTask> BackgroundTaskPlugins
        {
            get
            {
                return this.baseApp.BackgroundTaskPlugins;
            }
        }

        /// <inheritdoc/>
        public BaseSoluiNetApp BaseApp
        {
            get
            {
                return this.baseApp;
            }
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
