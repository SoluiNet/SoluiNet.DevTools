// <copyright file="CSharpTransform.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Transform.Uml.CSharpTransformation
{
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using SoluiNet.DevTools.Core.Tools.File;
    using SoluiNet.DevTools.Core.Tools.XML;

    public class CSharpTransform
    {
        private string sourceFile;

        public CSharpTransform(string sourceFile)
        {
            this.sourceFile = sourceFile;
        }

        public string CSharpSourceFile
        {
            get
            {
                return this.sourceFile;
            }
        }

        public string TransformToXml()
        {
            if (string.IsNullOrEmpty(this.sourceFile))
            {
                return string.Empty;
            }

            var fileContents = FileHelper.StringFromFile(this.sourceFile);

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
