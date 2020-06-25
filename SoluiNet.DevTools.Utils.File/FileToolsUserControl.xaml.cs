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
    using NLog;
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

        private Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

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

            var omitPrefix = this.OmitPrefix.IsChecked ?? false;
            var omitLineBreak = this.OmitLineBreak.IsChecked ?? false;
            var removeDuplicates = this.RemoveDuplicates.IsChecked ?? false;

            if (removeDuplicates)
            {
                extractedLines = extractedLines.Distinct().ToList();
            }

            var sorting = this.Sorting.SelectedValue.ToString();

            switch (sorting)
            {
                case "Content ASC":
                    extractedLines.Sort();
                    break;
                case "Content DESC":
                    extractedLines = extractedLines.OrderByDescending(x => x).ToList();
                    break;
                default:
                    break;
            }

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
                            "{0}{1}{2}",
                            omitPrefix ? string.Empty : prefix + (string.IsNullOrEmpty(prefix) ? string.Empty : " - "),
                            formatLine != null ? formatLine(lineContent) : lineContent,
                            omitLineBreak ? string.Empty : "\r\n"));
                }
                else
                {
                    this.Output.Text += string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}{2}",
                        omitPrefix ? string.Empty : prefix + (string.IsNullOrEmpty(prefix) ? string.Empty : " - "),
                        formatLine != null ? formatLine(lineContent) : lineContent,
                        omitLineBreak ? string.Empty : "\r\n");
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
                    this.Logger.Info(
                        CultureInfo.InvariantCulture,
                        "Search for lines in folder '{0}' (filter: '{2}') with the search pattern '{1}' (RegEx: {3}) and replace pattern '{4}' (Ommit Prefix: {5}, Remove duplicates: {7}, temporary file path. '{6}')",
                        this.FolderPath.Text,
                        this.SearchPattern.Text,
                        this.Filter.Text,
                        this.IsRegEx.IsChecked ?? false,
                        this.ReplacePattern.Text,
                        this.OmitPrefix.IsChecked ?? false,
                        temporaryFilePath,
                        this.RemoveDuplicates.IsChecked ?? false);

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
                    this.Logger.Info(
                        CultureInfo.InvariantCulture,
                        "Search for lines in file '{0}' (filter: '{2}') with the search pattern '{1}' (RegEx: {3}) and replace pattern '{4}' (Ommit Prefix: {5}, temporary file path. '{6}')",
                        this.FilePath.Text,
                        this.SearchPattern.Text,
                        this.Filter.Text,
                        this.IsRegEx.IsChecked ?? false,
                        this.ReplacePattern.Text,
                        this.OmitPrefix.IsChecked ?? false,
                        temporaryFilePath);

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

            this.Logger.Info(
                CultureInfo.InvariantCulture,
                "Find files with the search pattern '{1}' (RegEx: {2}) in folder '{0}' (temporary file path: '{3}')",
                this.FolderPath.Text,
                this.SearchPattern.Text,
                this.IsRegEx.IsChecked ?? false,
                temporaryFilePath);

            try
            {
                foreach (var item in foundFiles.AsParallel())
                {
                    if (searchPatternRegex.IsMatch(item))
                    {
                        if (this.WriteToLog.IsChecked ?? false)
                        {
                            var trimmedText = item.TrimEnd();

                            if ((trimmedText.EndsWith("\r\n", false, CultureInfo.InvariantCulture)
                                || trimmedText.EndsWith("\n", false, CultureInfo.InvariantCulture))
                                && (this.OmitLineBreak.IsChecked ?? false))
                            {
                                logFile?.Write(item);
                            }
                            else
                            {
                                logFile?.WriteLine(item);
                            }
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

        private void SplitFiles_Click(object sender, RoutedEventArgs e)
        {
            var originalFilePath = this.SplitFilePath.Text;
            var originalExtension = System.IO.Path.GetExtension(originalFilePath);

            var lines = File.ReadLines(originalFilePath);

            var i = 0;
            var offset = 0;
            var numberOfSplits = 0;

            var lastMatchingLine = 0;

            var conditionExisting = !string.IsNullOrEmpty(this.SplitCondition.Text);

            Regex splitRegex = null;
            var splitCondition = this.SplitCondition.Text;

            this.Logger.Info(
                CultureInfo.InvariantCulture,
                "Split file '{0}' (Condition: '{1}' - RegEx: {3}) at {2} lines",
                this.SplitFilePath.Text,
                this.SplitCondition.Text,
                this.NumberOfLines.Value ?? 0,
                this.SplitIsRegEx.IsChecked ?? false);

            if (this.SplitIsRegEx.IsChecked ?? false)
            {
                splitRegex = new Regex(splitCondition);
            }

            foreach (var line in lines)
            {
                if (conditionExisting)
                {
                    if (this.SplitIsRegEx.IsChecked ?? false)
                    {
                        if (splitRegex != null && splitRegex.IsMatch(line))
                        {
                            lastMatchingLine = i;
                        }
                    }
                    else
                    {
                        if (line.Contains(splitCondition))
                        {
                            lastMatchingLine = i;
                        }
                    }
                }

                if (i - offset == (this.NumberOfLines.Value ?? 0) - 1)
                {
                    var beginningOfSplit = offset;
                    var endOfSplit = i;

                    if (lastMatchingLine > 0)
                    {
                        endOfSplit = lastMatchingLine;
                    }

                    var splitFile = new StreamWriter(originalFilePath.Replace(originalExtension, string.Format(CultureInfo.InvariantCulture, ".{0:D8}{1}", ++numberOfSplits, originalExtension)));

                    try
                    {
                        var splitLines = File.ReadLines(originalFilePath).Skip(beginningOfSplit).Take(endOfSplit - beginningOfSplit);

                        foreach (var splitLine in splitLines)
                        {
                            splitFile.WriteLine(splitLine);
                        }

                        lastMatchingLine = 0;
                        offset = endOfSplit;
                    }
                    finally
                    {
                        splitFile.Flush();
                        splitFile.Close();
                        splitFile.Dispose();
                    }
                }

                i++;
            }

            var lastSplitFile = new StreamWriter(originalFilePath.Replace(originalExtension, string.Format(CultureInfo.InvariantCulture, ".{0:D8}{1}", ++numberOfSplits, originalExtension)));

            try
            {
                var splitLines = File.ReadLines(originalFilePath).Skip(offset).Take(i - offset);

                foreach (var splitLine in splitLines)
                {
                    lastSplitFile.WriteLine(splitLine);
                }
            }
            finally
            {
                lastSplitFile.Flush();
                lastSplitFile.Close();
                lastSplitFile.Dispose();
            }
        }

        private void OpenSplitFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.SplitFilePath.Text = fileDialog.FileName;
            }
        }

        private void OpenCompressionFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.CompressionFilePath.Text = fileDialog.FileName;
            }
        }
    }
}
