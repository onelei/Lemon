/**
*   Author：onelei
*   https://github.com/onelei/Lemon.Framework
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public static class ThreadString
{
    private static volatile object locker = new object();
     
    private static readonly StringBuilder InternalStringBuilder = new StringBuilder();
    private static readonly StringBuilder ExternalStringBuilder = new StringBuilder();
 

    public static StringBuilder Get()
    {
        lock (locker)
        {
            ExternalStringBuilder.Remove(0, ExternalStringBuilder.Length);
            return ExternalStringBuilder;
        }
    }
     
    public static string Concat(string s1, string s2)
    {
        lock (locker)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.Append(s1);
            InternalStringBuilder.Append(s2);
            return InternalStringBuilder.ToString(); 
        }
    }

    public static string Concat(string s1, string s2, string s3)
    {
        lock (locker)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.Append(s1);
            InternalStringBuilder.Append(s2);
            InternalStringBuilder.Append(s3);
            return InternalStringBuilder.ToString();
        }
    }

    public static string Format(string src, params object[] args)
    {
        lock (locker)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.AppendFormat(src, args);
            return InternalStringBuilder.ToString();
        }
    }
}

