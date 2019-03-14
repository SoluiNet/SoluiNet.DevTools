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
using Microsoft.Win32;
using SoluiNet.DevTools.Core.Tools.File;

namespace SoluiNet.DevTools.Utils.File
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class FileToolsUserControl : UserControl
    {
        public FileToolsUserControl()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() == true)
            {
                FilePath.Text = fileDialog.FileName;
            }
        }

        private void SearchLines_Click(object sender, RoutedEventArgs e)
        {
            List<string> extractedLines;

            if (IsRegEx.IsChecked ?? false)
            {
                extractedLines = FileTools.ExtractLinesMatchingRegEx(FilePath.Text, SearchPattern.Text);
            }
            else
            {
                extractedLines = FileTools.ExtractLinesContainingSearchPattern(FilePath.Text, SearchPattern.Text);
            }

            Output.Text = string.Empty;

            foreach (var line in extractedLines)
            {
                Output.Text += line + "\r\n";
            }
        }

        private void CalculateChecksum_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = FileTools.CalculateChecksum(ChecksumType.Text, FilePath.Text);
        }
    }
}
