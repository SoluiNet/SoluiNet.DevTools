// <copyright file="FilterHistory.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The entity filter history.
    /// </summary>
    [Table("VersionHistory")]
    public class FilterHistory
    {
        /// <summary>
        /// Gets or sets the filter history ID.
        /// </summary>
        public virtual long FilterHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the filter string.
        /// </summary>
        public virtual string FilterString { get; set; }

        /// <summary>
        /// Gets or sets the last execution date.
        /// </summary>
        public virtual DateTime LastExecutionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the execution user.
        /// </summary>
        public virtual string ExecutionUser { get; set; }
    }
}
