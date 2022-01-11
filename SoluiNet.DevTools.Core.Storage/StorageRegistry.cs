// <copyright file="StorageRegistry.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Storage
{
    using System;
    using System.Collections.Generic;
    using SoluiNet.DevTools.Core.Configuration;

    /// <summary>
    /// Provides a registry for all storage activities.
    /// </summary>
    public static class StorageRegistry
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public static string ConnectionString
        {
            get { return Core.Configuration.Configuration.GetConfigurationEntry("StorageConnectionString"); }
        }

        /// <summary>
        /// Store data to the storage.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="data">The data.</param>
        public static void StoreData(string entity, ICollection<IDictionary<string, object>> data)
        {
            // todo: store the data to the entity.
        }
    }
}
