// <copyright file="InvoiceRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using NHibernate;

    /// <summary>
    /// Provides a repository to handle categories.
    /// </summary>
    public class InvoiceRepository : BaseRepository<Invoice>
    {
        /// <summary>
        /// Add an invoice.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        public new void Add(Invoice invoice)
        {
            base.Add(invoice);
        }

        /// <summary>
        /// Get an invoice by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The invoice that has the passed ID.</returns>
        public new Invoice Get(int id)
        {
            return base.Get(id);
        }

        /// <summary>
        /// Get all invoices.
        /// </summary>
        /// <returns>Returns a list of all invoices.</returns>
        public new ICollection<Invoice> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Remove an invoice.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        public new void Remove(Invoice invoice)
        {
            base.Remove(invoice);
        }

        /// <summary>
        /// Update an invoice.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        public new void Update(Invoice invoice)
        {
            base.Update(invoice);
        }
    }
}
