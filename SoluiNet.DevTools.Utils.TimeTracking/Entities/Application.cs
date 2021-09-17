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
    using SoluiNet.DevTools.Core.XmlData;

    /// <summary>
    /// The entity Application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1724:Type names should not match namespaces", Justification = "We want to use keywords for better unterstanding of the purpose.")]
    [Table("Application")]
    public class Application : IContainsExtendedConfiguration
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setter may be needed for ORM.")]
        public virtual ICollection<UsageTime> UsageTime { get; set; }

        /// <summary>
        /// Gets or sets the application areas.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setter may be needed for ORM.")]
        public virtual ICollection<ApplicationArea> ApplicationArea { get; set; }
    }
}
