using System;
using System.Collections.Generic;
using LemonFramework.Log;

namespace LemonFramework
{
    public enum ELanguageType
    {
        Chinese,
        English,
        ChineseTraditional,
    }

    public sealed class LanguageManager : Singleton<LanguageManager>
    {
        private Dictionary<string, string> LanguageDict = new Dictionary<string, string>();

        public void Initial()
        {
            LanguageDict.Clear();
            LanguageDict.Add("LemonText", "LemonText");
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                LogManager.LogError("传入的多语言Key为空！！");
                return string.Empty;
            }
            string value;
            if (LanguageDict.TryGetValue(key, out value))
            {
                return value;
            }
            LogManager.LogError(StringUtility.Concat("没有找到对应的多语言Key = ", key));
            return key;
        }
    }
}
