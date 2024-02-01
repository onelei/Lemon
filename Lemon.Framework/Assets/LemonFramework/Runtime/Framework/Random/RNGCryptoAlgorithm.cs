using System;
using System.Security.Cryptography;

namespace Lemon.Framework.Random
{
    /// <summary>
    /// Random Number Generator
    /// </summary>
    public class RNGCryptoAlgorithm : IDisposable
    {
        private readonly RNGCryptoServiceProvider _provider;
        private readonly int _keySize;

        /// <summary>
        /// 32 equals 256 bit
        /// </summary>
        public RNGCryptoAlgorithm() : this(32)
        {
        }

        public RNGCryptoAlgorithm(int size)
        {
            _keySize = size;
            _provider = new RNGCryptoServiceProvider();
        }

        private byte[] GetBytes(int keySize)
        {
            byte[] randomKey = new byte[keySize];
            _provider.GetBytes(randomKey);
            return randomKey;
        }

        public string GetString(int keySize)
        {
            byte[] key = GetBytes(keySize);
            var keyString = BitConverter.ToString(key).Replace("-", "");
            return keyString;
        }

        public string GetString()
        {
            return GetString(_keySize);
        }

        public void Dispose()
        {
            _provider?.Dispose();
        }
    }
}