using System;

namespace LemonFramework.Log
{
    public class DefaultLogger : ILogger
    {
        public string Name { get; }
        protected IAppender Appender { get; }

        public DefaultLogger() : this(null, null)
        {
        }

        public DefaultLogger(string name) : this(name, null)
        {
        }

        public DefaultLogger(IAppender appender) : this(null, appender)
        {
        }

        public DefaultLogger(string name, IAppender appender)
        {
            Name = name;
            if (string.IsNullOrEmpty(Name))
            {
                Name = GetType().Name;
            }

            Appender = appender;
            if (Appender == null)
            {
                Appender = new FileAppender();
            }
        }

        public string GetCurrentTimestamp()
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyyMMddHHmmss");
            return timestamp;
        }

        public string GetFormatString(string msg)
        {
            return $"[{GetCurrentTimestamp()}] [{Name}] :{msg}";
        }

        public void Log(string msg)
        {
            UnityEngine.Debug.Log(GetFormatString(msg));
            Appender?.Append(msg);
        }

        public void LogWarning(string msg)
        {
            UnityEngine.Debug.LogWarning(GetFormatString(msg));
            Appender?.Append(msg);
        }

        public void LogError(string msg)
        {
            UnityEngine.Debug.LogError(GetFormatString(msg));
            Appender?.Append(msg);
        }
    }
}