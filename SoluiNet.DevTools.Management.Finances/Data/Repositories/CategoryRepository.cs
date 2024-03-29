﻿// <copyright file="CategoryRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// Provides a repository to handle categories.
    /// </summary>
    public class CategoryRepository : BaseRepository<Category>
    {
        /// <summary>
        /// Add a category.
        /// </summary>
        /// <param name="category">The category.</param>
        public new void Add(Category category)
        {
            base.Add(category);
        }

        /// <summary>
        /// Get a category by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The category that has the passed ID.</returns>
        public new Category Get(int id)
        {
            return base.Get(id);
        }

        /// <summary>
        /// Get all categories.
        /// </summary>
        /// <returns>Returns a list of all categories.</returns>
        public new ICollection<Category> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Remove a category.
        /// </summary>
        /// <param name="category">The category.</param>
        public new void Remove(Category category)
        {
            base.Remove(category);
        }

        /// <summary>
        /// Update a category.
        /// </summary>
        /// <param name="category">The category.</param>
        public new void Update(Category category)
        {
            base.Update(category);
        }

        /// <summary>
        /// Find a category by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the corresponding category.</returns>
        internal Category FindByName(string name)
        {
            using (ISession session = NHibernateContext.GetCurrentSession())
            {
                return session
                    .CreateCriteria<Category>()
                    .Add(Restrictions.Eq("Name", name))
                    .UniqueResult<Category>();
            }
        }
    }
}
