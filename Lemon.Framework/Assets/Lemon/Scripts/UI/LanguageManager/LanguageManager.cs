using System;
using System.Collections.Generic;
 

namespace Lemon.Framework
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
            LanguageDict.Add("QText", "QTEXT");
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                QLog.LogError("传入的多语言Key为空！！");
                return string.Empty;
            }
            string value;
            if (LanguageDict.TryGetValue(key, out value))
            {
                return value;
            }
            QLog.LogError(StringUtility.Concat("没有找到对应的多语言Key = ", key));
            return key;
        }
    }
}
