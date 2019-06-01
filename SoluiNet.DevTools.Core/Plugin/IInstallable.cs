// <copyright file="IInstallable.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the interface for an installable plugin.
    /// </summary>
    public interface IInstallable : IBasePlugin
    {
        /// <summary>
        /// Install the plugin via calling this method.
        /// </summary>
        void Install();
    }
}
