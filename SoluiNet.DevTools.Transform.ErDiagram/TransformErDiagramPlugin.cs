﻿using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Transform.ErDiagram
{
    public class TransformErDiagramPlugin : ITransformPlugin
    {
        public List<string> SupportedInputFormats
        {
            get
            {
                return new List<string> { "sql", "udl" };
            }
        }

        public List<string> SupportedOutputFormats
        {
            get
            {
                return new List<string> { "xml", "erd", "jpg" };
            }
        }

        public string Name
        {
            get
            {
                return "TransformErDiagram";
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
