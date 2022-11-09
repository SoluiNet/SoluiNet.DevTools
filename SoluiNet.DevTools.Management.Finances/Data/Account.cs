// <copyright file="Account.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    /// <summary>
    /// The account.
    /// </summary>
    public class Account : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the Business Identifier Code.
        /// </summary>
        public virtual string BIC { get; set; }

        /// <summary>
        /// Gets or sets the International Bank Account Number.
        /// </summary>
        public virtual string IBAN { get; set; }

        /// <summary>
        /// Gets or sets the bank.
        /// </summary>
        public virtual Bank Bank { get; set; }
    }
}
