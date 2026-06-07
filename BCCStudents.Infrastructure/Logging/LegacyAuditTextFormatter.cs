using Serilog.Events;
using Serilog.Formatting;

namespace BCCStudents.Infrastructure.Logging
{
    /// <summary>
    /// LogViewerForm-ის თავსებადი ფორმატი: [თარიღი] Operation: ... | User: ... | Status: ...
    /// </summary>
    public sealed class LegacyAuditTextFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            var timestamp = logEvent.Timestamp.LocalDateTime;
            var operation = GetProperty(logEvent, "Operation");
            var user = GetProperty(logEvent, "User");
            var status = GetProperty(logEvent, "Status");
            var details = GetProperty(logEvent, "Details");

            output.Write(
                $"[{timestamp:yyyy-MM-dd HH:mm:ss}] Operation: {operation} | User: {user} | Status: {status}\nDetails:\n{details}\n\n");
        }

        private static string GetProperty(LogEvent logEvent, string name)
        {
            if (!logEvent.Properties.TryGetValue(name, out var value))
                return string.Empty;

            return value.ToString().Trim('"');
        }
    }
}
