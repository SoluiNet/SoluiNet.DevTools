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
    /// The title change delegate.
    /// </summary>
    /// <param name="sender">The sending element.</param>
    /// <param name="titleParts">The changed title parts.</param>
    public delegate void TitleChangedHandler(object sender, Dictionary<string, string> titleParts);

    /// <summary>
    /// Provides an interface for UI elements in SoluiNet.DevTools.
    /// </summary>
    public interface ISoluiNetUIElement
    {
        /// <summary>
        /// The event handler for a title change.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1003:Use generic event handler instances",
            Justification = "We need the custom event handler for the additional parameters that could be delivered.")]
        event TitleChangedHandler TitleChanged;

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
