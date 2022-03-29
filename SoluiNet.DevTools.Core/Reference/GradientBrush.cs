// <copyright file="GradientBrush.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The gradient brush.
    /// </summary>
    public class GradientBrush : IBrush
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientBrush"/> class.
        /// </summary>
        /// <param name="firstColour">The first colour.</param>
        /// <param name="secondColour">The second colour.</param>
        /// <param name="angle">The angle.</param>
        public GradientBrush(IColour firstColour, IColour secondColour, decimal angle)
        {
            this.Parts = new List<GradientBrushPart>();

            this.Parts.Add(new GradientBrushPart() { Colour = firstColour });
            this.Parts.Add(new GradientBrushPart() { Colour = secondColour });

            this.Angle = angle;
        }

        /// <summary>
        /// The gradient brush part.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Will be used later.")]
        public class GradientBrushPart
        {
            /// <summary>
            /// Gets or sets the colour.
            /// </summary>
            public IColour Colour { get; set; }

            /// <summary>
            /// Gets or sets the offset.
            /// </summary>
            public double Offset { get; set; }
        }

        /// <summary>
        /// Gets the colours.
        /// </summary>
        public ICollection<GradientBrushPart> Parts { get; }

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        public decimal Angle { get; set; }
    }
}
