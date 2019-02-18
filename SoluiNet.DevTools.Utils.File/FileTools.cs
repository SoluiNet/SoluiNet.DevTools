using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.File
{
    public class FileTools
    {
        public static List<string> ExtractLinesContainingSearchPattern(string filePath, string searchPattern)
        {
            var result = new List<string>();

            foreach (var line in System.IO.File.ReadAllLines(filePath))
            {
                if(line.Contains(searchPattern))
                    result.Add(line);
            }

            return result;
        }

        public static List<string> ExtractLinesMatchingRegEx(string filePath, string regExPattern)
        {
            var result = new List<string>();
            var regEx = new Regex(regExPattern);

            foreach (var line in System.IO.File.ReadAllLines(filePath))
            {
                if (regEx.IsMatch(line))
                    result.Add(line);
            }

            return result;
        }
    }
}
