// <copyright file="XmlHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.XML
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.Xml.Xsl;
    using SoluiNet.DevTools.Core.Tools.Stream;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides a collection of methods to work with XML.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Deserialize a string.
        /// </summary>
        /// <typeparam name="T">The type which should be deserialized.</typeparam>
        /// <param name="serializedString">The serialized object as string.</param>
        /// <returns>Returns a deserialized instance of <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(string serializedString)
        {
            if (string.IsNullOrEmpty(serializedString))
            {
                return default(T);
            }

            var stream = new MemoryStream();

            var streamWriter = new StreamWriter(stream);

            streamWriter.Write(serializedString);
            streamWriter.Flush();

            stream.Position = 0;

            return Deserialize<T>(stream);
        }

        /// <summary>
        /// Deserialize a string.
        /// </summary>
        /// <typeparam name="T">The type which should be deserialized.</typeparam>
        /// <param name="serializedString">The serialized object as string.</param>
        /// <returns>Returns a deserialized instance of <typeparamref name="T"/>.</returns>
        public static T DeserializeString<T>(this string serializedString)
        {
            if (string.IsNullOrEmpty(serializedString))
            {
                return default(T);
            }

            return Deserialize<T>(serializedString);
        }

        /// <summary>
        /// Deserialize a stream.
        /// </summary>
        /// <typeparam name="T">The type which should be deserialized.</typeparam>
        /// <param name="stream">The serialized object as stream.</param>
        /// <returns>Returns a deserialized instance of <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(System.IO.Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            var deserializedElement = (T)xmlSerializer.Deserialize(stream);

            return deserializedElement;
        }

        /// <summary>
        /// Serialize a instance of an object to a string.
        /// </summary>
        /// <typeparam name="T">The type which should be serialized.</typeparam>
        /// <param name="instance">The instance of the type which should be serialized.</param>
        /// <returns>Returns a serialized instance of <typeparamref name="T"/> as string.</returns>
        public static string Serialize<T>(T instance)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var writer = new EncodedStringWriter(Encoding.UTF8))
            {
                xmlSerializer.Serialize(writer, instance);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Format the overgiven XML string
        /// Taken from: https://stackoverflow.com/questions/1123718/format-xml-string-to-print-friendly-xml-string.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>Returns an indented XML string.</returns>
        public static string Format(string xmlString)
        {
            string result = string.Empty;

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
        /// Transforms a XML String with the overgiven XSL template.
        /// </summary>
        /// <param name="xslTemplate">The XSL template.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>Returns the transformed XML.</returns>
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
        /// Checks if overgiven text contains a valid XML root node.
        /// </summary>
        /// <param name="xmlText">The string which should be checked.</param>
        /// <returns>Returns true if overgiven text contains a valid XML root node.</returns>
        public static bool IsValidXmlRootNode(string xmlText)
        {
            var xmlRegex = new Regex("</([^>]+?)>$");

            var endNode = xmlRegex.Match(xmlText);

            return endNode.Success && xmlText.Contains(string.Format("<{0}", endNode.Groups[1].Value));
        }

        /// <summary>
        /// Checks if overgiven text is a XML string.
        /// </summary>
        /// <param name="xmlText">The string which should be checked.</param>
        /// <returns>Returns true if overgiven text is a XML string.</returns>
        public static bool IsXml(string xmlText)
        {
            return xmlText.Contains("<?xml") || IsValidXmlRootNode(xmlText);
        }

        /// <summary>
        /// Serialize a XML document to a string.
        /// </summary>
        /// <param name="xmlDocument">The XML document which should be serialized.</param>
        /// <returns>Returns a serialized XML document as string.</returns>
        public static string Serialize(XDocument xmlDocument)
        {
            var stream = new MemoryStream();

            xmlDocument.Save(stream);

            return StreamHelper.StreamToString(stream);
        }

        /// <summary>
        /// Merge two XML documents in a single XML document.
        /// </summary>
        /// <param name="firstXmlDocument">The first XML document.</param>
        /// <param name="secondXmlDocument">The second XML document.</param>
        /// <param name="xmlNodeName">The XML node name which contents should be merged.</param>
        /// <returns>Returns a merged XML document as string.</returns>
        public static string Merge(string firstXmlDocument, string secondXmlDocument, string xmlNodeName)
        {
            var firstXml = XDocument.Parse(firstXmlDocument);
            var secondXml = XDocument.Parse(secondXmlDocument);

            // Combine and remove duplicates
            var combinedXml = firstXml.Descendants(xmlNodeName)
                .Union(secondXml.Descendants(xmlNodeName));

            return Serialize(combinedXml);
        }
    }
}
