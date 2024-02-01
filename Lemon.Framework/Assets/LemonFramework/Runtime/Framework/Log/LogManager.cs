﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lemon.Framework.Log
{
    public static class LogManager
    {
        private static readonly ILogger Logger;
        private static Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        static LogManager()
        {
            Logger = new DefaultLogger(null, new FileAppender());
        }

        public static ILogger GetLogger()
        {
            return GetLogger(null,null);
        }

        public static ILogger GetLogger(Type type)
        {
            return GetLogger(type?.Name);
        }
        
        public static ILogger GetLogger(string typeName)
        {
            return GetLogger(typeName, null);
        }

        public static ILogger GetLogger(string typeName, IAppender appender)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return Logger;
            }

            if (loggers.TryGetValue(typeName, out ILogger logger))
            {
                return logger;
            }

            logger = new DefaultLogger(typeName, appender);
            loggers.Add(typeName, logger);
            return logger;
        }

        public static void LogError(string msg)
        {
            GetLogger().LogError(msg);
        }

        public static void LogWarning(string msg)
        {
            GetLogger().LogWarning(msg);
        }

        public static void Log(string msg)
        {
            GetLogger().Log(msg);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogEditor(string msg)
        {
            Log(msg);
        }
    }
}