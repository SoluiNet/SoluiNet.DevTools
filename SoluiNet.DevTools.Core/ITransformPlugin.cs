using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core
{
    public interface ITransformPlugin : IBasePlugin
    {
        List<string> SupportedInputFormats { get; }

        List<string> SupportedOutputFormats { get; }

        object Transform(string inputFile, string preferedOutputFormat);
        object Transform(string inputStream, string inputFormat, string preferedOutputFormat);
    }
}
