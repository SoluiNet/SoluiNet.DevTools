using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.CryptoTools.Utilities
{
    public class GeneralUtils
    {
        public delegate byte[] GetProcessedByteArray(byte[] originalByteArray);

        public static string Encode(byte[] originalByteArray, GetProcessedByteArray getProcessedByteArrayDelegate, string chosenEncoding = "UTF8")
        {
            string result;

            switch (chosenEncoding)
            {
                case "ASCII":
                    result = Encoding.ASCII.GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "ByteString":
                    result = ByteArrayToString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "ISO-8859-1":
                    result = Encoding.GetEncoding("iso-8859-1").GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "UTF16":
                    result = Encoding.Unicode.GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "UTF16 BE":
                    result = Encoding.BigEndianUnicode.GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "UTF7":
                    result = Encoding.UTF7.GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "UTF32":
                    result = Encoding.UTF32.GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
                case "UTF8":
                default:
                    result = Encoding.UTF8.GetString(getProcessedByteArrayDelegate(originalByteArray));
                    break;
            }

            return result;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.Unicode.GetString(data);
        }
    }
}
