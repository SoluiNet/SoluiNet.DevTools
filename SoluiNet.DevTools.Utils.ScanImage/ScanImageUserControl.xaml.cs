// <copyright file="ScanImageUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.ScanImage
{
    using System;
    using System.Collections.Generic;
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
    using Microsoft.Win32;
    using SoluiNet.DevTools.Core.Tools.File;
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

            var barcodeResult = barcodeReader.Decode(this.ImageFilePath.Text.GetBitmapFromPath());

            this.ImageThumbnail.Source = this.ImageFilePath.Text.GetBitmapFromPath().ConvertToBitmapImage();

            if (barcodeResult != null)
            {
                this.ScanResult.Text = string.Format("Type: {0}\r\nText: {1}", barcodeResult.BarcodeFormat.ToString(), barcodeResult.Text);
            }
            else
            {
                this.ScanResult.Text = "No Barcode found";
            }
        }

        private void OcrImage_Click(object sender, RoutedEventArgs e)
        {
            this.ScanResult.Text = string.Empty;

            this.ImageThumbnail.Source = this.ImageFilePath.Text.GetBitmapFromPath().ConvertToBitmapImage();

            using (var ocrEngine = new TesseractEngine(string.Format("{0}\\tessdata", System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location)), "eng", EngineMode.Default))
            {
                // have to load Pix via a bitmap since Pix doesn't support loading a stream.
                using (var image = this.ImageFilePath.Text.GetBitmapFromPath())
                {
                    using (var pix = PixConverter.ToPix(image))
                    {
                        using (var page = ocrEngine.Process(pix))
                        {
                            // Console.WriteLine(page.GetMeanConfidence() + " : " + page.GetText());
                            this.ScanResult.Text += string.Format("Confidence {0} - #\"{1}\"#", page.GetMeanConfidence(), page.GetText());
                        }
                    }
                }
            }
        }
    }
}
