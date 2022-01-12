// <copyright file="IHoldsBaseApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides an interface to identify classes that hold the base app instead of inheriting from them.
    /// </summary>
    public interface IHoldsBaseApp
    {
        /// <summary>
        /// Gets the base app.
        /// </summary>
        BaseSoluiNetApp BaseApp { get; }
    }
}
