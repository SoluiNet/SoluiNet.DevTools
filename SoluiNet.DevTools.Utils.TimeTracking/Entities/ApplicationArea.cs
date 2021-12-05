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
    using SoluiNet.DevTools.Core.XmlData;

    /// <summary>
    /// The entity ApplicationArea.
    /// </summary>
    [Table("ApplicationArea")]
    public class ApplicationArea : IContainsExtendedConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationArea"/> class.
        /// </summary>
        public ApplicationArea()
        {
        }

        /// <summary>
        /// Gets or sets the ApplicationAreaId.
        /// </summary>
        public virtual int ApplicationAreaId { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationId.
        /// </summary>
        public virtual int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the AreaName.
        /// </summary>
        public virtual string AreaName { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public virtual Application Application { get; set; }

        /// <summary>
        /// Gets or sets the usage times.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2227:Collection properties should be read only",
            Justification = "This property will be used from Entity Framework. Just to be safe the setter shouldn't be removed")]
        public virtual ICollection<UsageTime> UsageTime { get; set; }

        /// <summary>
        /// Gets or sets the extended configuration.
        /// </summary>
        public virtual string ExtendedConfiguration { get; set; }
    }
}
