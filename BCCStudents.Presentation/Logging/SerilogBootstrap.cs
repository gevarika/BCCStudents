using BCCStudents.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BCCStudents.Presentation.Logging
{
    /// <summary>
    /// Central Serilog configuration for the desktop app (rolling app log under LocalAppData).
    /// Legacy files in the same folder (exception-log.txt, task-errors.txt, sync_log.txt, external-config.txt, db-init.txt, Update.txt) are no longer written; they may remain from older runs.
    /// </summary>
    public static class SerilogBootstrap
    {
        public const string SyncSourceContext = "Sync";

        private static MySqlApplicationLogSink _dbSink;

        public static string LogDirectory { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BCCStudents",
            "logs");

        public static void Initialize()
        {
            Directory.CreateDirectory(LogDirectory);
            Log.Logger = BuildLoggerConfiguration(includeDatabaseSink: false, dbSink: null).CreateLogger();
            Log.Information("Application starting");
        }

        public static void AddDatabaseSink(IServiceScopeFactory scopeFactory)
        {
            if (scopeFactory == null) throw new ArgumentNullException(nameof(scopeFactory));

            _dbSink?.Dispose();
            _dbSink = new MySqlApplicationLogSink(scopeFactory);
            Log.Logger = BuildLoggerConfiguration(includeDatabaseSink: true, dbSink: _dbSink).CreateLogger();
            Log.Information("Database log sink enabled");
        }

        private static LoggerConfiguration BuildLoggerConfiguration(bool includeDatabaseSink, MySqlApplicationLogSink dbSink)
        {
            var appLogPath = Path.Combine(LogDirectory, "app-.log");
            var syncLogPath = Path.Combine(LogDirectory, "sync-.log");
            const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "BCCStudents")
                .Enrich.With<UserEnricher>()
                .WriteTo.Logger(appOnly => appOnly
                    .Filter.ByExcluding(e => HasSourceContext(e, SyncSourceContext))
                    .WriteTo.File(
                        appLogPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        shared: true,
                        outputTemplate: logTemplate))
                .WriteTo.Logger(syncOnly => syncOnly
                    .Filter.ByIncludingOnly(e => HasSourceContext(e, SyncSourceContext))
                    .WriteTo.File(
                        syncLogPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        shared: true,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"));

            if (includeDatabaseSink && dbSink != null)
                config = config.WriteTo.Sink(dbSink);

            return config;
        }

        private static bool HasSourceContext(LogEvent logEvent, string sourceContext)
        {
            if (!logEvent.Properties.TryGetValue("SourceContext", out var value))
                return false;

            return value.ToString().Trim('"').Equals(sourceContext, StringComparison.Ordinal);
        }

        public static void Shutdown()
        {
            try
            {
                Log.Information("Application shutting down");
            }
            finally
            {
                Log.CloseAndFlush();
                _dbSink?.Dispose();
                _dbSink = null;
            }
        }

        private sealed class UserEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (ApplicationLogContext.UserId.HasValue)
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", ApplicationLogContext.UserId.Value));
                if (!string.IsNullOrWhiteSpace(ApplicationLogContext.Username))
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", ApplicationLogContext.Username));
            }
        }
    }
}
