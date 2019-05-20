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

namespace SoluiNet.DevTools.UI
{
    /// <summary>
    /// Interaktionslogik für PrepareText.xaml
    /// </summary>
    public partial class PrepareText : SoluiNetWindow
    {
        public PrepareText()
        {
            InitializeComponent();
        }

        private void AdjustText_Click(object sender, RoutedEventArgs e)
        {
            var outputText = Input.Text;

            if (IsRegex.IsChecked.HasValue && IsRegex.IsChecked.Value)
            {
                var regEx = new Regex(Pattern.Text);

                outputText = regEx.Replace(outputText, Replacement.Text);
            }
            else
            {
                outputText = outputText.Replace(Pattern.Text, Replacement.Text);
            }

            if (Trim.IsChecked.HasValue && Trim.IsChecked.Value)
            {
                outputText = outputText.Trim();
            }

            Output.Text = outputText;
        }

        private void CommonReplacements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var chosenReplacement = (sender as ComboBox).SelectedItem as ComboBoxItem;

            if (chosenReplacement == null)
                return;

            switch (chosenReplacement.Content)
            {
                case "Replace CRLF":
                    Pattern.Text = @"(.+?)(\r?\n)(\s*)";
                    Replacement.Text = @"$1, ";
                    IsRegex.IsChecked = true;
                    break;
                case "Replace CRLF with single apostrophes":
                    Pattern.Text = @"(.+?)(\r?\n)(\s*)";
                    Replacement.Text = @"'$1', ";
                    IsRegex.IsChecked = true;
                    break;
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            Input.Text = Output.Text;
        }
    }
}
