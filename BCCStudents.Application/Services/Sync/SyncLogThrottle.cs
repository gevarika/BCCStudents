using BCCStudents.Domain.Interfaces;
using System.Collections.Concurrent;

namespace BCCStudents.Application.Services.Sync
{
    /// <summary>
    /// იგივე შეტყობინების გამეორებით ლოგირების შეზღუდვა (მაგ. ოფლაინ რეჟიმში).
    /// </summary>
    internal static class SyncLogThrottle
    {
        private static readonly ConcurrentDictionary<string, DateTime> LastLoggedUtc = new();

        public static readonly TimeSpan DefaultInterval = TimeSpan.FromMinutes(1);

        public static bool TryWarn(ISyncLogger logger, string key, string message, TimeSpan? interval = null)
        {
            if (!ShouldLog(key, interval ?? DefaultInterval))
                return false;

            logger.Warn(message);
            return true;
        }

        public static bool TryError(ISyncLogger logger, string key, string message, Exception? exception, TimeSpan? interval = null)
        {
            if (!ShouldLog(key, interval ?? DefaultInterval))
                return false;

            logger.Error(message, exception);
            return true;
        }

        private static bool ShouldLog(string key, TimeSpan interval)
        {
            var now = DateTime.UtcNow;
            if (LastLoggedUtc.TryGetValue(key, out var last) && now - last < interval)
                return false;

            LastLoggedUtc[key] = now;
            return true;
        }
    }
}
