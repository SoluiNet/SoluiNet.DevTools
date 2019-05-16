using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core
{
    public interface IDataExchangePlugin
    {
        List<object> GetData(string whereClause); // use dynamic LINQ?

        object SetData(object identifier, Dictionary<string, object> valueData);
    }
}
