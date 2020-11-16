// <copyright file="XmlTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.XmlTransformation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Xsl;
    using SoluiNet.DevTools.Core.Tools.XML;

    /// <summary>
    /// Provides a collection of methods for XML.
    /// </summary>
    public static class XmlTools
    {
        /// <summary>
        /// Format the overgiven XML string
        /// Taken from: https://stackoverflow.com/questions/1123718/format-xml-string-to-print-friendly-xml-string.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>Returns an indented XML string.</returns>
        public static string Format(string xmlString)
        {
            return XmlHelper.Format(xmlString);
        }

        /// <summary>
        /// Transforms a XML String with the overgiven XSL template.
        /// </summary>
        /// <param name="xslTemplate">The XSL template.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>Returns the transformed XML.</returns>
        public static string Transform(string xslTemplate, string xmlString)
        {
            return XmlHelper.Transform(xslTemplate, xmlString);
        }

        /// <summary>
        /// Validate a XML string against a XML schema definition.
        /// </summary>
        /// <param name="xmlSchemaString">The XML schema definition.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>Returns the results of the validation into a string.</returns>
        public static string ValidateAgainstSchema(string xmlSchemaString, string xmlString)
        {
            var result = string.Empty;

            XmlSchema xmlSchema;
            using (var xsdReader = XmlReader.Create(new StringReader(xmlSchemaString)))
            {
                xmlSchema = XmlSchema.Read(xsdReader, (validationSender, eventArg) =>
                {
                    if (eventArg.Severity == XmlSeverityType.Warning)
                    {
                        result += "XSD - WARNING: ";
                    }
                    else if (eventArg.Severity == XmlSeverityType.Error)
                    {
                        result += "XSD - ERROR: ";
                    }

                    result += eventArg.Message + "\r\n";
                });
            }

            var xsdSchemas = new XmlSchemaSet();
            xsdSchemas.Add(xmlSchema);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = xsdSchemas,
                ValidationFlags =
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.ReportValidationWarnings,
            };

            settings.ValidationEventHandler += (validationSender, eventArg) =>
            {
                if (eventArg.Severity == XmlSeverityType.Warning)
                {
                    result += "XML -WARNING: ";
                }
                else if (eventArg.Severity == XmlSeverityType.Error)
                {
                    result += "XML -ERROR: ";
                }

                result += eventArg.Message + "\r\n";
            };

            try
            {
                using (var validationReader = XmlReader.Create(new StringReader(xmlString), settings))
                {
                    while (validationReader.Read())
                    {
                    }
                }
            }
            catch (XmlException xmlException)
            {
                result += string.Format(CultureInfo.InvariantCulture, "XML-Exception: {0}", xmlException.Message + "\r\n" + xmlException.InnerException?.Message);
            }

            return result;
        }
    }
}
