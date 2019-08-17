// <copyright file="IProvidesBackgroundTask.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.CommonMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Declares an interface which allows to execute a background task.
    /// </summary>
    public interface IProvidesBackgroundTask
    {
        /// <summary>
        /// Execute the background task.
        /// </summary>
        /// <returns>Returns the background task which should be executed.</returns>
        Task ExecuteBackgroundTask();
    }
}
