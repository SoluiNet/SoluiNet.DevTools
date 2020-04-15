// <copyright file="JsonUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
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

    /// <summary>
    /// Interaction logic for JsonUserControl.xaml.
    /// </summary>
    public partial class JsonUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonUserControl"/> class.
        /// </summary>
        public JsonUserControl()
        {
            this.InitializeComponent();
        }

        private void FormatJsonContent_Click(object sender, RoutedEventArgs e)
        {
            var originalContent = this.OriginalContent.Text;

            this.ProcessedContent.Text = JsonTools.Format(originalContent);
        }
    }
}
