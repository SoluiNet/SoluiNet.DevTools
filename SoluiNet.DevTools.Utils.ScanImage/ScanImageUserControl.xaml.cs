// <copyright file="ScanImageUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.ScanImage
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using ImageMagick;
    using Microsoft.Win32;
    using SoluiNet.DevTools.Core.Tools.File;
    using SoluiNet.DevTools.Core.Tools.Image;
    using SoluiNet.DevTools.Core.UI.WPF.Tools.Image;
    using Tesseract;
    using ZXing;

    /// <summary>
    /// Interaction logic for ScanImageUserControl.xaml.
    /// </summary>
    public partial class ScanImageUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanImageUserControl"/> class.
        /// </summary>
        public ScanImageUserControl()
        {
            this.InitializeComponent();
        }

        private bool IsCropped { get; set; }

        private void SearchFile_Click(object sender, RoutedEventArgs e)
        {
            var loadFileDialog = new OpenFileDialog()
            {
                Filter = "All Graphics Types|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff|"
                    + "Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPG (*.jpg,*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|TIFF (*.tif, *.tiff)|*.tif;*.tiff",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            };

            if (loadFileDialog.ShowDialog() == true)
            {
                this.ImageFilePath.Text = loadFileDialog.FileName;
            }
        }

        private void IdentifyBarcode_Click(object sender, RoutedEventArgs e)
        {
            var barcodeReader = new BarcodeReader();

            var barcodeResult = barcodeReader.Decode(this.GetScannableImage());

            this.ImageThumbnail.Source = this.GetThumbnail();

            if (barcodeResult != null)
            {
                this.ScanResult.Text = string.Format(CultureInfo.InvariantCulture, "Type: {0}\r\nText: {1}", barcodeResult.BarcodeFormat.ToString(), barcodeResult.Text);
            }
            else
            {
                this.ScanResult.Text = "No Barcode found";
            }
        }

        private void OcrImage_Click(object sender, RoutedEventArgs e)
        {
            this.ScanResult.Text = string.Empty;

            this.ImageThumbnail.Source = this.GetThumbnail();

            using (var ocrEngine = new TesseractEngine(string.Format(CultureInfo.InvariantCulture, "{0}\\tessdata", System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location)), "eng", EngineMode.Default))
            {
                var scannableImage = this.GetScannableImage();

                var image = new MagickImage(scannableImage.ConvertToByteArray());
                try
                {
                    /* TextCleanerScript cleaner = new TextCleanerScript(); */

                    // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                    using (var pix = PixConverter.ToPix(scannableImage))
                    {
                        using (var page = ocrEngine.Process(pix))
                        {
                            // Console.WriteLine(page.GetMeanConfidence() + " : " + page.GetText());
                            this.ScanResult.Text += string.Format(
                                CultureInfo.InvariantCulture,
                                "Confidence {0} - #\"{1}\"#",
                                page.GetMeanConfidence(),
                                page.GetText());
                        }
                    }
                }
                finally
                {
                    image?.Dispose();
                }
            }
        }

        private ImageSource GetThumbnail()
        {
            if (this.IsCropped)
            {
                return this.ImageThumbnail.Source = this.ImageFilePath.Text.GetBitmapFromPath().DrawRectangle(
                    new System.Drawing.Rectangle()
                    {
                        Height = Convert.ToInt32(this.RectangleHeight.Text, CultureInfo.InvariantCulture),
                        Width = Convert.ToInt32(this.RectangleWidth.Text, CultureInfo.InvariantCulture),
                        X = Convert.ToInt32(this.RectangleX.Text, CultureInfo.InvariantCulture),
                        Y = Convert.ToInt32(this.RectangleY.Text, CultureInfo.InvariantCulture),
                    },
                    System.Drawing.Color.Red)
                    .ConvertToBitmapImage();
            }
            else
            {
                return this.ImageFilePath.Text.GetBitmapFromPath().ConvertToBitmapImage();
            }
        }

        private Bitmap GetScannableImage()
        {
            if (this.IsCropped)
            {
                return this.ImageFilePath.Text.GetBitmapFromPath().CropImage(new System.Drawing.Rectangle()
                {
                    Height = Convert.ToInt32(this.RectangleHeight.Text, CultureInfo.InvariantCulture),
                    Width = Convert.ToInt32(this.RectangleWidth.Text, CultureInfo.InvariantCulture),
                    X = Convert.ToInt32(this.RectangleX.Text, CultureInfo.InvariantCulture),
                    Y = Convert.ToInt32(this.RectangleY.Text, CultureInfo.InvariantCulture),
                });
            }
            else
            {
                return this.ImageFilePath.Text.GetBitmapFromPath();
            }
        }

        private void CropImage_Click(object sender, RoutedEventArgs e)
        {
            this.IsCropped = true;

            this.ImageThumbnail.Source = this.GetThumbnail();

            this.ImagePreview.Width = new GridLength(250.0);

            this.ImagePreviewThumbnail.Source = this.GetScannableImage().ConvertToBitmapImage();
        }
    }
}
