using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Tools;
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

namespace SoluiNet.DevTools.Utils.VsDevEnv
{
    /// <summary>
    /// Interaktionslogik für VsDevEnvToolsUserControl.xaml
    /// </summary>
    public partial class VsDevEnvToolsUserControl : UserControl
    {
        public VsDevEnvToolsUserControl()
        {
            InitializeComponent();
        }

        private void TransformCsToUml_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "All files (*.cs)|*.cs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() == true)
            {
                PluginHelper.GetPluginByName<ITransformPlugin>("TransformUml").Transform(fileDialog.FileName, "xml");
            }
        }
    }
}
