// <copyright file="BaseRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using NHibernate;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Basic implementation of a repository.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class BaseRepository<T> : IRepository<T> 
        where T : BaseEntity
    {
        /// <summary>
        /// Add a record
        /// </summary>
        /// <param name="record">The record.</param>
        public void Add(T record)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(record);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Get a record by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The record which will be identified by the passed ID.</returns>
        public T Get(int id)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                return session.Get<T>(id);
            }
        }

        /// <summary>
        /// Get all records.
        /// </summary>
        /// <returns>Returns all records for the passed entity type.</returns>
        public ICollection<T> GetAll()
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                return session.Query<T>().ToList();
            }
        }

        /// <summary>
        /// Remove the passed record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Remove(T record)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(record);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Update the passed record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Update(T record)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(record);
                    transaction.Commit();
                }
            }
        }
    }
}
