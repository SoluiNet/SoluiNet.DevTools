﻿// <copyright file="ISmartHomeUiPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Blazor.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// Provides the interface for a plugin which will provide UI elements for Smart Home functions.
    /// </summary>
    public interface ISmartHomeUiPlugin : IBlazorPlugin, IThemable, ISupportsStorage
    {
    }
}
