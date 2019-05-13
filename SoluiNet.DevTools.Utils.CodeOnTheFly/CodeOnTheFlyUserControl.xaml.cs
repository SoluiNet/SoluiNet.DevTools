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

namespace SoluiNet.DevTools.Utils.CodeOnTheFly
{
    /// <summary>
    /// Interaktionslogik für CodeOnTheFlyUserControl.xaml
    /// </summary>
    public partial class CodeOnTheFlyUserControl : UserControl
    {
        public CodeOnTheFlyUserControl()
        {
            InitializeComponent();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            CodeTabs.SelectedIndex = 1;

            Result.Text = CodeOnTheFlyTools.RunDynamicCode(Code.Text);
        }
    }
}
