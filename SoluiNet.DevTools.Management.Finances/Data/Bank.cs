// <copyright file="Bank.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    /// <summary>
    /// The category.
    /// </summary>
    public class Bank : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the Business Identifier Code.
        /// </summary>
        public virtual string BIC { get; set; }
    }
}
