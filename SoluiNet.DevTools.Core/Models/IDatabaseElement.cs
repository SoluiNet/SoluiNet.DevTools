using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Models
{
    public interface IDatabaseElement
    {
        string Name { get; set; }

        string BodyDefinition { get; set; }
    }
}
