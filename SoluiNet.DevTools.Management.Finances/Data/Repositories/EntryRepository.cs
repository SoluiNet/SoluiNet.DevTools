// <copyright file="EntryRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using NHibernate;

    /// <summary>
    /// Provides a repository to handle financial entries.
    /// </summary>
    public class EntryRepository : BaseRepository<Entry>
    {
        /// <summary>
        /// Add a financial entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public new void Add(Entry entry)
        {
            base.Add(entry);
        }

        /// <summary>
        /// Get a financial entry by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The financial entry that has the passed ID.</returns>
        public new Entry Get(int id)
        {
            return base.Get(id);
        }

        /// <summary>
        /// Get all finacial entries.
        /// </summary>
        /// <returns>Returns a list of all financial entries.</returns>
        public new ICollection<Entry> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Remove a financial entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public new void Remove(Entry entry)
        {
            base.Remove(entry);
        }

        /// <summary>
        /// Update a financial entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public new void Update(Entry entry)
        {
            base.Update(entry);
        }
    }
}
