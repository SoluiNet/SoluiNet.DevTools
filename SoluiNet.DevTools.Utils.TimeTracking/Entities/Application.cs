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
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
        {
        }

        /// <summary>
        /// Gets or sets the ApplicationId.
        /// </summary>
        public virtual int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationName.
        /// </summary>
        public virtual string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the extended configuration.
        /// </summary>
        public virtual string ExtendedConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the usage times.
        /// </summary>
        public virtual ICollection<UsageTime> UsageTime { get; set; }

        /// <summary>
        /// Gets or sets the application areas.
        /// </summary>
        public virtual ICollection<ApplicationArea> ApplicationArea { get; set; }
    }
}
