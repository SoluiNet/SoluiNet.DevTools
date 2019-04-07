using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    public partial class FileToolsUserControl : System.Windows.Controls.UserControl
    {
        public FileToolsUserControl()
        {
            InitializeComponent();
        }

        private delegate string FormatSearchResultLine(string line);

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
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

        private void SetContentForFile(string filePath, string searchPattern, FormatSearchResultLine formatLine = null, string prefix = "")
        {
            List<string> extractedLines;

            if (IsRegEx.IsChecked ?? false)
            {
                extractedLines = FileTools.ExtractLinesMatchingRegEx(filePath, searchPattern);
            }
            else
            {
                extractedLines = FileTools.ExtractLinesContainingSearchPattern(filePath, searchPattern);
            }

            foreach (var line in extractedLines)
            {
                Output.Text += string.Format("{0}{1}\r\n", 
                    prefix + (string.IsNullOrEmpty(prefix) ? string.Empty : " - "), 
                    formatLine != null ? formatLine(line) : line);
            }
        }

        private void SearchLines_Click(object sender, RoutedEventArgs e)
        {
            var isFileSet = !(string.IsNullOrEmpty(FilePath.Text) || FilePath.Text == "File");
            var isFolderSet = !(string.IsNullOrEmpty(FolderPath.Text) || FolderPath.Text == "Folder");

            Output.Text = string.Empty;

            if (isFolderSet)
            {
                var foundFiles = FileHelper.GetFilesInDirectory(FolderPath.Text, Filter.Text, true);

                foreach (var item in foundFiles.AsParallel())
                {
                    SetContentForFile(item, 
                        SearchPattern.Text, 
                        (line) => {
                            return line.Trim();
                        }, 
                        item);
                }
            }
            else if (isFileSet)
            {
                SetContentForFile(FilePath.Text, SearchPattern.Text);
            }
        }

        private void CalculateChecksum_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = FileTools.CalculateChecksum(ChecksumType.Text, FilePath.Text);
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath.Text = folderDialog.SelectedPath;
            }
        }
    }
}
