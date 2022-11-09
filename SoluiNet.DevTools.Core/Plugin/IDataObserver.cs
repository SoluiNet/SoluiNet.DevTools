// <copyright file="IDataObserver.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Plugin.Data;

    /// <summary>
    /// Provides an interface for observing data.
    /// </summary>
    public interface IDataObserver : IBasePlugin, IObserver<IDataContainer>
    {
    }
}
