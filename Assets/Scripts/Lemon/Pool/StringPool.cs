using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon
{
    public class StringPool
    {
        private static StringBuilder stringBuilder = new StringBuilder();

        public static string Concat(string s1, string s2)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append(s1);
            stringBuilder.Append(s2);
            return stringBuilder.ToString();
        }

        public static string Format(string src, params object[] args)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.AppendFormat(src, args);
            return stringBuilder.ToString();
        } 

    }
}
