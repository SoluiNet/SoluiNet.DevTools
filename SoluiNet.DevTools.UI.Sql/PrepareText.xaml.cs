// <copyright file="PrepareText.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.Window;

    /// <summary>
    /// Interaction logic for PrepareText.xaml.
    /// </summary>
    public partial class PrepareText : SoluiNetWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrepareText"/> class.
        /// </summary>
        public PrepareText()
        {
            this.InitializeComponent();
        }

        private void AdjustText_Click(object sender, RoutedEventArgs e)
        {
            var outputText = this.Input.Text;

            if (this.IsRegex.IsChecked.HasValue && this.IsRegex.IsChecked.Value)
            {
                var regEx = new Regex(this.Pattern.Text);

                outputText = regEx.Replace(outputText, this.Replacement.Text);
            }
            else
            {
                outputText = outputText.Replace(this.Pattern.Text, this.Replacement.Text);
            }

            if (this.Trim.IsChecked.HasValue && this.Trim.IsChecked.Value)
            {
                outputText = outputText.Trim();
            }

            this.Output.Text = outputText;
        }

        private void CommonReplacements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var chosenReplacement = (sender as ComboBox).SelectedItem as ComboBoxItem;

            if (chosenReplacement == null)
            {
                return;
            }

            switch (chosenReplacement.Content)
            {
                case "Replace CRLF":
                    this.Pattern.Text = @"(.+?)(\r?\n)(\s*)";
                    this.Replacement.Text = @"$1, ";
                    this.IsRegex.IsChecked = true;
                    break;
                case "Replace CRLF with single apostrophes":
                    this.Pattern.Text = @"(.+?)(\r?\n)(\s*)";
                    this.Replacement.Text = @"'$1', ";
                    this.IsRegex.IsChecked = true;
                    break;
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            this.Input.Text = this.Output.Text;
        }
    }
}
