// <copyright file="Entry.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The accounting entry.
    /// </summary>
    public class Entry : BaseEntity
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the value date.
        /// </summary>
        public virtual DateTime ValueDate { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        public virtual string AdditionalInformation { get; set; }

        /// <summary>
        /// Gets or sets the sub entries.
        /// </summary>
        public virtual ICollection<SubEntry> SubEntries { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// Gets or sets the counter account.
        /// </summary>
        public virtual Account CounterAccount { get; set; }

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        public virtual Invoice Invoice { get; set; }
    }
}
