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
        public List<string> SupportedFileExtensions
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
    }
}
