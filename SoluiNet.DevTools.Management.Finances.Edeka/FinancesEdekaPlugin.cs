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
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Extensions;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

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
        public Color AccentColour1
        {
            get { return Colors.Yellow; }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public Color AccentColour2
        {
            get { return Colors.Blue; }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public Color ForegroundColour
        {
            get { return Colors.White; }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public Color BackgroundColour
        {
            get { return Colors.Black; }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public Color BackgroundAccentColour
        {
            get { return Colors.DimGray; }
        }

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
    }
}