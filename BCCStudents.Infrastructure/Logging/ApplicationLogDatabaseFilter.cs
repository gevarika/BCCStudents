using Serilog.Events;

namespace BCCStudents.Infrastructure.Logging
{
    /// <summary>
    /// ApplicationLogs ცხრილში მხოლოდ მნიშვნელოვანი ჩანაწერები — არა sync manager start/stop, არა ცარიელი ციკლები.
    /// Audit ჩანაწერები პირდაპირ repository-ში იწერება; ეს ფილტრი MySqlApplicationLogSink-ს ეხება.
    /// </summary>
    internal static class ApplicationLogDatabaseFilter
    {
        public static bool ShouldPersist(LogEvent logEvent)
        {
            if (logEvent == null)
                return false;

            var sourceContext = GetScalar(logEvent, "SourceContext");

            if (string.Equals(sourceContext, "Connection", StringComparison.Ordinal))
                return true;

            if (!string.IsNullOrWhiteSpace(GetScalar(logEvent, "Operation")))
                return true;

            if (string.Equals(sourceContext, "Sync", StringComparison.Ordinal))
                return IsSignificantSyncChange(logEvent);

            return false;
        }

        private static bool IsSignificantSyncChange(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage() ?? string.Empty;

            if (message.StartsWith("DownStream (Pull) წარმატებით ჩამოტვირთული:", StringComparison.Ordinal))
                return true;

            if (message.StartsWith("UpStream (Immediate) წარმატებით ატვირთული:", StringComparison.Ordinal))
                return true;

            if (message.StartsWith("DownStream sync დასრულდა: სულ ", StringComparison.Ordinal)
                && !message.Contains("სულ 0 ჩანაწერი", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        private static string GetScalar(LogEvent logEvent, string name)
        {
            if (!logEvent.Properties.TryGetValue(name, out var value))
                return null;

            var text = value.ToString();
            if (text.Length >= 2 && text.StartsWith('"') && text.EndsWith('"'))
                return text.Substring(1, text.Length - 2);

            return text;
        }
    }
}
