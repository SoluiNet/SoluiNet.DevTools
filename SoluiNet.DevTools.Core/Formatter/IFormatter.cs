using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Formatter
{
    public interface IFormatter
    {
        string FormatString(string originalString);
    }
}
