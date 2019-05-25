// <copyright file="GeneralUtils.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides a collection of methods for general purposes.
    /// </summary>
    public class GeneralUtils
    {
        /// <summary>
        /// A delegate to get a processed byte array.
        /// </summary>
        /// <param name="originalByteArray">The original byte array.</param>
        /// <returns>Returns a processed byte array.</returns>
        public delegate byte[] GetProcessedByteArray(byte[] originalByteArray);

        /// <summary>
        /// Encode a string.
        /// </summary>
        /// <param name="originalByteArray">The original byte array.</param>
        /// <param name="getProcessedByteArrayDelegate">The delegate to process the array.</param>
        /// <param name="chosenEncoding">The encoding which should be used.</param>
        /// <returns>Returns an encoded string.</returns>
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

        /// <summary>
        /// Convert a byte array to a string.
        /// </summary>
        /// <param name="byteArray">The byte array.</param>
        /// <returns>The string which represents the byte array.</returns>
        public static string ByteArrayToString(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Encode a string with base64.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>Returns the base64 encoded string.</returns>
        public static string Base64Encode(string plainText)
        {
            return plainText.ToBase64();
        }

        /// <summary>
        /// Decode a string with base64.
        /// </summary>
        /// <param name="encodedString">The encoded string.</param>
        /// <returns>Returns the base64 decoded string.</returns>
        public static string Base64Decode(string encodedString)
        {
            return encodedString.FromBase64();
        }
    }
}
