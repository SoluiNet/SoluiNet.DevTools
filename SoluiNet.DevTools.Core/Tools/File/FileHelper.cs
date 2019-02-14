using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Tools.File
{
    public class FileHelper
    {
        /// <summary>
        /// Load the content from the overgiven file path to a string
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>The file content</returns>
        public static string StringFromFile(string path)
        {
            if (!System.IO.File.Exists(path))
                return string.Empty;

            // Open the file to read from.
            return System.IO.File.ReadAllText(path);
        }
    }
}
