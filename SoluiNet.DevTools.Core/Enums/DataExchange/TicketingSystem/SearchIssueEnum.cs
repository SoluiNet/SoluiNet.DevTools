// <copyright file="SearchIssueEnum.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Enums.DataExchange.TicketingSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An enumeration which provides a list of all possible searches in ticketing systems.
    /// </summary>
    public enum SearchIssueEnum
    {
        /// <summary>
        /// All assigned tickets to current user
        /// </summary>
        AssignedToCurrentUser,

        /// <summary>
        /// All open assigned tickets to current user
        /// </summary>
        OpenAssignedToCurrentUser,

        /// <summary>
        /// All open tickets for a specific project
        /// </summary>
        OpenForProject,

        /// <summary>
        /// All tickets for a specific project
        /// </summary>
        SelectByProject,
    }
}
