using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Transform.Uml.CSharpTransformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Transform.Uml
{
    public class TransformUmlPlugin : ITransformPlugin
    {
        public List<string> SupportedInputFormats
        {
            get
            {
                return new List<string> { "cs", "dll" };
            }
        }

        public List<string> SupportedOutputFormats
        {
            get
            {
                // for XMI see following page: http://www.omgwiki.org/model-interchange/doku.php?id=start

                return new List<string> { "xml", "uml", "jpg", "xmi" };
            }
        }

        public string Name
        {
            get
            {
                return "TransformUml";
            }
        }

        public object Transform(string inputFile, string preferedOutputFormat)
        {
            var extension = Path.GetExtension(inputFile);

            object result = null;

            switch (extension.Substring(1))
            {
                case "cs":
                    var cSharpTransform = new CSharpTransform(inputFile);

                    if (preferedOutputFormat.Equals("xml"))
                    {
                        result = cSharpTransform.TransformToXml();
                    }

                    break;
                default:
                    result = "#ERR# No valid format #ERR#";
                    break;
            }

            return result;
        }

        public object Transform(string inputStream, string inputFormat, string preferedOutputFormat)
        {
            throw new NotImplementedException();
        }
    }
}
