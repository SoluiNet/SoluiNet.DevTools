// <copyright file="SearchCommit.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Enums.DataExchange.SourceCodeRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An enumeration which provides a list of all possible searches in source code repositories.
    /// </summary>
    public enum SearchCommit
    {
        /// <summary>
        /// All commits
        /// </summary>
        AllCommited,

        /// <summary>
        /// All commits by current user
        /// </summary>
        CommitedByCurrentUser,

        /// <summary>
        /// All commits by current user in a specific branch
        /// </summary>
        CommitedByCurrentUserInBranch,

        /// <summary>
        /// All commits in a specific branch
        /// </summary>
        CommitedInBranch,

        /// <summary>
        /// All commits of the last week
        /// </summary>
        CommitedLastWeek,
    }
}
