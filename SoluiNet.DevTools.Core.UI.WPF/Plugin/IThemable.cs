﻿// <copyright file="IThemable.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Reference;

    /// <summary>
    /// Provides an interface for a plugin which can be themed with colour variations.
    /// </summary>
    public interface IThemable : IBasePlugin
    {
        /// <summary>
        /// Gets the first colour accent.
        /// </summary>
        IColour AccentColour1 { get; }

        /// <summary>
        /// Gets the second colour accent.
        /// </summary>
        IColour AccentColour2 { get; }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        IColour ForegroundColour { get; }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        IColour BackgroundColour { get; }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        IColour BackgroundAccentColour { get; }
    }
}
