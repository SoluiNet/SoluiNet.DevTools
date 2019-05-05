using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
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
                return new List<string> { "xml", "uml", "jpg" };
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
            throw new NotImplementedException();
        }

        public object Transform(string inputStream, string inputFormat, string preferedOutputFormat)
        {
            throw new NotImplementedException();
        }
    }
}
