// <copyright file="TransformUmlPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Transform.Uml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Transform.Uml.CSharpTransformation;

    /// <summary>
    /// A plugin which provides transform methods for UML.
    /// </summary>
    public class TransformUmlPlugin : ITransformPlugin
    {
        /// <inheritdoc/>
        public List<string> SupportedInputFormats
        {
            get
            {
                return new List<string> { "cs", "dll" };
            }
        }

        /// <inheritdoc/>
        public List<string> SupportedOutputFormats
        {
            get
            {
                // for XMI see following page: http://www.omgwiki.org/model-interchange/doku.php?id=start
                return new List<string> { "xml", "uml", "jpg", "xmi" };
            }
        }

        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                return "TransformUml";
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public object Transform(string inputStream, string inputFormat, string preferedOutputFormat)
        {
            throw new NotImplementedException();
        }
    }
}
