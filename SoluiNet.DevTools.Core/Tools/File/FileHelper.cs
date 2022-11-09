// <copyright file="FileHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.File
{
    using System;
    using System.Collections.Generic;
#if COMPILED_FOR_NETCORE
    using System.Drawing;
#else
    using System.Drawing;
#endif
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides a collection of methods which help with working with files.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Load the content from the overgiven file path to a string.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file content.</returns>
        public static string StringFromFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return string.Empty;
            }

            // Open the file to read from.
            return System.IO.File.ReadAllText(path);
        }

        /// <summary>
        /// Load the content from the overgiven file path to a stream.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file content as <see cref="Stream"/>.</returns>
        public static Stream StreamFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (var fileContents = new FileStream(path, FileMode.Open))
            {
                var result = new MemoryStream();

                fileContents.CopyTo(result);

                fileContents.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[result.Length];
                result.Read(buffer, 0, buffer.Length);

                result.Position = 0;

                return result;
            }
        }

        /// <summary>
        /// Get all files which are contained in the overgiven directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="searchRecursively">Search recursively in sub folders.</param>
        /// <returns>A <see cref="ICollection{T}"/> of all file paths which matches the filter.</returns>
        public static ICollection<string> GetFilesInDirectory(string directoryPath, string filter = "*.*", bool searchRecursively = false)
        {
            var fileList = new List<string>();

            foreach (string file in Directory.EnumerateFiles(directoryPath, filter, searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                fileList.Add(file);
            }

            return fileList;
        }

        /// <summary>
        /// Convert an overgiven image file to a bitmap object (taken from: https://stackoverflow.com/questions/24383256/how-can-i-convert-a-jpg-file-into-a-bitmap-using-c).
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns a bitmap object for the overgiven image file path.</returns>
        public static Bitmap ConvertToBitmap(string filePath)
        {
            Bitmap bitmap;
            using (Stream bmpStream = File.Open(filePath, FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);
            }

            return bitmap;
        }

        /// <summary>
        /// Convert an overgiven image file to a bitmap object.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns a bitmap object for the overgiven image file path.</returns>
        public static Bitmap GetBitmapFromPath(this string filePath)
        {
            return FileHelper.ConvertToBitmap(filePath);
        }

        /// <summary>
        /// Get a temporary file path.
        /// </summary>
        /// <returns>Returns a temporary file path.</returns>
        public static string GetTemporaryFilePath()
        {
            var temporaryFileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}.tmp",
                StringHelper.GetRandomString(8),
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}\\SoluiNet.DevTools\\temp\\{1}",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                temporaryFileName);
        }
    }
}
