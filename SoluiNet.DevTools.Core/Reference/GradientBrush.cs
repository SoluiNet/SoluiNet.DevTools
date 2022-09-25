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
        /// Initializes a new instance of the <see cref="GradientBrush"/> class.
        /// </summary>
        /// <param name="colours">The colours.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        public GradientBrush(ICollection<IColour> colours, decimal angle, GradientPoint startPoint = null, GradientPoint endPoint = null)
        {
            if (colours == null)
            {
                throw new ArgumentNullException(nameof(colours));
            }

            this.Parts = new List<GradientBrushPart>();

            var i = 1;

            foreach (var colour in colours)
            {
                (this.Parts as List<GradientBrushPart>).Add(new GradientBrushPart()
                {
                    Colour = colour,
                    Offset = (1 / colours.Count) * i++,
                });
            }

            this.Angle = angle;

            if (startPoint != null)
            {
                this.StartPoint = startPoint;
            }
            else
            {
                this.StartPoint.X = 0;
                this.StartPoint.Y = 0;
            }

            if (endPoint != null)
            {
                this.EndPoint = endPoint;
            }
            else
            {
                this.EndPoint.X = 1;
                this.EndPoint.Y = 0.75m;
            }
        }

        /// <summary>
        /// Gets the colours.
        /// </summary>
        public ICollection<GradientBrushPart> Parts { get; }

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        public decimal Angle { get; set; }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        public GradientPoint StartPoint { get; set; }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        public GradientPoint EndPoint { get; set; }

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
        /// The gradient point.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Will be used later.")]
        public class GradientPoint
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GradientPoint"/> class.
            /// </summary>
            /// <param name="x">The X coordinate.</param>
            /// <param name="y">The Y coordinate.</param>
            public GradientPoint(decimal x, decimal y)
            {
                this.X = x;
                this.Y = y;
            }

            /// <summary>
            /// Gets or sets the X coordinate.
            /// </summary>
            public decimal X { get; set; }

            /// <summary>
            /// Gets or sets the Y coordinate.
            /// </summary>
            public decimal Y { get; set; }
        }
    }
}
