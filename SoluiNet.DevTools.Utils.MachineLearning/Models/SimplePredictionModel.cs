using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.MachineLearning.Models
{
    public class SimplePredictionModel<T>
    {
        [ColumnName("Score")]
        public T PredictionValue { get; set; }
    }
}
