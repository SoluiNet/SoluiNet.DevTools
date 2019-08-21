// <copyright file="RemovableUserControl.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.UIElement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The base class for any UI element which can be removed.
    /// </summary>
    public class RemovableUserControl : UserControl
    {
        /// <summary>
        /// The delegate for the removal of this UI element.
        /// </summary>
        public delegate void ElementRemoval();

        /// <summary>
        /// Gets or sets the event handler for the removal of this UI element.
        /// </summary>
        public ElementRemoval RemoveElement { get; set; }
    }
}
