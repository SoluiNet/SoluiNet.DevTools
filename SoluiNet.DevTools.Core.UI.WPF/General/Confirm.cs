// <copyright file="Confirm.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.General
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core.UI.WPF.Window;

    /// <summary>
    /// Provides methods to display a confirm window.
    /// </summary>
    public static class Confirm
    {
        /// <summary>
        /// Show a confirm window.
        /// </summary>
        /// <param name="text">The text which should be displayed for the confirm.</param>
        /// <param name="caption">The window caption.</param>
        /// <returns>Returns true if the confirmation button has been clicked in the window, otherwise false.</returns>
        public static bool ShowDialog(string text, string caption)
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            var prompt = new SoluiNetWindow()
            {
                Width = 500,
                Height = 150,
                TitleFormatString = "Dialog ({0}) - SoluiNet.DevTools",
                Title = string.Format("Dialog ({0}) - SoluiNet.DevTools", caption),
                Left = (screenWidth / 2) - 250,
                Top = (screenHeight / 2) - 75,
            };

            var mainGrid = new Grid();

            prompt.Content = mainGrid;

            var textLabel = new Label() { Margin = new Thickness(50, 20, 0, 0), Content = text, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            var confirmation = new Button() { Content = "Yes", Margin = new Thickness(50, 70, 0, 0), Width = 100, IsDefault = true, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var declination = new Button() { Content = "No", Margin = new Thickness(30, 70, 0, 0), Width = 100, IsCancel = true, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            confirmation.Click += (sender, e) =>
            {
                prompt.DialogResult = true;
                prompt.Close();
            };

            declination.Click += (sender, e) =>
            {
                prompt.DialogResult = false;
                prompt.Close();
            };

            mainGrid.Children.Add(textLabel);
            mainGrid.Children.Add(confirmation);
            mainGrid.Children.Add(declination);

            var showDialogResult = prompt.ShowDialog();

            return showDialogResult.HasValue && showDialogResult.Value;
        }
    }
}
