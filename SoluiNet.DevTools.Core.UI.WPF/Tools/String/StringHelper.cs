// <copyright file="StringHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.String
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Media;

    /// <summary>
    /// Provides a collection of methods which support working with strings.
    /// </summary>
    public static partial class StringHelper
    {
        /// <summary>
        /// Convert string to colour.
        /// </summary>
        /// <param name="colourString">The string which contains a colour definition.</param>
        /// <returns>Returns a <see cref="Color"/>-object which has been converted from the overgiven string.</returns>
        public static Color ToColour(this string colourString)
        {
            return (Color)ColorConverter.ConvertFromString(colourString);
        }
    }
}
