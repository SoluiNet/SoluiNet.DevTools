using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.CryptoTools.Utilities
{
    public interface ICryptoUtil
    {
        string Encrypt(string plainText, IDictionary<string, object> options);

        string EncryptAndBase64Encode(string plainText, IDictionary<string, object> options);

        string Decrypt(string encryptedText, IDictionary<string, object> options);

        string Base64DecodeAndDecrypt(string encryptedText, IDictionary<string, object> options);
    }
}
