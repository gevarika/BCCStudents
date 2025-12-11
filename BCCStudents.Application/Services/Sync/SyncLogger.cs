using System;
using System.IO;
using System.Text;
using System.Threading;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Sync
{
    public class SyncLogger : ISyncLogger
    {
        private readonly string _logFilePath;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void Info(string message) => Write("INFO", message);

        public void Warn(string message) => Write("WARN", message);

        public void Error(string message, Exception exception = null)
        {
            var fullMessage = exception == null
                ? message
                : $"{message}{Environment.NewLine}{exception}";
            Write("ERROR", fullMessage);
        }

        private void Write(string level, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";

            try
            {
                _lock.EnterWriteLock();
                File.AppendAllText(_logFilePath, logLine, Encoding.UTF8);
            }
            catch
            {
                // áƒáƒ áƒáƒ¤áƒ”áƒ áƒ¡ áƒ•áƒáƒ™áƒ”áƒ—áƒ”áƒ‘áƒ— â€“ áƒáƒ  áƒ£áƒœáƒ“áƒ áƒ“áƒáƒ‘áƒšáƒáƒ™áƒáƒ¡ áƒáƒžáƒšáƒ˜áƒ™áƒáƒªáƒ˜áƒ.
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                {
                    _lock.ExitWriteLock();
                }
            }
        }
    }
}


