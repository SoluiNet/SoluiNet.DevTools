// <copyright file="RsaUtils.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Encodings;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.OpenSsl;

    /// <summary>
    /// Provides a collection of methods to work with RSA encryption.
    /// </summary>
    public class RsaUtils : ICryptoUtil
    {
        /// <summary>
        /// Encrypt a plain text with RSA.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="publicKeyPath">The public key path.</param>
        /// <returns>Returns an encrypted text.</returns>
        public static string Encrypt(string plainText, string publicKeyPath)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = File.OpenText(publicKeyPath))
            {
                var publicKey = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, publicKey);
            }

            var encrypted = GeneralUtils.Encode(bytesToEncrypt, x => encryptEngine.ProcessBlock(x, 0, x.Length), "Unicode");

            return encrypted;
        }

        /// <summary>
        /// Encrypt a plain text with RSA and base64 encode it.
        /// </summary>
        /// <param name="clearText">The plain text.</param>
        /// <param name="publicKeyPath">The public key path.</param>
        /// <returns>Returns an encrypted and base64 encoded text.</returns>
        public static string EncryptAndBase64Encode(string clearText, string publicKeyPath)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = File.OpenText(publicKeyPath))
            {
                var publicKey = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, publicKey);
            }

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));

            return encrypted;
        }

        /// <summary>
        /// Decrypt an encrypted text with RSA.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="privateKeyPath">The private key path.</param>
        /// <param name="chosenEncoding">The chosen encoding.</param>
        /// <returns>Returns a decrypted text.</returns>
        public static string Decrypt(string encryptedText, string privateKeyPath, string chosenEncoding = "UTF8")
        {
            var bytesToDecrypt = Encoding.Unicode.GetBytes(encryptedText);

            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = System.IO.File.OpenText(privateKeyPath))
            {
                var privateKey = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, privateKey.Private);
            }

            var decrypted = GeneralUtils.Encode(bytesToDecrypt, x => decryptEngine.ProcessBlock(x, 0, x.Length), chosenEncoding);

            return decrypted;
        }

        /// <summary>
        /// Decrypt a base64 encoded and encrypted text with RSA.
        /// </summary>
        /// <param name="base64Input">The encrypted text.</param>
        /// <param name="privateKeyPath">The private key path.</param>
        /// <param name="chosenEncoding">The chosen encoding.</param>
        /// <returns>Returns a base64 decoded and decrypted text.</returns>
        public static string Base64DecodeAndDecrypt(string base64Input, string privateKeyPath, string chosenEncoding = "UTF8")
        {
            var bytesToDecrypt = Convert.FromBase64String(base64Input);

            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = System.IO.File.OpenText(privateKeyPath))
            {
                var privateKey = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, privateKey.Private);
            }

            var decrypted = GeneralUtils.Encode(bytesToDecrypt, x => decryptEngine.ProcessBlock(x, 0, x.Length), chosenEncoding);

            return decrypted;
        }

        /// <summary>
        /// Encrypt a plain text with RSA.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns an encrypted text.</returns>
        public string Encrypt(string plainText, IDictionary<string, object> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var publicKeyPath = options["PublicKeyPath"].ToString();

            if (publicKeyPath.StartsWith("\"", StringComparison.InvariantCulture))
            {
                publicKeyPath = publicKeyPath.Substring(1, publicKeyPath.Length - 1);
            }

            if (publicKeyPath.EndsWith("\"", StringComparison.InvariantCulture))
            {
                publicKeyPath = publicKeyPath.Substring(0, publicKeyPath.Length - 1);
            }

            return Encrypt(plainText, publicKeyPath);
        }

        /// <summary>
        /// Encrypt a plain text with RSA and base64 encode it.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns an encrypted and base64 encoded text.</returns>
        public string EncryptAndBase64Encode(string plainText, IDictionary<string, object> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var publicKeyPath = options["PublicKeyPath"].ToString();

            if (publicKeyPath.StartsWith("\"", StringComparison.InvariantCulture))
            {
                publicKeyPath = publicKeyPath.Substring(1, publicKeyPath.Length - 1);
            }

            if (publicKeyPath.EndsWith("\"", StringComparison.InvariantCulture))
            {
                publicKeyPath = publicKeyPath.Substring(0, publicKeyPath.Length - 1);
            }

            return EncryptAndBase64Encode(plainText, publicKeyPath);
        }

        /// <summary>
        /// Decrypt an encrypted text with RSA.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns a decrypted text.</returns>
        public string Decrypt(string encryptedText, IDictionary<string, object> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var privateKeyPath = options["PrivateKeyPath"].ToString();

            if (privateKeyPath.StartsWith("\"", StringComparison.InvariantCulture))
            {
                privateKeyPath = privateKeyPath.Substring(1, privateKeyPath.Length - 1);
            }

            if (privateKeyPath.EndsWith("\"", StringComparison.InvariantCulture))
            {
                privateKeyPath = privateKeyPath.Substring(0, privateKeyPath.Length - 1);
            }

            return Decrypt(encryptedText, privateKeyPath);
        }

        /// <summary>
        /// Decrypt a base64 encoded and encrypted text with RSA.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns a base64 decoded and decrypted text.</returns>
        public string Base64DecodeAndDecrypt(string encryptedText, IDictionary<string, object> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var privateKeyPath = options["PrivateKeyPath"].ToString();

            if (privateKeyPath.StartsWith("\"", StringComparison.InvariantCulture))
            {
                privateKeyPath = privateKeyPath.Substring(1, privateKeyPath.Length - 1);
            }

            if (privateKeyPath.EndsWith("\"", StringComparison.InvariantCulture))
            {
                privateKeyPath = privateKeyPath.Substring(0, privateKeyPath.Length - 1);
            }

            return Base64DecodeAndDecrypt(encryptedText, privateKeyPath);
        }
    }
}
