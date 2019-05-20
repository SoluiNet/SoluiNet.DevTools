using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoluiNet.DevTools.Core.UI
{
    public class SoluiNetWindow : Window
    {
        public string TitleFormatString { get; set; }

        public void SetTitleParts(IDictionary<string, string> titleParts)
        {
            if (titleParts == null || !titleParts.Any())
            {
                return;
            }

            Title = string.Format(TitleFormatString, titleParts.OrderBy(x => x.Key).Select(x => x.Value).ToArray());
        }
    }
}
