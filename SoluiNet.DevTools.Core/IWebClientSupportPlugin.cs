using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoluiNet.DevTools.Core.Enums;

namespace SoluiNet.DevTools.Core
{
    public interface IWebClientSupportPlugin : IBasePlugin
    {
        WebClientFormatEnum Format { get; }

        WebClientTypeEnum Type { get; }
    }
}
