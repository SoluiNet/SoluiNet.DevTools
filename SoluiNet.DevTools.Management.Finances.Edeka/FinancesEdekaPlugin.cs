// <copyright file="FinancesEdekaPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Edeka
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Security.Principal;
    using System.Windows;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SoluiNet.DevTools.Core.Application;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
    using System.Windows.Media;
#endif
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Reference;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
    using SoluiNet.DevTools.Core.UI.Blazor.Reference;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Extensions;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Reference;
#endif

    /// <summary>
    /// Provides a plugin that allows one to manage finance information from EDEKA.
    /// </summary>
    public class FinancesEdekaPlugin : IManagementPlugin, IManagementUiPlugin
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return "FinancesEdekaPlugin"; }
        }

        /// <summary>
        /// Gets the first accent colour.
        /// </summary>
        public IColour AccentColour1
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Yellow"); }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public IColour AccentColour2
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Blue"); }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public IColour ForegroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("White"); }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public IColour BackgroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Black"); }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public IColour BackgroundAccentColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DimGray"); }
        }

        /// <inheritdoc />
        public Dictionary<string, ICollection<object>> Resources
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

#if BUILD_FOR_WINDOWS
        /// <summary>
        /// Gets or sets the main grid.
        /// </summary>
        private Grid MainGrid { get; set; }

        /// <summary>
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            this.MainGrid = mainGrid;

            var tabControl = mainGrid.GetChildOfType<TabControl>();

            if (tabControl.Name == "MainTabs")
            {
                var tabItem = new TabItem()
                {
                    Header = "Edeka",
                    Name = "Finances_Edeka_TabItem",
                    Background = new LinearGradientBrush(this.AccentColour1, this.AccentColour2, 0.00),
                    Foreground = new SolidColorBrush(this.ForegroundColour),
                };

                tabControl.SelectionChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.Source is TabControl)
                    {
                        if (tabItem.IsSelected)
                        {
                            tabControl.Background = new SolidColorBrush(this.BackgroundColour);
                        }
                    }
                };

                tabControl.Items.Add(tabItem);

                tabItem.Content = new Grid()
                {
                    Name = "Finances_Edeka_TabItem_Content",
                    Background = new LinearGradientBrush(this.BackgroundAccentColour, this.BackgroundColour, 45.00),
                };

                ((Grid)tabItem.Content).Children.Add(new FinancesEdekaUserControl());
            }
        }
#endif
    }
}