using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.Crypto.Utilities
{
    public class AesUtils: ICryptoUtil
    {
        public static string Encrypt(string plainText, string key, string initializationValue)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedArray;

            using (var mstream = new MemoryStream())
            {
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    using (var cryptoStream = new CryptoStream(mstream,
                        aesProvider.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationValue)), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }

                }
                encryptedArray = mstream.ToArray();
            }

            var encrypted = GeneralUtils.Encode(encryptedArray, x => x, "Unicode");

            return encrypted;
        }

        public static string EncryptAndBase64Encode(string clearText, string key, string initializationValue)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);
            byte[] encryptedArray;

            using (var mstream = new MemoryStream())
            {
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    using (var cryptoStream = new CryptoStream(mstream,
                        aesProvider.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(initializationValue)), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }

                }
                encryptedArray = mstream.ToArray();
            }

            var encrypted = Convert.ToBase64String(encryptedArray);
            return encrypted;
        }

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

        public string Encrypt(string plainText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return Encrypt(plainText, key, initializationValue);
        }

        public string EncryptAndBase64Encode(string plainText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return EncryptAndBase64Encode(plainText, key, initializationValue);
        }

        public string Decrypt(string encryptedText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return Decrypt(encryptedText, key, initializationValue);
        }

        public string Base64DecodeAndDecrypt(string encryptedText, IDictionary<string, object> options)
        {
            var key = options["key"].ToString();
            var initializationValue = options["iniValue"].ToString();

            return Base64DecodeAndDecrypt(encryptedText, key, initializationValue);
        }
    }
}
