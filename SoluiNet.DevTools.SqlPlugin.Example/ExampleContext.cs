// <copyright file="ExampleContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SqlPlugin.Example
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The database context which can work with the example database.
    /// </summary>
    public partial class ExampleContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleContext"/> class.
        /// </summary>
        /// <param name="alternativeEnvironment">The alternative environment if chosen anything else than Default.</param>
        public ExampleContext(string alternativeEnvironment = "")
            : base("name=ExampleConnection" + (string.IsNullOrEmpty(alternativeEnvironment)
                       ? string.Empty
                       : "." + alternativeEnvironment))
        {
        }

        /// <summary>
        /// Gets or sets the customer accessor.
        /// </summary>
        public virtual DbSet<Entities.Customer> Customer { get; set; }

        /// <summary>
        /// Event handler for model creation.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Customer>()
                .Property(e => e.Email)
                .IsUnicode(false);
        }
    }
}
