// <copyright file="Entry.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    /// <summary>
    /// The accounting entry.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }
    }
}
