using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Tools.Json
{
    public static class JsonTools
    {
        public static string Serialize(object serializableObject)
        {
            return JsonConvert.SerializeObject(serializableObject);
        }
    }
}
