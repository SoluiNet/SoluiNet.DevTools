// <copyright file="SoluiNetWebException.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// An exception type for SoluiNet Plugins.
    /// </summary>
    [Serializable]
    public class SoluiNetWebException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetWebException"/> class.
        /// </summary>
        public SoluiNetWebException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetWebException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SoluiNetWebException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetWebException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SoluiNetWebException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetWebException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected SoluiNetWebException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
