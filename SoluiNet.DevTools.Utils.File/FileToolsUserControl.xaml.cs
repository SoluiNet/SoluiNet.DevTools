// <copyright file="FileToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.File
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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
    using SoluiNet.DevTools.Core.UI.WPF.General;

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

        private void SetContentForFile(string filePath, string searchPattern, FormatSearchResultLine formatLine = null, string prefix = "", StreamWriter logWriter = null)
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

            var ommitPrefix = this.OmmitPrefix.IsChecked ?? false;

            var searchRegex = new Regex(searchPattern);
            var replacePattern = this.ReplacePattern.Text;

            foreach (var line in extractedLines)
            {
                var lineContent = line;

                if (!string.IsNullOrEmpty(replacePattern))
                {
                    lineContent = searchRegex.Replace(line, replacePattern);
                }

                if (logWriter != null)
                {
                    logWriter.WriteLine(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}{1}\r\n",
                            ommitPrefix ? string.Empty : prefix + (string.IsNullOrEmpty(prefix) ? string.Empty : " - "),
                            formatLine != null ? formatLine(lineContent) : lineContent));
                }
                else
                {
                    this.Output.Text += string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}\r\n",
                        ommitPrefix ? string.Empty : prefix + (string.IsNullOrEmpty(prefix) ? string.Empty : " - "),
                        formatLine != null ? formatLine(lineContent) : lineContent);
                }
            }
        }

        private void SearchLines_Click(object sender, RoutedEventArgs e)
        {
            var isFileSet = !(string.IsNullOrEmpty(this.FilePath.Text) || this.FilePath.Text == "File");
            var isFolderSet = !(string.IsNullOrEmpty(this.FolderPath.Text) || this.FolderPath.Text == "Folder");

            this.Output.Text = string.Empty;

            StreamWriter logFile = null;
            var temporaryFilePath = string.Empty;

            if (this.WriteToLog.IsChecked ?? false)
            {
                temporaryFilePath = FileHelper.GetTemporaryFilePath();

                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(temporaryFilePath));
                logFile = File.AppendText(temporaryFilePath);
            }

            try
            {
                if (isFolderSet)
                {
                    var foundFiles = FileHelper.GetFilesInDirectory(this.FolderPath.Text, this.Filter.Text, true);

                    foreach (var item in foundFiles.AsParallel())
                    {
                        this.SetContentForFile(
                            item,
                            this.SearchPattern.Text,
                            (line) => line.Trim(),
                            item,
                            logFile);
                    }
                }
                else if (isFileSet)
                {
                    this.SetContentForFile(this.FilePath.Text, this.SearchPattern.Text, null, string.Empty, logFile);
                }
            }
            finally
            {
                if (logFile != null)
                {
                    logFile.Flush();
                    logFile.Close();
                    logFile.Dispose();
                }
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
            if (string.IsNullOrEmpty(this.SearchPattern.Text))
            {
                System.Windows.MessageBox.Show("Please provide an search pattern", "Missing search pattern");
                return;
            }

            var foundFiles = FileHelper.GetFilesInDirectory(this.FolderPath.Text, this.Filter.Text, true);

            Regex searchPatternRegex;

            if (this.IsRegEx.IsChecked ?? false)
            {
                searchPatternRegex = new Regex(this.SearchPattern.Text);
            }
            else
            {
                searchPatternRegex = new Regex(string.Format(CultureInfo.InvariantCulture, "(.*){0}(.*)", Regex.Escape(this.SearchPattern.Text).Replace("\\*", "(.*)")));
            }

            StreamWriter logFile = null;
            var temporaryFilePath = string.Empty;

            if (this.WriteToLog.IsChecked ?? false)
            {
                temporaryFilePath = FileHelper.GetTemporaryFilePath();

                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(temporaryFilePath));
                logFile = File.AppendText(temporaryFilePath);
            }

            try
            {
                foreach (var item in foundFiles.AsParallel())
                {
                    if (searchPatternRegex.IsMatch(item))
                    {
                        if (this.WriteToLog.IsChecked ?? false)
                        {
                            logFile.WriteLine(item);
                        }
                        else
                        {
                            this.Output.Text += item + "\r\n";
                        }
                    }
                }

                if (!(this.WriteToLog.IsChecked ?? false) && this.Output.Text.EndsWith("\r\n", false, CultureInfo.InvariantCulture))
                {
                    this.Output.Text = this.Output.Text.Substring(0, this.Output.Text.Length - 2);
                }
                else
                {
                    this.Output.Text += string.Format(CultureInfo.InvariantCulture, "Results saved to '{0}'", temporaryFilePath);
                }
            }
            finally
            {
                if (logFile != null)
                {
                    logFile.Flush();
                    logFile.Close();
                    logFile.Dispose();
                }
            }
        }

        private void ExtractFiles_Click(object sender, RoutedEventArgs e)
        {
            var fileSeparator = string.IsNullOrEmpty(this.FileSeparator.Text) ? "\r\n" : this.FileSeparator.Text;

            this.CompressionTabs.SelectedIndex = 2;

            foreach (var item in this.CompressionFiles.Text.Split(new string[] { fileSeparator }, StringSplitOptions.RemoveEmptyEntries).AsParallel())
            {
                var extractionPath = item.Remove(item.Length - 4);

                this.CompressionLog.Text += string.Format(CultureInfo.InvariantCulture, "Extract '{0}' to '{1}'.\r\n", item, extractionPath);
                ZipFile.ExtractToDirectory(item, extractionPath);
            }
        }

        private void CopyFromSearchResults_Click(object sender, RoutedEventArgs e)
        {
            this.CompressionFiles.Text = this.Output.Text;
        }
    }
}
