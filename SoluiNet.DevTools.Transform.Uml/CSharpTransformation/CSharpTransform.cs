// <copyright file="CSharpTransform.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;
using System.Xml.Linq;
using SoluiNet.DevTools.Core.Tools.File;
using SoluiNet.DevTools.Core.Tools.XML;

namespace SoluiNet.DevTools.Transform.Uml.CSharpTransformation
{
    public class CSharpTransform
    {
        private string _sourceFile;

        public string CSharpSourceFile
        {
            get
            {
                return _sourceFile;
            }
        }

        public CSharpTransform(string sourceFile)
        {
            _sourceFile = sourceFile;
        }

        public string TransformToXml()
        {
            if (string.IsNullOrEmpty(_sourceFile))
            {
                return string.Empty;
            }

            var fileContents = FileHelper.StringFromFile(_sourceFile);

            if (string.IsNullOrEmpty(fileContents))
            {
                return string.Empty;
            }

            var classesRegex = new Regex(@"class (.+)\s*\{");

            var xmlDocument = new XDocument();

            var umlDefinition = new XElement("umlDefinition");

            foreach (Match match in classesRegex.Matches(fileContents))
            {
                var classDefinition = new XElement("class");

                classDefinition.Add(new XAttribute("className", match.Groups[1].Value));

                umlDefinition.Add(classDefinition);
            }

            xmlDocument.Add(umlDefinition);

            return XmlHelper.Serialize(xmlDocument);
        }
    }
}
