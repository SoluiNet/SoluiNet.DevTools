// <copyright file="SoluiNetException.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Exceptions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A general exception in SoluiNet applications.
    /// </summary>
    public class SoluiNetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetException"/> class.
        /// </summary>
        public SoluiNetException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SoluiNetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="formatValues">The format values.</param>
        public SoluiNetException(string message, params string[] formatValues)
            : base(string.Format(CultureInfo.InvariantCulture, message, formatValues))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SoluiNetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected SoluiNetException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
