// <copyright file="Invoice.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    /// <summary>
    /// The invoice.
    /// </summary>
    public class Invoice : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the storage location.
        /// </summary>
        public virtual string StorageLocation { get; set; }
    }
}
