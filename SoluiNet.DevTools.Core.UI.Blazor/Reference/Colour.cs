// <copyright file="Colour.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Blazor.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a colour.
    /// </summary>
    public class Colour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="red">The red parts.</param>
        /// <param name="green">The green parts.</param>
        /// <param name="blue">The blue parts.</param>
        public Colour(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="red">The red parts.</param>
        /// <param name="green">The green parts.</param>
        /// <param name="blue">The blue parts.</param>
        /// <param name="transparency">The transparency.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws <see cref="ArgumentOutOfRangeException"/> if transparency is more than 100%.</exception>
        public Colour(byte red, byte green, byte blue, byte transparency)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;

            if (transparency > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(transparency), "Transparency can be maximum 100%");
            }
        }

        /// <summary>
        /// Gets the red parts in the colour.
        /// </summary>
        public byte Red { get; }

        /// <summary>
        /// Gets the green parts in the colour.
        /// </summary>
        public byte Green { get; }

        /// <summary>
        /// Gets the blue parts in the colour.
        /// </summary>
        public byte Blue { get; }

        /// <summary>
        /// Gets the transparency of the colour.
        /// </summary>
        public byte Transparency { get; }
    }
}
