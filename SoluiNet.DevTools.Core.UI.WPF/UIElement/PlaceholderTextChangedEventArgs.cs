// <copyright file="PlaceholderTextChangedEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.UIElement
{
    using System;

    /// <summary>
    /// The event arguments which should be delivered if the placeholder text changed.
    /// </summary>
    public class PlaceholderTextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceholderTextChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldPlaceholderText">The old placeholder text.</param>
        /// <param name="newPlaceholderText">The new placeholder text.</param>
        public PlaceholderTextChangedEventArgs(string oldPlaceholderText, string newPlaceholderText)
        {
            this.OldPlaceholderText = oldPlaceholderText;
            this.NewPlaceholderText = newPlaceholderText;
        }

        /// <summary>
        /// Gets or sets the old placeholder text.
        /// </summary>
        public string OldPlaceholderText { get; set; }

        /// <summary>
        /// Gets or sets the new placeholder text.
        /// </summary>
        public string NewPlaceholderText { get; set; }
    }
}
