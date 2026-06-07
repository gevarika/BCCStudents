using Serilog;
using Serilog.Core;

namespace BCCStudents.Infrastructure.Logging
{
    /// <summary>
    /// Per-category audit file loggers (students_log.txt, payments_log.txt, ...).
    /// </summary>
    public static class AuditLogFactory
    {
        private static readonly string BaseLogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BCCStudents",
            "logs");

        private static readonly object Gate = new();
        private static readonly Dictionary<string, Logger> Loggers = new(StringComparer.OrdinalIgnoreCase);

        public static ILogger GetLogger(string fileName)
        {
            lock (Gate)
            {
                if (Loggers.TryGetValue(fileName, out var existing))
                    return existing;

                Directory.CreateDirectory(BaseLogPath);

                var logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File(
                        path: Path.Combine(BaseLogPath, fileName),
                        formatter: new LegacyAuditTextFormatter(),
                        shared: true,
                        rollingInterval: RollingInterval.Infinite)
                    .CreateLogger();

                Loggers[fileName] = logger;
                return logger;
            }
        }

        public static void CloseAndFlush()
        {
            lock (Gate)
            {
                foreach (var logger in Loggers.Values)
                    logger.Dispose();

                Loggers.Clear();
            }
        }
    }
}
