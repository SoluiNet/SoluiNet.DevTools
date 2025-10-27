// <copyright file="TimeTrackingException.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Exceptions
{
    using System;

    /// <summary>
    /// Base exception for time tracking operations.
    /// </summary>
    public class TimeTrackingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingException"/> class.
        /// </summary>
        public TimeTrackingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TimeTrackingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TimeTrackingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}