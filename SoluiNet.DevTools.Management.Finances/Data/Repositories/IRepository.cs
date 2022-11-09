// <copyright file="IRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for repositories.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Add a record to the repository.
        /// </summary>
        /// <param name="record">The record which should be added.</param>
        void Add(T record);

        /// <summary>
        /// Update a record in the repository.
        /// </summary>
        /// <param name="record">The record which should be updated.</param>
        void Update(T record);

        /// <summary>
        /// Remove a record from the repository.
        /// </summary>
        /// <param name="record">The record which should be removed.</param>
        void Remove(T record);

        /// <summary>
        /// Get record by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>Returns the record which has the passed ID.</returns>
        T Get(int id);

        /// <summary>
        /// Get all records of the repository.
        /// </summary>
        /// <returns>Returns all records from the repository.</returns>
        ICollection<T> GetAll();
    }
}
