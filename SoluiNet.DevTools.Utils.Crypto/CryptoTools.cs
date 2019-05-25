// <copyright file="CryptoTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Utils.Crypto.Utilities;

    /// <summary>
    /// Provides a collection of methods for cryptography and hashing.
    /// </summary>
    public class CryptoTools
    {
        /// <summary>
        /// Encrypt a plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns the encrypted text.</returns>
        public static string Encrypt(string plainText, IDictionary<string, object> options)
        {
            var toEncrypt = plainText;
            var encryptedText = string.Empty;

            var decodeFromBase64 = options.ContainsKey("DecodeFromBase64") &&
                                   Convert.ToBoolean(options["DecodeFromBase64"]);
            var encodeToBase64 = options.ContainsKey("EncodeToBase64") && Convert.ToBoolean(options["EncodeToBase64"]);

            var method = options.ContainsKey("Method") ? options["Method"].ToString() : string.Empty;

            switch (method)
            {
                // RSA
                case "RSA":
                    var publicKeyPath = options["PublicKeyPath"].ToString();

                    if (publicKeyPath.StartsWith("\""))
                    {
                        publicKeyPath = publicKeyPath.Substring(1, publicKeyPath.Length - 1);
                    }

                    if (publicKeyPath.EndsWith("\""))
                    {
                        publicKeyPath = publicKeyPath.Substring(0, publicKeyPath.Length - 1);
                    }

                    if (decodeFromBase64)
                    {
                        toEncrypt = GeneralUtils.Base64Decode(toEncrypt);
                    }

                    encryptedText = encodeToBase64 ? RsaUtils.EncryptAndBase64Encode(toEncrypt, publicKeyPath) : RsaUtils.Encrypt(toEncrypt, publicKeyPath);
                    break;

                // AES
                case "AES":
                    var key = options["key"].ToString();
                    var initializationValue = options["iniValue"].ToString();

                    encryptedText = encodeToBase64 ? AesUtils.EncryptAndBase64Encode(toEncrypt, key, initializationValue) : AesUtils.Encrypt(toEncrypt, key, initializationValue);
                    break;

                // Base64
                case "Base64":
                    encryptedText = GeneralUtils.Base64Encode(toEncrypt);
                    break;

                // Hashing
                case "Hashing":
                    switch (options["hashMethod"].ToString())
                    {
                        case "MD5":
                            // create new instance of MD5
                            var md5 = MD5.Create();

                            // convert the input text to array of bytes
                            var hashDataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(toEncrypt));

                            // return hexadecimal string
                            encryptedText = BitConverter.ToString(hashDataMd5).Replace("-", string.Empty);
                            break;
                        case "SHA1":
                            // create new instance of SHA1
                            var sha1 = SHA1.Create();

                            // convert the input text to array of bytes
                            var hashDataSha1 = sha1.ComputeHash(Encoding.Default.GetBytes(toEncrypt));

                            // return hexadecimal string
                            encryptedText = BitConverter.ToString(hashDataSha1).Replace("-", string.Empty);
                            break;
                        case "SHA2":
                        case "SHA256":
                            // create new instance of SHA256
                            var sha2 = SHA256.Create();

                            // convert the input text to array of bytes
                            var hashDataSha2 = sha2.ComputeHash(Encoding.Default.GetBytes(toEncrypt));

                            // return hexadecimal string
                            encryptedText = BitConverter.ToString(hashDataSha2).Replace("-", string.Empty);
                            break;
                    }

                    break;
            }

            return encryptedText;
        }

        /// <summary>
        /// Decrypt an encrypted text.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="options">The decryption options.</param>
        /// <returns>Returns a decrypted text.</returns>
        public static string Decrypt(string encryptedText, IDictionary<string, object> options)
        {
            var toDecrypt = encryptedText;
            var decryptedText = string.Empty;

            var decodeFromBase64 = options.ContainsKey("DecodeFromBase64") &&
                                   Convert.ToBoolean(options["DecodeFromBase64"]);
            var encodeToBase64 = options.ContainsKey("EncodeToBase64") && Convert.ToBoolean(options["EncodeToBase64"]);

            var method = options.ContainsKey("Method") ? options["Method"].ToString() : string.Empty;

            switch (method)
            {
                // RSA
                case "RSA":
                    var privateKeyPath = options["PrivateKeyPath"].ToString();

                    if (privateKeyPath.StartsWith("\""))
                    {
                        privateKeyPath = privateKeyPath.Substring(1, privateKeyPath.Length - 1);
                    }

                    if (privateKeyPath.EndsWith("\""))
                    {
                        privateKeyPath = privateKeyPath.Substring(0, privateKeyPath.Length - 1);
                    }

                    decryptedText = decodeFromBase64 ? RsaUtils.Base64DecodeAndDecrypt(toDecrypt, privateKeyPath) : RsaUtils.Decrypt(toDecrypt, privateKeyPath);
                    break;

                // AES
                case "AES":
                    var key = options["key"].ToString();
                    var initializationValue = options["iniValue"].ToString();

                    decryptedText = decodeFromBase64 ? AesUtils.Base64DecodeAndDecrypt(toDecrypt, key, initializationValue) : AesUtils.Decrypt(toDecrypt, key, initializationValue);
                    break;

                // Base64
                case "Base64":
                    decryptedText = GeneralUtils.Base64Decode(toDecrypt);
                    break;
            }

            return encodeToBase64 ? GeneralUtils.Base64Encode(decryptedText) : decryptedText;
        }
    }
}
