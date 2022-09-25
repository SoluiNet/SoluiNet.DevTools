// <copyright file="IColourFactory.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Reference
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents an interface for colour definition factories.
    /// </summary>
    public interface IColourFactory
    {
        /// <summary>
        /// Gets a colour from RGB values.
        /// </summary>
        /// <param name="red">The red parts.</param>
        /// <param name="green">The green parts.</param>
        /// <param name="blue">The blue parts.</param>
        /// <returns>Returns an <see cref="IColour"/> which will be created with the passed colour parts.</returns>
        IColour FromRgb(byte red, byte green, byte blue);

        /// <summary>
        /// Gets a colour by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns an <see cref="IColour"/> which represents the passed name.</returns>
        IColour FromName(string name);
    }
}
