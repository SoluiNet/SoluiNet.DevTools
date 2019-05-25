// <copyright file="Customer.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SqlPlugin.Example.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// The entity customer.
    /// </summary>
    public partial class Customer
    {
        /// <summary>
        /// Gets or sets the e-mail.
        /// </summary>
        [StringLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [StringLength(100)]
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the customer number.
        /// </summary>
        [Key]
        public int Customernumber { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets a value which indicates if the user has been locked out.
        /// </summary>
        public bool? LockedOut { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [StringLength(100)]
        public string Lastname { get; set; }
    }
}
