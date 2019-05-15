using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core
{
    public interface IPluginWithBackgroundTask : IBasePlugin
    {
        Task ExecuteBackgroundTask();
    }
}
