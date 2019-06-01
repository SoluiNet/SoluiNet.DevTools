// <copyright file="Category.cs" company="SoluiNet">
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
    /// The entity Category.
    /// </summary>
    [Table("Category")]
    public class Category
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        public Category()
        {
            this.CategoryUsageTime = new HashSet<CategoryUsageTime>();
        }

        /// <summary>
        /// Gets or sets the CategoryId.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the CategoryName.
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the category usage time links.
        /// </summary>
        public ICollection<CategoryUsageTime> CategoryUsageTime { get; set; }
    }
}
