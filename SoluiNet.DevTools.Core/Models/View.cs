using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Models
{
    public class View : IDatabaseElement
    {
        public string Name { get; set; }

        public string BodyDefinition { get; set; }
    }
}
