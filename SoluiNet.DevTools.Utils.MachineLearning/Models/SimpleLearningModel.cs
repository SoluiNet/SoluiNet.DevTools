// <copyright file="SimpleLearningModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.MachineLearning.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A model for simple learning purposes.
    /// </summary>
    /// <typeparam name="TReference">The type for the reference value.</typeparam>
    /// <typeparam name="TDependence">The type for the dependent value.</typeparam>
    public class SimpleLearningModel<TReference, TDependence>
    {
        /// <summary>
        /// Gets or sets the reference value.
        /// </summary>
        public TReference ReferenceValue { get; set; }

        /// <summary>
        /// Gets or sets the dependent value.
        /// </summary>
        public TDependence DependentValue { get; set; }
    }
}
