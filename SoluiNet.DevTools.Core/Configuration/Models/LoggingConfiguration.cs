// <copyright file="LoggingConfiguration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Configuration.Models
{
    /// <summary>
    /// Represents logging configuration settings.
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        public string MinimumLevel { get; set; } = "Debug";

        /// <summary>
        /// Gets or sets a value indicating whether trace logging is enabled.
        /// </summary>
        public bool EnableTraceLogging { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether console logging is enabled.
        /// </summary>
        public bool EnableConsoleLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether file logging is enabled.
        /// </summary>
        public bool EnableFileLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of archive files to keep.
        /// </summary>
        public int MaxArchiveFiles { get; set; } = 7;

        /// <summary>
        /// Gets or sets the log file name pattern.
        /// </summary>
        public string LogFilePattern { get; set; } = "console_{shortdate}.log";
    }
}