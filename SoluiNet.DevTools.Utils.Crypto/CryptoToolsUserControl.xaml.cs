// <copyright file="CryptoToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto
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

    /// <summary>
    /// Interaction logic for CryptoToolsUserControl.xaml.
    /// </summary>
    public partial class CryptoToolsUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoToolsUserControl"/> class.
        /// </summary>
        public CryptoToolsUserControl()
        {
            this.InitializeComponent();
        }

        private string GetEncryptionMethod()
        {
            switch (this.EncryptionMethodTabs.SelectedIndex)
            {
                // RSA
                case 0:
                    return "RSA";

                // AES
                case 1:
                    return "AES";

                // Base64
                case 2:
                    return "Hashing";

                // Base64
                case 3:
                    return "Base64";
            }

            return string.Empty;
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            var options = new Dictionary<string, object>()
            {
                { "Method", this.GetEncryptionMethod() },
                { "DecodeFromBase64", this.Base64Encoded.IsChecked ?? false },
                { "EncodeToBase64", this.EncodeWithBase64.IsChecked ?? false },
            };

            if (this.GetEncryptionMethod() == "RSA")
            {
                options.Add("PublicKeyPath", this.PublicKeyPath.Text);
            }
            else if (this.GetEncryptionMethod() == "AES")
            {
                options.Add("key", this.AesKey.Text);
                options.Add("iniValue", this.AesIniValue.Text);
            }
            else if (this.GetEncryptionMethod() == "Hashing")
            {
                options.Add("hashMethod", this.HashMethod.Text);
            }

            this.EncryptedText.Text = CryptoTools.Encrypt(this.DecryptedText.Text, options);
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            var options = new Dictionary<string, object>()
            {
                { "Method", this.GetEncryptionMethod() },
                { "DecodeFromBase64", this.Base64Encoded.IsChecked ?? false },
                { "EncodeToBase64", this.EncodeWithBase64.IsChecked ?? false },
            };

            if (this.GetEncryptionMethod() == "RSA")
            {
                options.Add("PrivateKeyPath", this.PrivateKeyPath.Text);
            }
            else if (this.GetEncryptionMethod() == "AES")
            {
                options.Add("key", this.AesKey.Text);
                options.Add("iniValue", this.AesIniValue.Text);
            }

            this.DecryptedText.Text = CryptoTools.Decrypt(this.EncryptedText.Text, options);
        }

        private void EncryptionMethodTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                this.Decrypt.IsEnabled = !this.HashTab.IsSelected;
            }
        }
    }
}
