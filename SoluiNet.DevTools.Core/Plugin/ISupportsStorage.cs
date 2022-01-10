// <copyright file="ISupportsStorage.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Support the storage functionality.
    /// </summary>
    public interface ISupportsStorage
    {
        /// <summary>
        /// Gets the entity name.
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        /// <remarks>
        /// Can be implemented via the returning of the type or a type definition class (TODO: has to be implemented).
        /// </remarks>
        object TypeDefinition { get; }
    }
}
