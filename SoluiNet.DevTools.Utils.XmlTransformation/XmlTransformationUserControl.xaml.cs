using Microsoft.Win32;
using SoluiNet.DevTools.Core.Tools.File;
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

namespace SoluiNet.DevTools.Utils.XmlTransformation
{
    /// <summary>
    /// Interaktionslogik für XmlTransformationUserControl.xaml
    /// </summary>
    public partial class XmlTransformationUserControl : UserControl
    {
        public XmlTransformationUserControl()
        {
            InitializeComponent();
        }

        private void Transform_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = XmlTools.Transform(XslInput.Text, XmlInput.Text);

            if (!string.IsNullOrEmpty(Output.Text))
                OutputHtml.NavigateToString(Output.Text);
        }

        private void FormatXsl_Click(object sender, RoutedEventArgs e)
        {
            XslInput.Text = XmlTools.Format(XslInput.Text);
        }

        private void LoadXslFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "XML Stylesheet (*.xsl)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() == true)
            {
                XslInput.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void FormatXml_Click(object sender, RoutedEventArgs e)
        {
            XmlInput.Text = XmlTools.Format(XmlInput.Text);
        }

        private void LoadXmlFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "Extensible Markup Language (*.xml)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() == true)
            {
                XmlInput.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void ShowCode_Click(object sender, RoutedEventArgs e)
        {
            Output.Visibility = Visibility.Visible;
            OutputHtml.Visibility = Visibility.Hidden;
        }

        private void ShowHtml_Click(object sender, RoutedEventArgs e)
        {
            Output.Visibility = Visibility.Hidden;
            OutputHtml.Visibility = Visibility.Visible;
        }

        private void FormatXsd_Click(object sender, RoutedEventArgs e)
        {
            XsdInput.Text = XmlTools.Format(XsdInput.Text);
        }

        private void LoadXsdFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "XML Schema Definition (*.xsd)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() == true)
            {
                XsdInput.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void FormatXmlForSchema_Click(object sender, RoutedEventArgs e)
        {
            XmlInputForSchema.Text = XmlTools.Format(XmlInputForSchema.Text);
        }

        private void LoadXmlForSchemaFromFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "Extensible Markup Language (*.xml)|*.xsl|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() == true)
            {
                XmlInputForSchema.Text = FileHelper.StringFromFile(fileDialog.FileName);
            }
        }

        private void ValidateXmlSchema(object sender, RoutedEventArgs e)
        {
            OutputXsd.Text = XmlTools.ValidateAgainstSchema(XsdInput.Text, XmlInputForSchema.Text);
        }
    }
}
