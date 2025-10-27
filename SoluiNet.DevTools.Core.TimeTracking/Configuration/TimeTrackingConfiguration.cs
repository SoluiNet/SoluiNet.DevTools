// <copyright file="TimeTrackingConfiguration.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;

    /// <summary>
    /// Implementation of time tracking configuration.
    /// </summary>
    public class TimeTrackingConfiguration : ITimeTrackingConfiguration
    {
        private readonly Dictionary<string, string> platformSpecificSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingConfiguration"/> class.
        /// </summary>
        public TimeTrackingConfiguration()
        {
            this.MonitoringIntervalSeconds = 10;
            this.DatabasePath = this.GetDefaultDatabasePath();
            this.LogLevel = LogLevel.Information;
            this.EnableAutoStart = false;
            this.platformSpecificSettings = new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public int MonitoringIntervalSeconds { get; set; }

        /// <inheritdoc />
        public string DatabasePath { get; set; }

        /// <inheritdoc />
        public LogLevel LogLevel { get; set; }

        /// <inheritdoc />
        public bool EnableAutoStart { get; set; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> PlatformSpecificSettings => this.platformSpecificSettings;

        /// <inheritdoc />
        public TimeSpan MonitoringInterval => TimeSpan.FromSeconds(this.MonitoringIntervalSeconds);

        /// <summary>
        /// Sets a platform-specific setting.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="value">The setting value.</param>
        public void SetPlatformSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            }

            this.platformSpecificSettings[key] = value ?? string.Empty;
        }

        /// <summary>
        /// Removes a platform-specific setting.
        /// </summary>
        /// <param name="key">The setting key to remove.</param>
        /// <returns>True if the setting was removed, false if it didn't exist.</returns>
        public bool RemovePlatformSetting(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return this.platformSpecificSettings.Remove(key);
        }

        /// <inheritdoc />
        public IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (this.MonitoringIntervalSeconds < 1 || this.MonitoringIntervalSeconds > 3600)
            {
                errors.Add("MonitoringIntervalSeconds must be between 1 and 3600 seconds.");
            }

            if (string.IsNullOrWhiteSpace(this.DatabasePath))
            {
                errors.Add("DatabasePath cannot be null or empty.");
            }
            else
            {
                try
                {
                    var directory = Path.GetDirectoryName(this.DatabasePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        errors.Add($"Database directory does not exist: {directory}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Invalid database path: {ex.Message}");
                }
            }

            return errors;
        }

        /// <summary>
        /// Creates a configuration with default values.
        /// </summary>
        /// <returns>A new configuration instance with default values.</returns>
        public static TimeTrackingConfiguration CreateDefault()
        {
            return new TimeTrackingConfiguration();
        }

        /// <summary>
        /// Gets the default database path for the current platform.
        /// </summary>
        /// <returns>The default database path.</returns>
        private string GetDefaultDatabasePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var soluiNetPath = Path.Combine(appDataPath, "SoluiNet", "DevTools");

            if (!Directory.Exists(soluiNetPath))
            {
                try
                {
                    Directory.CreateDirectory(soluiNetPath);
                }
                catch
                {
                    // Fall back to current directory if we can't create the app data directory
                    return Path.Combine(Environment.CurrentDirectory, "timetracking.db");
                }
            }

            return Path.Combine(soluiNetPath, "timetracking.db");
        }
    }
}