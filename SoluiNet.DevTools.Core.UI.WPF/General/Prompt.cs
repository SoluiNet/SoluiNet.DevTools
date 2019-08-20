// <copyright file="Prompt.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.UI.Window;

    /// <summary>
    /// Provides methods to display a prompt window.
    /// </summary>
    public static class Prompt
    {
        /// <summary>
        /// Show a prompt dialog.
        /// </summary>
        /// <param name="text">The text which should be displayed for the prompt.</param>
        /// <param name="caption">The window caption.</param>
        /// <returns>Returns the value which has been provided to the prompt window.</returns>
        public static string ShowDialog(string text, string caption)
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
            var textBox = new TextBox() { Margin = new Thickness(50, 50, 0, 0), Width = 400, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var confirmation = new Button() { Content = "Ok", Margin = new Thickness(350, 70, 0, 0), Width = 100, IsDefault = true, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            confirmation.Click += (sender, e) =>
            {
                prompt.DialogResult = true;
                prompt.Close();
            };

            mainGrid.Children.Add(textLabel);
            mainGrid.Children.Add(textBox);
            mainGrid.Children.Add(confirmation);

            var showDialogResult = prompt.ShowDialog();

            return showDialogResult.HasValue && showDialogResult.Value ? textBox.Text : string.Empty;
        }
    }
}
