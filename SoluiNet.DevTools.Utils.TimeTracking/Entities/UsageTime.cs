// <copyright file="UsageTime.cs" company="SoluiNet">
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
    /// The entity UsageTime.
    /// </summary>
    [Table("UsageTime")]
    public class UsageTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageTime"/> class.
        /// </summary>
        public UsageTime()
        {
            this.CategoryUsageTime = new HashSet<CategoryUsageTime>();
        }

        /// <summary>
        /// Gets or sets the UsageTimeId.
        /// </summary>
        public int UsageTimeId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationIdentification.
        /// </summary>
        public string ApplicationIdentification { get; set; }

        /// <summary>
        /// Gets or sets the StartTime.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationId.
        /// </summary>
        public int? ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationAreaId.
        /// </summary>
        public int? ApplicationAreaId { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        /// Gets or sets the application area.
        /// </summary>
        public ApplicationArea ApplicationArea { get; set; }

        /// <summary>
        /// Gets or sets the category usage time links.
        /// </summary>
        public ICollection<CategoryUsageTime> CategoryUsageTime { get; set; }
    }
}
