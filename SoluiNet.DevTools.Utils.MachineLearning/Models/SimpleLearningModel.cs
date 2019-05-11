using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.MachineLearning.Models
{
    public class SimpleLearningModel<T, O>
    {
        public T ReferenceValue { get; set; }
        public O DependentValue { get; set; }
    }
}
