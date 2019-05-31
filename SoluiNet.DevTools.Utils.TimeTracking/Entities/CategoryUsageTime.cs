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
        /// Gets or sets the UsageTimeId.
        /// </summary>
        public int UsageTimeId { get; set; }

        /// <summary>
        /// Gets or sets the CategoryId.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the Duration.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets or sets the usage time.
        /// </summary>
        public UsageTime UsageTime { get; set; }
    }
}
