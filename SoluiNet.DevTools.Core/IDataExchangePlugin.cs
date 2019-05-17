using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core
{
    public interface IDataExchangePlugin : IBasePlugin
    {
        List<object> GetData(string whereClause); // use dynamic LINQ?

        List<object> GetData(string entityName, IDictionary<string, object> searchData);

        object SetData(object identifier, IDictionary<string, object> valueData);
    }
}
