using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Tools.String
{
    public class StringHelper
    {
        public static string ReplaceFirstOccurence(string inputString, string searchPattern, string replacementValue = "")
        {
            var regEx = new Regex(Regex.Escape(searchPattern));

            return regEx.Replace(inputString, replacementValue, 1);
        }

        public static string PrepareHeaderLabel(string headerLabel, bool removeEntityStructure = true)
        {
            var headerLabelString = headerLabel;

            if (headerLabelString.Contains('.') && removeEntityStructure)
            {
                var positionOfLastDot = headerLabelString.LastIndexOf('.');

                headerLabelString = headerLabelString.Substring(positionOfLastDot + 1, headerLabelString.Length - positionOfLastDot - 1);
            }

            return headerLabelString.Replace("_", "__");
        }
    }
}
