// <copyright file="EntryRepository.cs" company="SoluiNet">
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
    public class EntryRepository : IRepository<Entry>
    {
        /// <summary>
        /// Add a financial entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Add(Entry entry)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(entry);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Get a financial entry by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The financial entry that has the passed ID.</returns>
        public Entry Get(int id)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                return session.Get<Entry>(id);
            }
        }

        /// <summary>
        /// Get all finacial entries.
        /// </summary>
        /// <returns>Returns a list of all financial entries.</returns>
        public ICollection<Entry> GetAll()
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                return session.Query<Entry>().ToList();
            }
        }

        /// <summary>
        /// Remove a financial entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Remove(Entry entry)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(entry);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Update a financial entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Update(Entry entry)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(entry);
                    transaction.Commit();
                }
            }
        }
    }
}
