using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace SoluiNet.DevTools.Core.Tools.XML
{
    public static class XmlHelper
    {
        public static T Deserialize<T>(string serializedString)
        {
            var stream = new MemoryStream();

            var streamWriter = new StreamWriter(stream);

            streamWriter.Write(serializedString);
            streamWriter.Flush();

            stream.Position = 0;

            return Deserialize<T>(stream);
        }

        public static T Deserialize<T>(Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            var deserializedElement = (T)xmlSerializer.Deserialize(stream);

            return deserializedElement;
        }

        public static string Serialize<T>(T instance)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            
            using (var writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, instance);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Format the overgiven XML string
        /// Taken from: https://stackoverflow.com/questions/1123718/format-xml-string-to-print-friendly-xml-string
        /// </summary>
        /// <param name="xmlString">The XML string</param>
        /// <returns>Returns an indented XML string</returns>
        public static string Format(string xmlString)
        {
            string result = "";

            var stream = new MemoryStream();
            var xmlWriter = new XmlTextWriter(stream, Encoding.Unicode);
            var xmlDoc = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                xmlDoc.LoadXml(xmlString);

                xmlWriter.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                xmlDoc.WriteContentTo(xmlWriter);
                xmlWriter.Flush();
                stream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                stream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                var reader = new StreamReader(stream);

                // Extract the text from the StreamReader.
                var formattedXml = reader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException)
            {
                // Handle the exception
                return string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Transforms a XML String with the overgiven XSL template
        /// </summary>
        /// <param name="xsl">The XSL template</param>
        /// <param name="xml">The XML string</param>
        /// <returns>Returns the transformed XML</returns>
        public static string Transform(string xslTemplate, string xmlString)
        {
            using (var xslStringReader = new StringReader(xslTemplate))
            using (var xmlStringReader = new StringReader(xmlString))
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
                        return outputWriter.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if overgiven text contains a valid XML root node
        /// </summary>
        /// <param name="xmlText">The string which should be checked</param>
        /// <returns>Returns true if overgiven text contains a valid XML root node</returns>
        public static bool IsValidXmlRootNode(string xmlText)
        {
            var xmlRegex = new Regex("</([^>]+?)>$");

            var endNode = xmlRegex.Match(xmlText);

            return endNode.Success && xmlText.Contains(string.Format("<{0}", endNode.Groups[1].Value));
        }

        /// <summary>
        /// Returns true if overgiven text is a XML string
        /// </summary>
        /// <param name="xmlText">The string which should be checked</param>
        /// <returns>Returns true if overgiven text is a XML string</returns>
        public static bool IsXml(string xmlText)
        {
            return xmlText.Contains("<?xml") || IsValidXmlRootNode(xmlText);
        }
    }
}
