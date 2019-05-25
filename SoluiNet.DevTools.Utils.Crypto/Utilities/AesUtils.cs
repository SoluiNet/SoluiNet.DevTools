// <copyright file="AesUtils.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods to work with AES encryption.
    /// </summary>
    public class AesUtils : ICryptoUtil
    {
        /// <summary>
        /// Encrypt a plain text with AES.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initializationValue">The initialization value.</param>
        /// <returns>Returns an encrypted text.</returns>
        public static string Encrypt(string plainText, string key, string initializationValue)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedArray;

            using (var mstream = new MemoryStream())
            {
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    using (var cryptoStream = new CryptoStream(
                        mstream,
                        aesProvider.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationValue)),
                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }
                }

                encryptedArray = mstream.ToArray();
            }

            var encrypted = GeneralUtils.Encode(encryptedArray, x => x, "Unicode");

            return encrypted;
        }

        /// <summary>
        /// Encrypt a plain text with AES and base64 encode it.
        /// </summary>
        /// <param name="clearText">The plain text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initializationValue">The initialization value.</param>
        /// <returns>Returns an encrypted and base64 encoded text.</returns>
        public static string EncryptAndBase64Encode(string clearText, string key, string initializationValue)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);
            byte[] encryptedArray;

            using (var mstream = new MemoryStream())
            {
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    using (var cryptoStream = new CryptoStream(
                        mstream,
                        aesProvider.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationValue)),
                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }
                }

                encryptedArray = mstream.ToArray();
            }

            var encrypted = Convert.ToBase64String(encryptedArray);
            return encrypted;
        }

        /// <summary>
        /// Decrypt a base64 encoded and encrypted text with AES.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initializationValue">The initialization value.</param>
        /// <returns>Returns a base64 decoded and decrypted text.</returns>
        public static string Base64DecodeAndDecrypt(string encryptedText, string key, string initializationValue)
        {
            using (MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, new AesCryptoServiceProvider().CreateDecryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationValue)), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt an encrypted text with AES.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="key">The key.</param>
        /// <param name="initializationValue">The initialization value.</param>
        /// <param name="chosenEncoding">The chosen encoding for the plain text.</param>
        /// <returns>Returns a decrypted text.</returns>
        public static string Decrypt(string encryptedText, string key, string initializationValue, string chosenEncoding = "UTF8")
        {
            using (MemoryStream ms = new MemoryStream(Encoding.GetEncoding(chosenEncoding).GetBytes(encryptedText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, new AesCryptoServiceProvider().CreateDecryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationValue)), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Encrypt a plain text with AES.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns an encrypted text.</returns>
        public string Encrypt(string plainText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return Encrypt(plainText, key, initializationValue);
        }

        /// <summary>
        /// Encrypt a plain text with AES and base64 encode it.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns an encrypted and base64 encoded text.</returns>
        public string EncryptAndBase64Encode(string plainText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return EncryptAndBase64Encode(plainText, key, initializationValue);
        }

        /// <summary>
        /// Decrypt an encrypted text with AES.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns a decrypted text.</returns>
        public string Decrypt(string encryptedText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return Decrypt(encryptedText, key, initializationValue);
        }

        /// <summary>
        /// Decrypt a base64 encoded and encrypted text with AES.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns a base64 decoded and decrypted text.</returns>
        public string Base64DecodeAndDecrypt(string encryptedText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return Base64DecodeAndDecrypt(encryptedText, key, initializationValue);
        }
    }
}
