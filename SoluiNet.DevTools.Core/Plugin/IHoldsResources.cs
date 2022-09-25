// <copyright file="IHoldsResources.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides an interface for a plugin that is holding resources.
    /// </summary>
    public interface IHoldsResources : IBasePlugin
    {
        /// <summary>
        /// Gets the resources.
        /// </summary>
        public Dictionary<string, ICollection<object>> Resources { get; }
    }
}
