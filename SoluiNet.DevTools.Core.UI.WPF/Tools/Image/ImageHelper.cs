// <copyright file="ImageHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Tools.Image
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Provides a collection of methods which support working with images.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Converts a bitmap object to a bitmap image.
        /// </summary>
        /// <param name="bitmap">The bitmap object.</param>
        /// <returns>Returns a bitmap image object.</returns>
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        /// <summary>
        /// Converts a bitmap object to a bitmap image.
        /// </summary>
        /// <param name="bitmap">The bitmap object.</param>
        /// <returns>Returns a bitmap image object.</returns>
        public static BitmapImage ConvertToBitmapImage(this Bitmap bitmap)
        {
            return ImageHelper.BitmapToImageSource(bitmap);
        }
    }
}
