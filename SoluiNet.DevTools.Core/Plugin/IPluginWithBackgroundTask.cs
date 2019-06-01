// <copyright file="IPluginWithBackgroundTask.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface for plugins with background tasks.
    /// </summary>
    public interface IPluginWithBackgroundTask : IBasePlugin
    {
        /// <summary>
        /// Execute the background task.
        /// </summary>
        /// <returns>Returns the background task which should be executed.</returns>
        Task ExecuteBackgroundTask();
    }
}
