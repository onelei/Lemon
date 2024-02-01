using Lemon.Framework.Random;
using UnityEngine;

namespace Lemon.Framework.Encrypt
{
    public static class EncryptUtility
    {
        private const string DefaultKey = "7D2FD84CFAB55C49DE95AE67637500AAD274272890AB04835C3BE44CFB0B15A2";

        public static string Encrypt(string data, string key = DefaultKey)
        {
            return XXTEA.EncryptToBase64String(data, key);
        }

        public static string Decrypt(string data, string key = DefaultKey)
        {
            return XXTEA.DecryptBase64StringToString(data, key);
        }

        public static byte[] Encrypt(byte[] data, string key = DefaultKey)
        {
            return XXTEA.Encrypt(data, key);
        }

        public static byte[] Decrypt(byte[] data, string key = DefaultKey)
        {
            return XXTEA.Decrypt(data, key);
        }
   
#if UNITY_EDITOR
        //[UnityEditor.MenuItem("Lemon.Framework/Encrypt/UnitTest")]
        public static void UnitTest()
        {
            var str = "Hello World! 你好，中国！";
            var key = "1234567890";
            var encrypt_data = XXTEA.EncryptToBase64String(str, key);
            Debug.Log(encrypt_data);
            Debug.Assert("QncB1C0rHQoZ1eRiPM4dsZtRi9pNrp7sqvX76cFXvrrIHXL6" == encrypt_data);
            var decrypt_data = XXTEA.DecryptBase64StringToString(encrypt_data, key);
            Debug.Assert(str == decrypt_data);

            using (var provider = new RNGCryptoAlgorithm())
            {
                Debug.Log($"Generate Key: {provider.GetString()}");
            }
        }
#endif
    }
}