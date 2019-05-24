// <copyright file="IDatabaseElement.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for a database element.
    /// </summary>
    public interface IDatabaseElement
    {
        /// <summary>
        /// Gets or sets the database element name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the defintion of the database element.
        /// </summary>
        string BodyDefinition { get; set; }
    }
}
