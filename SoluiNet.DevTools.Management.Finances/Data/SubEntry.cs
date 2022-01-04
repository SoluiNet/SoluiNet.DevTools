// <copyright file="SubEntry.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    /// <summary>
    /// A part of an accounting entry.
    /// </summary>
    public class SubEntry : BaseEntity
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        public virtual Entry Entry { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public virtual Category Category { get; set; }
    }
}
