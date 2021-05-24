// <copyright file="CategoryUsageTime.cs" company="SoluiNet">
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
    /// The entity CategoryUsageTime.
    /// </summary>
    [Table("Category_UsageTime")]
    public class CategoryUsageTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryUsageTime"/> class.
        /// </summary>
        public CategoryUsageTime()
        {
        }

        /// <summary>
        /// Gets or sets the UsageTimeId.
        /// </summary>
        public virtual int UsageTimeId { get; set; }

        /// <summary>
        /// Gets or sets the CategoryId.
        /// </summary>
        public virtual int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        public virtual double Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this record was distributed evenly or has been manually assigned.
        /// </summary>
        public virtual bool? DistributedEvenly { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// Gets or sets the usage time.
        /// </summary>
        public virtual UsageTime UsageTime { get; set; }
    }
}
