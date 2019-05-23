// <copyright file="IPluginWithBackgroundTask.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPluginWithBackgroundTask : IBasePlugin
    {
        Task ExecuteBackgroundTask();
    }
}
