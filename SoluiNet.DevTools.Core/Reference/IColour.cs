// <copyright file="IColour.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents an interface for colour definitions.
    /// </summary>
    public interface IColour
    {
        /// <summary>
        /// Gets the red parts in the colour.
        /// </summary>
        byte Red { get; }

        /// <summary>
        /// Gets the green parts in the colour.
        /// </summary>
        byte Green { get; }

        /// <summary>
        /// Gets the blue parts in the colour.
        /// </summary>
        byte Blue { get; }

        /// <summary>
        /// Gets the transparency of the colour.
        /// </summary>
        byte Transparency { get; }
    }
}
