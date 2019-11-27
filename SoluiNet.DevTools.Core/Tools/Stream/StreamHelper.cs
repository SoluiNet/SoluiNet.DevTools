// <copyright file="StreamHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Stream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods to work with streams.
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Convert a stream to a string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding. If not provided UTF-8 will be used.</param>
        /// <returns>Returns a <see cref="string"/> for the contents of the <see cref="Stream"/>.</returns>
        public static string StreamToString(System.IO.Stream stream, Encoding encoding = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            stream.Position = 0;

            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Convert a stream to a string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding. If not provided UTF-8 will be used.</param>
        /// <returns>Returns a <see cref="string"/> for the contents of the <see cref="Stream"/>.</returns>
        public static string ReadStringFromStream(this System.IO.Stream stream, Encoding encoding = null)
        {
            return StreamToString(stream, encoding);
        }

        /// <summary>
        /// Convert stream to byte array (taken from: https://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream).
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns the converted stream as byte array.</returns>
        public static byte[] ToByteArray(this System.IO.Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}
