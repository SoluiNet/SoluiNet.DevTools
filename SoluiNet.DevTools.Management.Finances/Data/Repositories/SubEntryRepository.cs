// <copyright file="SubEntryRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using NHibernate;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a repository to handle financial entries.
    /// </summary>
    public class SubEntryRepository : BaseRepository<SubEntry>
    {
        /// <summary>
        /// Add a sub entry.
        /// </summary>
        /// <param name="subEntry">The sub entry.</param>
        public new void Add(SubEntry subEntry)
        {
            base.Add(subEntry);
        }

        /// <summary>
        /// Get a sub entry by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The sub entry that has the passed ID.</returns>
        public new SubEntry Get(int id)
        {
            return base.Get(id);
        }

        /// <summary>
        /// Get all sub entries.
        /// </summary>
        /// <returns>Returns a list of all sub entries.</returns>
        public new ICollection<SubEntry> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Remove a sub entry.
        /// </summary>
        /// <param name="subEntry">The sub entry.</param>
        public new void Remove(SubEntry subEntry)
        {
            base.Remove(subEntry);
        }

        /// <summary>
        /// Update a sub entry.
        /// </summary>
        /// <param name="subEntry">The sub entry.</param>
        public new void Update(SubEntry subEntry)
        {
            base.Update(subEntry);
        }
    }
}
