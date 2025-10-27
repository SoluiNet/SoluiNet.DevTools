// <copyright file="PlatformNotSupportedException.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Exceptions
{
    using System;

    /// <summary>
    /// Exception thrown when the current platform is not supported for time tracking.
    /// </summary>
    public class PlatformNotSupportedException : TimeTrackingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformNotSupportedException"/> class.
        /// </summary>
        /// <param name="platformName">The name of the unsupported platform.</param>
        public PlatformNotSupportedException(string platformName)
            : base($"Platform '{platformName}' is not supported for time tracking")
        {
            this.PlatformName = platformName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformNotSupportedException"/> class.
        /// </summary>
        /// <param name="platformName">The name of the unsupported platform.</param>
        /// <param name="innerException">The inner exception.</param>
        public PlatformNotSupportedException(string platformName, Exception innerException)
            : base($"Platform '{platformName}' is not supported for time tracking", innerException)
        {
            this.PlatformName = platformName;
        }

        /// <summary>
        /// Gets the name of the unsupported platform.
        /// </summary>
        public string PlatformName { get; }
    }
}