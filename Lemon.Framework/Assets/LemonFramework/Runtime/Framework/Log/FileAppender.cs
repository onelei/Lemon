using System;
using System.IO;
using UnityEngine;

namespace Lemon.Framework.Log
{
    public class FileAppender : IAppender
    {
        protected string FilePath;
        protected StreamWriter StreamWriter;
        private volatile object _locker = new object();

        public FileAppender() : this(null)
        {
        }

        public FileAppender(string filePath)
        {
            FilePath = filePath;
            if (string.IsNullOrEmpty(FilePath))
            {
                FilePath = Application.persistentDataPath + "/log.txt";
            }

            try
            {
                StreamWriter?.Dispose();
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }

                StreamWriter =
                    new StreamWriter(new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            }
            catch (Exception e)
            {
                throw new Exception($"FileAppender: {e.Message}");
            }
        }

        public string GetFilePath()
        {
            return FilePath;
        }

        public void Append(string msg)
        {
            lock (_locker)
            {
                StreamWriter?.WriteLine(msg);
                StreamWriter?.Flush();
            }
        }

        public void Dispose()
        {
            lock (_locker)
            {
                StreamWriter?.Dispose();
                StreamWriter = null;
            }
        }
    }
}