// <copyright file="ITimeTrackingConfiguration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interface for time tracking configuration.
    /// </summary>
    public interface ITimeTrackingConfiguration
    {
        /// <summary>
        /// Gets the monitoring interval in seconds.
        /// </summary>
        int MonitoringIntervalSeconds { get; }

        /// <summary>
        /// Gets the database connection string or path.
        /// </summary>
        string DatabasePath { get; }

        /// <summary>
        /// Gets the logging level.
        /// </summary>
        LogLevel LogLevel { get; }

        /// <summary>
        /// Gets a value indicating whether auto-start is enabled.
        /// </summary>
        bool EnableAutoStart { get; }

        /// <summary>
        /// Gets platform-specific settings.
        /// </summary>
        IReadOnlyDictionary<string, string> PlatformSpecificSettings { get; }

        /// <summary>
        /// Gets the monitoring interval as a TimeSpan.
        /// </summary>
        TimeSpan MonitoringInterval { get; }

        /// <summary>
        /// Validates the configuration and returns any validation errors.
        /// </summary>
        /// <returns>A collection of validation error messages, empty if valid.</returns>
        IEnumerable<string> Validate();
    }
}