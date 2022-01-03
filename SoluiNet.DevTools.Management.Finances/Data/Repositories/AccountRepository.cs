// <copyright file="AccountRepository.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data.Repositories
{
    using NHibernate;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a repository to handle categories.
    /// </summary>
    public class AccountRepository : BaseRepository<Account>
    {
        /// <summary>
        /// Add an account.
        /// </summary>
        /// <param name="account">The account.</param>
        public new void Add(Account account)
        {
            base.Add(account);
        }

        /// <summary>
        /// Get an account by ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The account that has the passed ID.</returns>
        public new Account Get(int id)
        {
            return base.Get(id);
        }

        /// <summary>
        /// Get all accounts.
        /// </summary>
        /// <returns>Returns a list of all accounts.</returns>
        public new ICollection<Account> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Remove an account.
        /// </summary>
        /// <param name="account">The account.</param>
        public new void Remove(Account account)
        {
            base.Remove(account);
        }

        /// <summary>
        /// Update a account.
        /// </summary>
        /// <param name="account">The account.</param>
        public new void Update(Account account)
        {
            base.Update(account);
        }
    }
}
