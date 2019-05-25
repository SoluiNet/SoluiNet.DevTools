// <copyright file="XmlTransformationUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.XmlTransformation
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
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Xsl;
    using Microsoft.Win32;
    using SoluiNet.DevTools.Core.Tools.File;

    /// <summary>
    /// Interaction logic for XmlTransformationUserControl.xaml.
    /// </summary>
    public partial class XmlTransformationUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformationUserControl"/> class.
        /// </summary>
        public XmlTransformationUserControl()
        {
            this.InitializeComponent();
        }

        private void Transform_Click(object sender, RoutedEventArgs e)
        {
            this.Output.Text = XmlTools.Transform(this.XslInput.Text, this.XmlInput.Text);

            if (!string.IsNullOrEmpty(this.Output.Text))
            {
                this.OutputHtml.NavigateToString(this.Output.Text);
            }
        }

        private void FormatXsl_Click(object sender, RoutedEventArgs e)
        {
            this.XslInput.Text = XmlTools.Format(this.XslInput.Text);
        }

        private void LoadXslFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "XML Stylesheet (*.xsl)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.XslInput.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void FormatXml_Click(object sender, RoutedEventArgs e)
        {
            this.XmlInput.Text = XmlTools.Format(this.XmlInput.Text);
        }

        private void LoadXmlFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "Extensible Markup Language (*.xml)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.XmlInput.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void ShowCode_Click(object sender, RoutedEventArgs e)
        {
            this.Output.Visibility = Visibility.Visible;
            this.OutputHtml.Visibility = Visibility.Hidden;
        }

        private void ShowHtml_Click(object sender, RoutedEventArgs e)
        {
            this.Output.Visibility = Visibility.Hidden;
            this.OutputHtml.Visibility = Visibility.Visible;
        }

        private void FormatXsd_Click(object sender, RoutedEventArgs e)
        {
            this.XsdInput.Text = XmlTools.Format(this.XsdInput.Text);
        }

        private void LoadXsdFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "XML Schema Definition (*.xsd)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.XsdInput.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void FormatXmlForSchema_Click(object sender, RoutedEventArgs e)
        {
            this.XmlInputForSchema.Text = XmlTools.Format(this.XmlInputForSchema.Text);
        }

        private void LoadXmlForSchemaFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "Extensible Markup Language (*.xml)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                this.XmlInputForSchema.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void ValidateXmlSchema(object sender, RoutedEventArgs e)
        {
            this.OutputXsd.Text = XmlTools.ValidateAgainstSchema(this.XsdInput.Text, this.XmlInputForSchema.Text);
        }
    }
}
