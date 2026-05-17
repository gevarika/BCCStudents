using BCCStudents.Domain.Interfaces;
using System.Text;

namespace BCCStudents.Application.Services.Sync
{
    public class SyncLogger : ISyncLogger
    {
        private readonly string _logFilePath;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public SyncLogger()
        {
            var logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BCCStudents",
                "logs");

            Directory.CreateDirectory(logDir);
            _logFilePath = Path.Combine(logDir, "sync_log.txt");
        }

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
                // არაფერს ვაკეთებთ – არ უნდა დაბლოკოს აპლიკაცია.
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


