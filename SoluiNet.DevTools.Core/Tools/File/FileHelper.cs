﻿// <copyright file="FileHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.File
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        /// Get all files which are contained in the overgiven directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="searchRecursively">Search recursively in sub folders.</param>
        /// <returns>A <see cref="List{T}"/> of all file paths which matches the filter.</returns>
        public static List<string> GetFilesInDirectory(string directoryPath, string filter = "*.*", bool searchRecursively = false)
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
    }
}
