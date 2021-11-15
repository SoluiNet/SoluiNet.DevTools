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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "It should be clear that this type is an enum.")]
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
