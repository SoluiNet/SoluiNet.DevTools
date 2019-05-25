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

    public partial class Customer
    {
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Firstname { get; set; }

        [Key]
        public int Customernumber { get; set; }

        public DateTime? Created { get; set; }

        public bool? LockedOut { get; set; }

        [StringLength(100)]
        public string Lastname { get; set; }
    }
}
