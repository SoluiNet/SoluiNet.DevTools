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

    public class SimplePredictionModel<T>
    {
        [ColumnName("Score")]
        public T PredictionValue { get; set; }
    }
}
