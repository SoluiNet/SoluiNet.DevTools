// <copyright file="StorageContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using SoluiNet.DevTools.Core.Storage.Events;

    /// <summary>
    /// Provides the context for the storage.
    /// </summary>
    public class StorageContext : DbContext
    {
        /// <summary>
        /// An event that will be called if the model is creating.
        /// </summary>
        public event EventHandler<ModelCreatingEventArgs> ModelCreating;

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(StorageRegistry.ConnectionString);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.ModelCreating?.Invoke(this, new ModelCreatingEventArgs() { ModelBuilder = modelBuilder });
        }
    }
}
