using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon
{
    public class StringPool
    {
        private static StringBuilder CommonStringBuilder = new StringBuilder();
        private static StringBuilder InternalStringBuilder = new StringBuilder();

        public static StringBuilder GetStringBuilder()
        {
            CommonStringBuilder.Remove(0, CommonStringBuilder.Length);
            return CommonStringBuilder;
        }

        public static string Concat(string s1, string s2)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.Append(s1);
            InternalStringBuilder.Append(s2);
            return InternalStringBuilder.ToString();
        }

        public static string Concat(string s1, string s2, string s3)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.Append(s1);
            InternalStringBuilder.Append(s2);
            InternalStringBuilder.Append(s3);
            return InternalStringBuilder.ToString();
        }

        public static string Format(string src, params object[] args)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.AppendFormat(src, args);
            return InternalStringBuilder.ToString();
        } 

    }
}
