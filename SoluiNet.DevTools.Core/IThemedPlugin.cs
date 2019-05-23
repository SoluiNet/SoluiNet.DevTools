// <copyright file="IThemedPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    /// <summary>
    /// Provides an interface for a plugin which can be themed with colour variations.
    /// </summary>
    public interface IThemedPlugin : IBasePlugin
    {
        /// <summary>
        /// Gets the first colour accent.
        /// </summary>
        Color AccentColour1 { get; }

        /// <summary>
        /// Gets the second colour accent.
        /// </summary>
        Color AccentColour2 { get; }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        Color ForegroundColour { get; }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        Color BackgroundColour { get; }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        Color BackgroundAccentColour { get; }
    }
}
