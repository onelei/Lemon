using UnityEngine;

namespace Lemon.Framework.Encrypt
{
    public class EncryptHandler
    {
        public void Encrypt()
        {
        }

        public void Decrypt()
        {
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
        }
#endif
    }
}