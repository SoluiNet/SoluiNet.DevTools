// <copyright file="ColourConverter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Tools.Number;

    /// <summary>
    /// Provides a converter for colours.
    /// </summary>
    public static class ColourConverter
    {
        /// <summary>
        /// Convert a colour to CMYK.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>Returns a <see cref="Tuple{T1, T2, T3, T4}"/> which holds a CMYK colour.</returns>
        public static (decimal cyan, decimal magenta, decimal yellow, decimal black) ToCmyk(IColour colour)
        {
            if (colour == null)
            {
                throw new ArgumentNullException(nameof(colour));
            }

            decimal redParts = colour.Red / 255;
            decimal greenParts = colour.Green / 255;
            decimal blueParts = colour.Blue / 255;

            var black = 1 - NumberTools.Max(redParts, greenParts, blueParts);
            var cyan = (1 - redParts - black) / (1 - black);
            var magenta = (1 - greenParts - black) / (1 - black);
            var yellow = (1 - blueParts - black) / (1 - black);

            return (cyan, magenta, yellow, black);
        }

        /// <summary>
        /// Convert a colour to HSL.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>Returns a <see cref="Tuple{T1, T2, T3}"/> which holds a HSL colour.</returns>
        public static (decimal hue, decimal saturation, decimal lightness) ToHsl(IColour colour)
        {
            if (colour == null)
            {
                throw new ArgumentNullException(nameof(colour));
            }

            decimal redParts = colour.Red / 255;
            decimal greenParts = colour.Green / 255;
            decimal blueParts = colour.Blue / 255;

            var maxPart = NumberTools.Max(redParts, greenParts, blueParts);
            var minPart = NumberTools.Min(redParts, greenParts, blueParts);
            var delta = maxPart - minPart;

            decimal hue = 0;

            if (delta != 0)
            {
                if (maxPart == redParts)
                {
                    hue = 60 * (((greenParts - blueParts) / delta) % 6);
                }
                else if (maxPart == greenParts)
                {
                    hue = 60 * (((blueParts - redParts) / delta) + 2);
                }
                else if (maxPart == blueParts)
                {
                    hue = 60 * (((redParts - greenParts) / delta) + 4);
                }
            }

            if (hue < 0)
            {
                hue += 360;
            }

            decimal lightness = 0;

            if (delta != 0)
            {
                lightness = (maxPart + minPart) / 2;
            }

            decimal saturation = 0;

            if (delta != 0)
            {
                saturation = delta / (1 - Math.Abs((2 * lightness) - 1));
            }

            return (hue, saturation, lightness);
        }

        /// <summary>
        /// Convert a colour to HSV.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>Returns a <see cref="Tuple{T1, T2, T3}"/> which holds a HSV colour.</returns>
        public static (decimal hue, decimal saturation, decimal value) ToHsv(IColour colour)
        {
            if (colour == null)
            {
                throw new ArgumentNullException(nameof(colour));
            }

            decimal redParts = colour.Red / 255;
            decimal greenParts = colour.Green / 255;
            decimal blueParts = colour.Blue / 255;

            var maxPart = NumberTools.Max(redParts, greenParts, blueParts);
            var minPart = NumberTools.Min(redParts, greenParts, blueParts);
            var delta = maxPart - minPart;

            decimal hue = 0;

            if (delta != 0)
            {
                if (maxPart == redParts)
                {
                    hue = 60 * (((greenParts - blueParts) / delta) % 6);
                }
                else if (maxPart == greenParts)
                {
                    hue = 60 * (((blueParts - redParts) / delta) + 2);
                }
                else if (maxPart == blueParts)
                {
                    hue = 60 * (((redParts - greenParts) / delta) + 4);
                }
            }

            if (hue < 0)
            {
                hue += 360;
            }

            decimal value = maxPart;

            decimal saturation = 0;

            if (maxPart != 0)
            {
                saturation = delta / maxPart;
            }

            return (hue, saturation, value);
        }
    }
}
