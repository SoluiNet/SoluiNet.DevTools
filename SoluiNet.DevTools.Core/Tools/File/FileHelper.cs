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

        /// <summary>
        /// Get all files which are contained in the overgiven directory
        /// </summary>
        /// <param name="directoryPath">The directory path</param>
        /// <param name="filter">The filter</param>
        /// <param name="searchRecursively">Search recursively in sub folders</param>
        /// <returns>A <see cref="List{string}"/> of all file paths which matches the filter</returns>
        public static List<string> GetFilesInDirectory(string directoryPath, string filter = "*.*", bool searchRecursively = false)
        {
            var fileList = new List<string>();

            /*foreach (string directory in Directory.GetDirectories(directoryPath))
            {
                foreach (string file in Directory.EnumerateFiles(directory, filter, SearchOption.TopDirectoryOnly))
                {
                    fileList.Add(file);
                }

                if (searchRecursively)
                    fileList.AddRange(GetFilesInDirectory(directoryPath, filter, searchRecursively));
            }*/

            foreach(string file in Directory.EnumerateFiles(directoryPath, filter, searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                fileList.Add(file);
            }

            return fileList;
        }
    }
}
