// <copyright file="SimplePredictionModel.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.MachineLearning.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ML.Data;

    /// <summary>
    /// A model for prediction for simple learning purposes.
    /// </summary>
    /// <typeparam name="T">The type of the prediction value.</typeparam>
    public class SimplePredictionModel<T>
    {
        /// <summary>
        /// Gets or sets the prediction value.
        /// </summary>
        [ColumnName("Score")]
        public T PredictionValue { get; set; }
    }
}
