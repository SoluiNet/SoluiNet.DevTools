// <copyright file="ColourExtensions.cs" company="SoluiNet">
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
    public static class ColourExtensions
    {
        /// <summary>
        /// Gets the hex value for the colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>Returns a <see cref="string"/> which contains the colour in following format: <c>#RRGGBB</c>.</returns>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the colour is null.</exception>
        public static string AsHexValue(this IColour colour)
        {
            if (colour == null)
            {
                throw new ArgumentNullException(nameof(colour));
            }

            return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}", colour.Red, colour.Green, colour.Blue);
        }

        /// <summary>
        /// Gets the hex value for the colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>Returns a <see cref="Color"/> object.</returns>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the colour is null.</exception>
        public static Color AsColor(this IColour colour)
        {
            if (colour == null)
            {
                throw new ArgumentNullException(nameof(colour));
            }

            if(!(colour is Colour))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} has not the correct type {1}", nameof(colour), typeof(Colour).FullName));
            }

            return (colour as Colour).Value;
        }
    }
}
