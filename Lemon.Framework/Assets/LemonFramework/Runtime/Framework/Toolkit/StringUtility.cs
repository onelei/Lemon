/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System.Text;
using Lemon.Framework.Log;

namespace Lemon.Framework
{
    public static class StringUtility
    {
        private readonly static int MaxCount = 100;
        private static volatile object locker = new object();
        // Object pool to avoid allocations.
        private static readonly ObjectPool<StringBuilder> Pool = new ObjectPool<StringBuilder>(Clear, null);
        private static readonly StringBuilder SharedStringBuilder = new StringBuilder(1024);

        static void Clear(StringBuilder stringBuilder)
        {
            if (Pool.countAll >= MaxCount)
            {
                LogManager.LogError("Pool count reach to MaxCount.");
            }

            stringBuilder.Clear();
        }

        public static StringBuilder GetSharedStringBuilder()
        {
            SharedStringBuilder.Clear();
            return SharedStringBuilder;
        }

        public static StringBuilder GetStringBuilder()
        {
            StringBuilder stringBuilder = Pool.Get();
            return stringBuilder;
        }

        public static void Release(StringBuilder toRelease)
        {
            if (Pool.countAll >= MaxCount)
            {
                LogManager.LogError("Pool count reach to MaxCount.");
            }

            Pool.Release(toRelease);
        }

        public static string Concat(string s1, string s2)
        {
            lock (locker)
            {
                StringBuilder stringBuilder = Pool.Get();
                stringBuilder.Append(s1);
                stringBuilder.Append(s2);
                string result = stringBuilder.ToString();
                Release(stringBuilder);
                return result;
            }
        }

        public static string Concat(string s1, string s2, string s3)
        {
            lock (locker)
            {
                StringBuilder stringBuilder = Pool.Get();
                stringBuilder.Append(s1);
                stringBuilder.Append(s2);
                stringBuilder.Append(s3);
                string result = stringBuilder.ToString();
                Release(stringBuilder);
                return result;
            }
        }

        public static string Format(string src, params object[] args)
        {
            lock (locker)
            {
                StringBuilder stringBuilder = Pool.Get();
                stringBuilder.Remove(0, stringBuilder.Length);
                stringBuilder.AppendFormat(src, args);
                string result = stringBuilder.ToString();
                Release(stringBuilder);
                return result;
            }
        }
    }
}