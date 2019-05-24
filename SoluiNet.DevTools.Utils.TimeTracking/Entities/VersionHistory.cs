// <copyright file="VersionHistory.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The entity VersionHistory.
    /// </summary>
    [Table("VersionHistory")]
    public class VersionHistory
    {
        /// <summary>
        /// Gets or sets the VersionHistoryId.
        /// </summary>
        public int VersionHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the VersionNumber.
        /// </summary>
        public string VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the AppliedDateTime.
        /// </summary>
        public DateTime AppliedDateTime { get; set; }
    }
}
