// <copyright file="IGroupedUtilitiesDevPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    public interface IGroupedUtilitiesDevPlugin : IUtilitiesDevPlugin
    {
        /// <summary>
        /// Gets the label which should be used for group.
        /// </summary>
        string Group { get; }
    }
}
