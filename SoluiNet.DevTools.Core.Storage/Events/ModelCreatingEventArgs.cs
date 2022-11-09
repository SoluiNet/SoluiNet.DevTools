// <copyright file="ModelCreatingEventArgs.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Storage.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The event arguments for model creating.
    /// </summary>
    public class ModelCreatingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the model builder.
        /// </summary>
        public ModelBuilder ModelBuilder { get; set; }
    }
}
