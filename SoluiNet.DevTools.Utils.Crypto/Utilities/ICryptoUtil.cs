// <copyright file="ICryptoUtil.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Crypto.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface to encrypt and decrypt strings.
    /// </summary>
    public interface ICryptoUtil
    {
        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="plainText">The plain text which should be encrypted.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns an encrypted string.</returns>
        string Encrypt(string plainText, IDictionary<string, object> options);

        /// <summary>
        /// Encrypt a string and base64 encode it.
        /// </summary>
        /// <param name="plainText">The plain text which should be encrypted.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns an encrypted and base64 encoded string.</returns>
        string EncryptAndBase64Encode(string plainText, IDictionary<string, object> options);

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="encryptedText">The encrypted text which should be decrypted.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns a decrypted string.</returns>
        string Decrypt(string encryptedText, IDictionary<string, object> options);

        /// <summary>
        /// Base64 decode and Decrypt a string.
        /// </summary>
        /// <param name="encryptedText">The encrypted text which should be decrypted.</param>
        /// <param name="options">The encryption options.</param>
        /// <returns>Returns a base64 decoded and decrypted string.</returns>
        string Base64DecodeAndDecrypt(string encryptedText, IDictionary<string, object> options);
    }
}
