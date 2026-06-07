using BCCStudents.Domain.Interfaces;
using Serilog;

namespace BCCStudents.Application.Services.Sync
{
    /// <summary>
    /// Sync logging via Serilog (SourceContext=Sync). Requires SerilogBootstrap.Initialize() before first use.
    /// </summary>
    public class SyncLogger : ISyncLogger
    {
        private readonly ILogger _logger = Log.ForContext("SourceContext", "Sync");

        public void Info(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            _logger.Information(message);
        }

        public void Warn(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            _logger.Warning(message);
        }

        public void Error(string message, Exception? exception = null)
        {
            if (string.IsNullOrWhiteSpace(message) && exception == null)
                return;

            if (exception == null)
                _logger.Error(message);
            else
                _logger.Error(exception, message);
        }
    }
}
