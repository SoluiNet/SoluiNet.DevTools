﻿// <copyright file="Category.cs" company="SoluiNet">
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
        }

        /// <summary>
        /// Gets or sets the CategoryId.
        /// </summary>
        public virtual int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the CategoryName.
        /// </summary>
        public virtual string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the category usage time links.
        /// </summary>
        public virtual ICollection<CategoryUsageTime> CategoryUsageTime { get; set; }
    }
}
