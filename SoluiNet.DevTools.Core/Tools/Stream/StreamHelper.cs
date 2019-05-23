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
    public class StreamHelper
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
    }
}
