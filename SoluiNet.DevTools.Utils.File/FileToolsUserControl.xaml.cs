// <copyright file="FileToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System.Globalization;

namespace SoluiNet.DevTools.Utils.File
{
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

    /// <summary>
    /// Interaction logic for FileToolsUserControl.xaml.
    /// </summary>
    public partial class FileToolsUserControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileToolsUserControl"/> class.
        /// </summary>
        public FileToolsUserControl()
        {
            this.InitializeComponent();
        }

        private delegate string FormatSearchResultLine(string line);

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.FilePath.Text = fileDialog.FileName;
            }
        }

        private void SetContentForFile(string filePath, string searchPattern, FormatSearchResultLine formatLine = null, string prefix = "")
        {
            List<string> extractedLines;

            if (this.IsRegEx.IsChecked ?? false)
            {
                extractedLines = FileTools.ExtractLinesMatchingRegEx(filePath, searchPattern);
            }
            else
            {
                extractedLines = FileTools.ExtractLinesContainingSearchPattern(filePath, searchPattern);
            }

            foreach (var line in extractedLines)
            {
                this.Output.Text += string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}\r\n",
                    prefix + (string.IsNullOrEmpty(prefix) ? string.Empty : " - "),
                    formatLine != null ? formatLine(line) : line);
            }
        }

        private void SearchLines_Click(object sender, RoutedEventArgs e)
        {
            var isFileSet = !(string.IsNullOrEmpty(this.FilePath.Text) || this.FilePath.Text == "File");
            var isFolderSet = !(string.IsNullOrEmpty(this.FolderPath.Text) || this.FolderPath.Text == "Folder");

            this.Output.Text = string.Empty;

            if (isFolderSet)
            {
                var foundFiles = FileHelper.GetFilesInDirectory(this.FolderPath.Text, this.Filter.Text, true);

                foreach (var item in foundFiles.AsParallel())
                {
                    this.SetContentForFile(
                        item,
                        this.SearchPattern.Text,
                        (line) => line.Trim(),
                        item);
                }
            }
            else if (isFileSet)
            {
                this.SetContentForFile(this.FilePath.Text, this.SearchPattern.Text);
            }
        }

        private void CalculateChecksum_Click(object sender, RoutedEventArgs e)
        {
            this.Output.Text = FileTools.CalculateChecksum(this.ChecksumType.Text, this.FilePath.Text);
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
            };

            try
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    this.FolderPath.Text = folderDialog.SelectedPath;
                }
            }
            finally
            {
                folderDialog.Dispose();
            }
        }

        private void FindFiles_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
