// <copyright file="SystemInfo.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Entity for storing system information and platform metadata.
    /// </summary>
    [Table("SystemInfo")]
    public class SystemInfo
    {
        /// <summary>
        /// Gets or sets the SystemInfoId.
        /// </summary>
        public virtual int SystemInfoId { get; set; }

        /// <summary>
        /// Gets or sets the platform name (Windows, Linux, macOS).
        /// </summary>
        public virtual string Platform { get; set; }

        /// <summary>
        /// Gets or sets the platform version.
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// Gets or sets the system architecture (x64, x86, ARM64).
        /// </summary>
        public virtual string Architecture { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this information was recorded.
        /// </summary>
        public virtual DateTime RecordedAt { get; set; }

        /// <summary>
        /// Gets or sets additional platform-specific metadata as JSON.
        /// </summary>
        public virtual string AdditionalMetadata { get; set; }
    }
}