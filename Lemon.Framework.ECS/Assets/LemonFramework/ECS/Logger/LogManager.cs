using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LemonFrameSync.Logger
{
    public class LogManager
    {
        private static LogManager _instance;
        private static readonly object Lock = new object();

        // 日志文件路径
        private string _logFilePath;

        // 日志文件流
        private StreamWriter _logWriter;

        // 日志队列，用于线程安全处理
        private readonly Queue<string> _logQueue = new Queue<string>();

        // 处理日志的线程
        private Thread _logProcessingThread;

        // 控制线程运行的标志
        private bool _isRunning = false;

        // 帧同步专用锁，确保日志与帧同步
        private object _frameLock = new object();

        /// <summary>
        /// 日志级别
        /// </summary>
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// 单例实例
        /// </summary>
        public static LogManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new LogManager();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        /// <param name="logDirectory">日志目录，默认为Application.persistentDataPath/FrameSyncLogs</param>
        public void Initialize(string logDirectory = null)
        {
            // 设置日志目录
            var dir = Application.persistentDataPath;
#if UNITY_EDITOR
            dir = $"{Application.dataPath}/../";
#endif
            string logDir = logDirectory ?? Path.Combine(dir, "FrameSyncLogs");

            // 创建目录如果不存在
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            // 创建日志文件（按时间命名）
            string fileName = $"FrameSyncLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            _logFilePath = Path.Combine(logDir, fileName);

            // 初始化日志写入器
            _logWriter = new StreamWriter(_logFilePath, true, Encoding.UTF8) {AutoFlush = true};

            // 记录初始化信息
            WriteLogInternal($"日志系统初始化完成，日志文件路径: {_logFilePath}", LogLevel.Info);

            // 启动日志处理线程
            _isRunning = true;
            _logProcessingThread = new Thread(ProcessLogs);
            _logProcessingThread.IsBackground = true;
            _logProcessingThread.Start();
        }

        /// <summary>
        /// 处理日志队列的线程函数
        /// </summary>
        private void ProcessLogs()
        {
            while (_isRunning)
            {
                try
                {
                    // 检查是否有日志需要处理
                    lock (_logQueue)
                    {
                        while (_logQueue.Count > 0)
                        {
                            string logMessage = _logQueue.Dequeue();
                            // 写入日志到文件
                            _logWriter?.WriteLine(logMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"日志处理出错: {ex.Message}");
                }

                // 短暂休眠，减少CPU占用
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 写入日志（线程安全）
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="level">日志级别</param>
        /// <param name="frame">当前帧号（可选）</param>
        public void Log(string message, LogLevel level = LogLevel.Info, int frame = -1)
        {
            if (string.IsNullOrEmpty(message)) return;

            // 生成格式化的日志消息
            string formattedMessage = FormatLogMessage(message, level, frame);

            // 将日志加入队列
            lock (_logQueue)
            {
                _logQueue.Enqueue(formattedMessage);
            }

            // 同时输出到Unity控制台，方便实时查看
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }

        /// <summary>
        /// 写入帧同步相关日志，会使用帧锁确保同步性
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="frame">当前帧号</param>
        public void LogFrame(string message, int frame)
        {
            if (_frameLock == null)
            {
                Log("LogManager尚未初始化，无法使用LogFrame方法", LogLevel.Error);
                return;
            }

            // 使用帧锁确保日志与帧操作同步
            lock (_frameLock)
            {
                Log(message, LogLevel.Info, frame);
            }
        }

        /// <summary>
        /// 格式化日志消息
        /// </summary>
        private string FormatLogMessage(string message, LogLevel level, int frame)
        {
            string frameStr = frame >= 0 ? $"[Frame: {frame}]" : "";
            return $"[{DateTime.Now:HH:mm:ss.fff}] {frameStr}[{level}] {message}";
        }

        /// <summary>
        /// 内部日志写入（用于系统自身信息）
        /// </summary>
        private void WriteLogInternal(string message, LogLevel level)
        {
            string formattedMessage = FormatLogMessage(message, level, -1);
            _logWriter?.WriteLine(formattedMessage);
        }

        /// <summary>
        /// 获取当前日志文件路径
        /// </summary>
        public string GetLogFilePath()
        {
            return _logFilePath;
        }

        /// <summary>
        /// 关闭日志系统，释放资源
        /// </summary>
        public void Shutdown()
        {
            _isRunning = false;

            // 等待日志处理线程结束
            if (_logProcessingThread != null && _logProcessingThread.IsAlive)
            {
                _logProcessingThread.Join(1000);
            }

            // 处理剩余的日志
            while (_logQueue.Count > 0)
            {
                string logMessage = _logQueue.Dequeue();
                _logWriter?.WriteLine(logMessage);
            }

            // 记录关闭信息
            WriteLogInternal("日志系统已关闭", LogLevel.Info);

            // 释放资源
            _logWriter?.Flush();
            _logWriter?.Close();
            _logWriter?.Dispose();
            _logWriter = null;
        }
    }
}