// <copyright file="ColourFactory.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.Reference;

    /// <summary>
    /// Represents a colour.
    /// </summary>
    public class ColourFactory : IColourFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColourFactory"/> class.
        /// </summary>
        public ColourFactory()
        {
        }

        /// <summary>
        /// Gets a colour from RGB values.
        /// </summary>
        /// <param name="red">The red parts.</param>
        /// <param name="green">The green parts.</param>
        /// <param name="blue">The blue parts.</param>
        /// <returns>Returns a <see cref="IColour"/> which will be created with the passed colour parts.</returns>
        public IColour FromRgb(byte red, byte green, byte blue)
        {
            return new Colour(red, green, blue);
        }

        /// <summary>
        /// Gets a colour by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns a <see cref="IColour"/> which represents the passed name.</returns>
        public IColour FromName(string name)
        {
            switch (name)
            {
                case "Blue":
                    return new Colour(0x00, 0x00, 0xFF);
                case "Green":
                    return new Colour(0x00, 0xFF, 0x00);
                case "Red":
                    return new Colour(0xFF, 0x00, 0x00);
                case "White":
                    return new Colour(0xFF, 0xFF, 0xFF);
                case "Yellow":
                    return new Colour(0xFF, 0xFF, 0x00);
                case "Black":
                default:
                    return new Colour(0x00, 0x00, 0x00);
            }
        }
    }
}
