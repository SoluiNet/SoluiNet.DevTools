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
        /// Gets the cyan parts in the colour.
        /// </summary>
        decimal Cyan { get; }

        /// <summary>
        /// Gets the magenta parts in the colour.
        /// </summary>
        decimal Magenta { get; }

        /// <summary>
        /// Gets the yellow parts in the colour.
        /// </summary>
        decimal Yellow { get; }

        /// <summary>
        /// Gets the black parts in the colour.
        /// </summary>
        decimal Black { get; }

        /// <summary>
        /// Gets the transparency of the colour.
        /// </summary>
        byte Transparency { get; }
    }
}
