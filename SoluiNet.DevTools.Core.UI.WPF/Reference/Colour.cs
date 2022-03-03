// <copyright file="Colour.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Reference;

    /// <summary>
    /// Represents a colour.
    /// </summary>
    public class Colour : IColour
    {
        private Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="red">The red parts.</param>
        /// <param name="green">The green parts.</param>
        /// <param name="blue">The blue parts.</param>
        public Colour(byte red, byte green, byte blue)
        {
            this.color = Color.FromArgb(0x00, red, green, blue);
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
            if (transparency > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(transparency), "Transparency can be maximum 100%");
            }

            this.color = Color.FromArgb(transparency, red, green, blue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Colour(string name)
        {
            this.color = Color.FromName(name);
        }

        /// <summary>
        /// Gets the red parts in the colour.
        /// </summary>
        public byte Red
        {
            get
            {
                return this.color.R;
            }
        }

        /// <summary>
        /// Gets the green parts in the colour.
        /// </summary>
        public byte Green
        {
            get
            {
                return this.color.G;
            }
        }

        /// <summary>
        /// Gets the blue parts in the colour.
        /// </summary>
        public byte Blue
        {
            get
            {
                return this.color.B;
            }
        }

        /// <summary>
        /// Gets the transparency of the colour.
        /// </summary>
        public byte Transparency
        {
            get
            {
                return this.color.A;
            }
        }

        /// <summary>
        /// Gets the <see cref="Color"/> object.
        /// </summary>
        public Color Value
        {
            get
            {
                return this.color;
            }
        }

        /// <inheritdoc />
        public decimal Cyan
        {
            get
            {
                return ColourConverter.ToCmyk(this).cyan;
            }
        }

        /// <inheritdoc />
        public decimal Magenta
        {
            get
            {
                return ColourConverter.ToCmyk(this).magenta;
            }
        }

        /// <inheritdoc />
        public decimal Yellow
        {
            get
            {
                return ColourConverter.ToCmyk(this).yellow;
            }
        }

        /// <inheritdoc />
        public decimal Black
        {
            get
            {
                return ColourConverter.ToCmyk(this).black;
            }
        }
    }
}
