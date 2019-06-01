// <copyright file="ISoluiNetUIElement.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.UIElement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for UI elements in SoluiNet.DevTools.
    /// </summary>
    public interface ISoluiNetUIElement
    {
        /// <summary>
        /// Gets the label.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the technical name.
        /// </summary>
        string TechnicalName { get; }
    }
}
