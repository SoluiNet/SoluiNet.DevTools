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

    public class RsaUtils : ICryptoUtil
    {
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

        public string Encrypt(string plainText, IDictionary<string, object> options)
        {
            var publicKeyPath = options["PublicKeyPath"].ToString();

            if (publicKeyPath.StartsWith("\""))
            {
                publicKeyPath = publicKeyPath.Substring(1, publicKeyPath.Length - 1);
            }

            if (publicKeyPath.EndsWith("\""))
            {
                publicKeyPath = publicKeyPath.Substring(0, publicKeyPath.Length - 1);
            }

            return Encrypt(plainText, publicKeyPath);
        }

        public string EncryptAndBase64Encode(string plainText, IDictionary<string, object> options)
        {
            var publicKeyPath = options["PublicKeyPath"].ToString();

            if (publicKeyPath.StartsWith("\""))
            {
                publicKeyPath = publicKeyPath.Substring(1, publicKeyPath.Length - 1);
            }

            if (publicKeyPath.EndsWith("\""))
            {
                publicKeyPath = publicKeyPath.Substring(0, publicKeyPath.Length - 1);
            }

            return EncryptAndBase64Encode(plainText, publicKeyPath);
        }

        public string Decrypt(string encryptedText, IDictionary<string, object> options)
        {
            var privateKeyPath = options["PrivateKeyPath"].ToString();

            if (privateKeyPath.StartsWith("\""))
            {
                privateKeyPath = privateKeyPath.Substring(1, privateKeyPath.Length - 1);
            }

            if (privateKeyPath.EndsWith("\""))
            {
                privateKeyPath = privateKeyPath.Substring(0, privateKeyPath.Length - 1);
            }

            return Decrypt(encryptedText, privateKeyPath);
        }

        public string Base64DecodeAndDecrypt(string encryptedText, IDictionary<string, object> options)
        {
            var privateKeyPath = options["PrivateKeyPath"].ToString();

            if (privateKeyPath.StartsWith("\""))
            {
                privateKeyPath = privateKeyPath.Substring(1, privateKeyPath.Length - 1);
            }

            if (privateKeyPath.EndsWith("\""))
            {
                privateKeyPath = privateKeyPath.Substring(0, privateKeyPath.Length - 1);
            }

            return Base64DecodeAndDecrypt(encryptedText, privateKeyPath);
        }
    }
}
