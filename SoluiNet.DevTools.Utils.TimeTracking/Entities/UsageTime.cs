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
        }

        /// <summary>
        /// Gets or sets the UsageTimeId.
        /// </summary>
        public virtual int UsageTimeId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationIdentification.
        /// </summary>
        public virtual string ApplicationIdentification { get; set; }

        /// <summary>
        /// Gets or sets the StartTime.
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        public virtual int Duration { get; set; }

        /// <summary>
        /// Gets or sets additional information.
        /// </summary>
        public virtual string AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationId.
        /// </summary>
        public virtual int? ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationAreaId.
        /// </summary>
        public virtual int? ApplicationAreaId { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public virtual Application Application { get; set; }

        /// <summary>
        /// Gets or sets the application area.
        /// </summary>
        public virtual ApplicationArea ApplicationArea { get; set; }

        /// <summary>
        /// Gets or sets the category usage time links.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2227:Collection properties should be read only",
            Justification = "This property will be used from Entity Framework. Just to be safe the setter shouldn't be removed")]
        public virtual ICollection<CategoryUsageTime> CategoryUsageTime { get; set; }
    }
}
