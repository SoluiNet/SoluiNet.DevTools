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

    public class SimpleLearningModel<TReference, TDependence>
    {
        public TReference ReferenceValue { get; set; }

        public TDependence DependentValue { get; set; }
    }
}
