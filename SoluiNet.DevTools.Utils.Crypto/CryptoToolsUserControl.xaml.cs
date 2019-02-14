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

namespace SoluiNet.DevTools.Utils.Crypto
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class CryptToolsUserControl : UserControl
    {
        public CryptToolsUserControl()
        {
            InitializeComponent();
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
                    return "Base64";
            }

            return string.Empty;
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            var options = new Dictionary<string, object>()
            {
                {"Method", GetEncryptionMethod()},
                {"DecodeFromBase64", Base64Encoded.IsChecked ?? false},
                {"EncodeToBase64", EncodeWithBase64.IsChecked ?? false}
            };

            if (GetEncryptionMethod() == "RSA")
            {
                options.Add("PublicKeyPath", PublicKeyPath.Text);
            }
            else if (GetEncryptionMethod() == "AES")
            {
                options.Add("key", AesKey.Text);
                options.Add("iniValue", AesIniValue.Text);
            }

            EncryptedText.Text = CryptoTools.Encrypt(DecryptedText.Text, options);
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            var options = new Dictionary<string, object>()
            {
                {"Method", GetEncryptionMethod()},
                {"DecodeFromBase64", Base64Encoded.IsChecked ?? false},
                {"EncodeToBase64", EncodeWithBase64.IsChecked ?? false}
            };

            if (GetEncryptionMethod() == "RSA")
            {
                options.Add("PrivateKeyPath", PrivateKeyPath.Text);
            }
            else if (GetEncryptionMethod() == "AES")
            {
                options.Add("key", AesKey.Text);
                options.Add("iniValue", AesIniValue.Text);
            }

            DecryptedText.Text = CryptoTools.Decrypt(EncryptedText.Text, options);
        }
    }
}
