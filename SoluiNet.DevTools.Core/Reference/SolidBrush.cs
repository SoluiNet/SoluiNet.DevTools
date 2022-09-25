// <copyright file="SolidBrush.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The solid brush.
    /// </summary>
    public class SolidBrush : IBrush
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolidBrush"/> class.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public SolidBrush(IColour colour)
        {
            this.Colour = colour;
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        public IColour Colour { get; set; }
    }
}
