// <copyright file="IBrushFactory.cs" company="SoluiNet">
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
    public interface IBrushFactory
    {
        /// <summary>
        /// Create a solid brush instance.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <returns>Returns an <see cref="IBrush"/> which will represent a solid brush with the passed <paramref name="colour"/>.</returns>
        SolidBrush CreateSolidBrush(IColour colour);

        /// <summary>
        /// Create a gradient brush instance.
        /// </summary>
        /// <param name="firstColour">The first colour.</param>
        /// <param name="secondColour">The second colour.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>Returns an <see cref="IBrush"/> which will represent a gradient brush from the <paramref name="firstColour"/>
        /// to the <paramref name="secondColour"/> with an angle of <paramref name="angle"/>.</returns>
        GradientBrush CreateGradientBrush(IColour firstColour, IColour secondColour, decimal angle);
    }
}
