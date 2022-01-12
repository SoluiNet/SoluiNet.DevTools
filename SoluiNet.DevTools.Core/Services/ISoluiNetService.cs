// <copyright file="ISoluiNetService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides an interface for all services.
    /// </summary>
    public interface ISoluiNetService
    {
        /// <summary>
        /// Gets the service name.
        /// </summary>
        string Name { get; }
    }
}