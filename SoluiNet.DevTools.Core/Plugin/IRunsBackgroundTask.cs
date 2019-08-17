// <copyright file="IRunsBackgroundTask.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.CommonMethods;

    /// <summary>
    /// Provides an interface for plugins with background tasks.
    /// </summary>
    public interface IRunsBackgroundTask : IBasePlugin, IProvidesBackgroundTask
    {
    }
}
