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
            /*var xslTransformation = new XslCompiledTransform();
            xslTransformation.Load(new XmlTextReader(new StringReader(XslInput.Text)));*/
            
            using (var xslStringReader = new StringReader(XslInput.Text))
            using (var xmlStringReader = new StringReader(XmlInput.Text))
            {
                using (var xslReader = XmlReader.Create(xslStringReader))
                using (var xmlReader = XmlReader.Create(xmlStringReader))
                {
                    var xslTransformation = new XslCompiledTransform();
                    xslTransformation.Load(xslReader);

                    using (var outputWriter = new StringWriter())
                    using (var xmlWriter = XmlWriter.Create(outputWriter, xslTransformation.OutputSettings))
                    {
                        xslTransformation.Transform(xmlReader, xmlWriter);
                        Output.Text = outputWriter.ToString();
                    }
                }
            }
        }
    }
}
