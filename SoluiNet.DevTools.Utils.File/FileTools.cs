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

            try
            {
                foreach (var line in System.IO.File.ReadAllLines(filePath))
                {
                    if (line.Contains(searchPattern))
                        result.Add(line);
                }
            }
            catch(Exception exception)
            {
                result.Add(string.Format("##ERROR## Couldn't extract lines from file {0} - {1} ##ERROR##", filePath, exception.Message));
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

        public static string CalculateChecksum(string hashType, string filePath)
        {
            //// todo: get hash type from enumeration

            if (!System.IO.File.Exists(filePath))
            {
                return string.Empty;
            }

            if (hashType.ToLowerInvariant() == "md5")
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = System.IO.File.OpenRead(filePath))
                    {
                        var hash = md5.ComputeHash(stream);

                        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                    }
                }
            }

            return string.Empty;
        }
    }
}
