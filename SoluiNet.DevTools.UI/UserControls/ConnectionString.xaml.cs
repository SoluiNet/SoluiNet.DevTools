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

namespace SoluiNet.DevTools.UI.UserControls
{
    /// <summary>
    /// Interaktionslogik für ConnectionString.xaml
    /// </summary>
    public partial class ConnectionString : UserControl
    {
        public ConnectionString()
        {
            InitializeComponent();
        }

        private bool Expanded { get; set; }

        public string Value
        {
            get { return ConnectionStringValue.Text; }
            set { ConnectionStringValue.Text = value; }
        }

        public string NameKey
        {
            get { return ConnectionStringName.Text; }
            set { ConnectionStringName.Text = value; }
        }

        public string ProviderName { get; set; }

        private void AdditionalInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Expanded)
            {
                this.Expanded = true;

                AdditionalInfo.Content = "^";
                this.Height = 200;

                var providerNameTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "ProviderName"
                };

                providerNameTextBox.Text = ProviderName;
                providerNameTextBox.TextChanged += (o, args) => { this.ProviderName = ((TextBox) o).Text; };

                ContentGrid.Children.Add(providerNameTextBox);
            }
            else
            {
                AdditionalInfo.Content = "...";
                this.Height = 45;

                var providerNameTextBox = ContentGrid.FindName("ProviderName") as UIElement;
                ContentGrid.Children.Remove(providerNameTextBox);

                this.Expanded = false;
            }
        }
    }
}
