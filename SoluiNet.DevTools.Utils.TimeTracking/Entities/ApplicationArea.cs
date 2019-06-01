// <copyright file="ApplicationArea.cs" company="SoluiNet">
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
    /// The entity ApplicationArea.
    /// </summary>
    [Table("ApplicationArea")]
    public class ApplicationArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationArea"/> class.
        /// </summary>
        public ApplicationArea()
        {
            this.UsageTime = new HashSet<UsageTime>();
        }

        /// <summary>
        /// Gets or sets the ApplicationAreaId.
        /// </summary>
        public int ApplicationAreaId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationId.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationName.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        /// Gets or sets the usage times.
        /// </summary>
        public ICollection<UsageTime> UsageTime { get; set; }
    }
}
