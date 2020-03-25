// <copyright file="CertificateUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Certificate
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
    /// Interaction logic for CertificateUserControl.xaml.
    /// </summary>
    public partial class CertificateUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateUserControl"/> class.
        /// </summary>
        public CertificateUserControl()
        {
            this.InitializeComponent();
        }

        private void GetInformation_Click(object sender, RoutedEventArgs e)
        {
            var url = this.CertificateUrl.Text;

            var certificate = CertificateTools.GetCertificateByUrl(new Uri(url));

            this.InformationBox.Text += string.Format(
                CultureInfo.InvariantCulture,
                "--- Information from '{4}' ---\r\nCN: {0}\r\nSubject: {1}\r\nCE Date: {2}\r\nPublicKey: {3}\r\n\r\n",
                certificate.Issuer,
                certificate.Subject,
                certificate.GetExpirationDateString(),
                certificate.GetPublicKeyString(),
                url);
        }
    }
}
