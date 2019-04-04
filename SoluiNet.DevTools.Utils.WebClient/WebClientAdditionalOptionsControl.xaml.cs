using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoluiNet.DevTools.Utils.WebClient
{
    /// <summary>
    /// Interaktionslogik für WebClientAdditionalOptionsControl.xaml
    /// </summary>
    public partial class WebClientAdditionalOptionsControl : UserControl
    {
        public WebClientAdditionalOptionsControl()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> Options
        {
            get
            {
                var options = new Dictionary<string, string>();

                foreach (var option in OptionsMainGrid.Children)
                {
                    if (option is WebClientAdditionalOptionControl control)
                    {
                        options.Add(control.Key.Text, control.Value.Text);
                    }
                }

                return options;
            }
        }

        public void AddOption(string key, string value)
        {
            var newOption = new WebClientAdditionalOptionControl();

            newOption.Key.Text = key;
            newOption.Value.Text = value;

            newOption.SetValue(Grid.RowProperty, 1);

            newOption.RemoveElement.Click += (sourceElement, args) =>
            {
                OptionsMainGrid.Children.Remove(newOption);
            };

            OptionsMainGrid.Children.Add(newOption);
        }

        private void AddAdditionalOption_Click(object sender, RoutedEventArgs e)
        {
            var newOption = new WebClientAdditionalOptionControl();

            newOption.SetValue(Grid.RowProperty, 1);

            newOption.RemoveElement.Click += (sourceElement, args) =>
            {
                OptionsMainGrid.Children.Remove(newOption);
            };

            OptionsMainGrid.Children.Add(newOption);
        }
    }
}
