// <copyright file="ImageHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Image
{
    using System;
#if COMPILED_FOR_NETCORE
    using System.Drawing;
#else
    using System.Drawing;
#endif
    using SoluiNet.DevTools.Core.Tools.Resources;

    /// <summary>
    /// Provides a collection of methods which help with working with images.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Crop an image (taken from: https://stackoverflow.com/questions/9484935/how-to-cut-a-part-of-image-in-c-sharp).
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="area">The section which should be cropped.</param>
        /// <returns>Returns the cropped section from the overgiven image.</returns>
        public static Bitmap CropImage(this Bitmap image, Rectangle area)
        {
            // An empty bitmap which will hold the cropped image
            var bitmap = new Bitmap(area.Width, area.Height);

            var g = Graphics.FromImage(bitmap);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(image, 0, 0, area, GraphicsUnit.Pixel);

            return bitmap;
        }

        /// <summary>
        /// Draw a rectangle in an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="area">The rectangle which should be drawn.</param>
        /// <param name="colour">The colour the rectangle should have.</param>
        /// <returns>Returns an image with a drawn rectangle.</returns>
        public static Bitmap DrawRectangle(this Bitmap image, Rectangle area, Color colour)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var bitmap = image.Clone() as Bitmap;

            // Create pen.
            var pen = new Pen(colour, 3);

            try
            {
                var g = Graphics.FromImage(bitmap ?? throw new InvalidOperationException(typeof(ImageHelper).GetResourceString("NullBitmap")));

                // Draw rectangle to image.
                g.DrawRectangle(pen, area);

                return bitmap;
            }
            finally
            {
                pen.Dispose();
            }
        }
    }
}
