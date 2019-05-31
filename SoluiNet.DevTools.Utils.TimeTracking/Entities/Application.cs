// <copyright file="Application.cs" company="SoluiNet">
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
    /// The entity Application.
    /// </summary>
    [Table("Application")]
    public class Application
    {
        /// <summary>
        /// Gets or sets the ApplicationId.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationName.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the usage times.
        /// </summary>
        public ICollection<UsageTime> UsageTime { get; set; }

        /// <summary>
        /// Gets or sets the application areas.
        /// </summary>
        public ICollection<ApplicationArea> ApplicationArea { get; set; }
    }
}
