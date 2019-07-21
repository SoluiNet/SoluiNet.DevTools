// <copyright file="EncodedStringWriter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.String
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A StringWriter class with encoding.
    /// </summary>
    public sealed class EncodedStringWriter : StringWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedStringWriter"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public EncodedStringWriter(Encoding encoding)
        {
            this.Encoding = encoding;
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        public override Encoding Encoding { get; }
    }
}
