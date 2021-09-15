// <copyright file="StorageRegistry.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Storage
{
    using System;
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
    }
}
