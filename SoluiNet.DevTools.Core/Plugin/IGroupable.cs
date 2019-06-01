// <copyright file="IGroupable.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    /// <summary>
    /// Provides an interface for a plugin which should be grouped.
    /// </summary>
    public interface IGroupable : IBasePlugin
    {
        /// <summary>
        /// Gets the label which should be used for group.
        /// </summary>
        string Group { get; }
    }
}
