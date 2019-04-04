using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
    }
}
