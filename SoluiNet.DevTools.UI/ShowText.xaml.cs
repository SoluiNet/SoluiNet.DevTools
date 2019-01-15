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
using System.Windows.Shapes;
using SoluiNet.DevTools.Core.Tools.UI;

namespace SoluiNet.DevTools.UI
{
    /// <summary>
    /// Interaktionslogik für ShowText.xaml
    /// </summary>
    public partial class ShowText : Window
    {
        public string Text
        {
            get { return TextToShow.Text; }
            set { TextToShow.Text = value; }
        }

        public ShowText()
        {
            InitializeComponent();

            var highlighting = UIHelper.LoadHighlightingDefinition(typeof(ShowText), "SQL.xshd");

            TextToShow.SyntaxHighlighting = highlighting;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
