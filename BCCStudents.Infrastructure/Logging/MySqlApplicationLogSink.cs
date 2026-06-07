using System.Threading.Channels;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;

namespace BCCStudents.Infrastructure.Logging
{
    public sealed class MySqlApplicationLogSink : ILogEventSink, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Channel<ApplicationLogEntry> _channel;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _worker;
        private int _disposeStarted;

        public MySqlApplicationLogSink(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _channel = Channel.CreateBounded<ApplicationLogEntry>(new BoundedChannelOptions(2000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });
            _worker = Task.Run(ProcessQueueAsync);
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null || Volatile.Read(ref _disposeStarted) != 0)
                return;

            if (!ApplicationLogDatabaseFilter.ShouldPersist(logEvent))
                return;

            try
            {
                var entry = Map(logEvent);
                _channel.Writer.TryWrite(entry);
            }
            catch
            {
                // Never throw from sink.
            }
        }

        private async Task ProcessQueueAsync()
        {
            var batch = new List<ApplicationLogEntry>(50);
            try
            {
                await foreach (var entry in _channel.Reader.ReadAllAsync(_cts.Token))
                {
                    batch.Add(entry);
                    while (batch.Count < 50 && _channel.Reader.TryRead(out var next))
                        batch.Add(next);

                    await FlushBatchAsync(batch).ConfigureAwait(false);
                    batch.Clear();
                }
            }
            catch (OperationCanceledException)
            {
                // shutdown
            }
            finally
            {
                if (batch.Count > 0)
                    await FlushBatchAsync(batch).ConfigureAwait(false);
            }
        }

        private async Task FlushBatchAsync(List<ApplicationLogEntry> batch)
        {
            if (batch.Count == 0)
                return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IApplicationLogRepository>();
                // Shutdown-ისას _cts უკვე Cancel/Dispose შეიძლება იყოს — ბოლო batch მაინც უნდა ჩაიწეროს.
                await repository.InsertBatchAsync(batch, CancellationToken.None).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            catch (ObjectDisposedException)
            {
                // shutdown race
            }
            catch
            {
                // DB unavailable — drop batch silently.
            }
        }

        private static ApplicationLogEntry Map(LogEvent logEvent)
        {
            var sourceContext = GetScalar(logEvent, "SourceContext");
            var sourceType = ResolveSourceType(sourceContext, logEvent);
            var message = logEvent.RenderMessage();
            var exception = logEvent.Exception?.ToString();

            return new ApplicationLogEntry
            {
                LogGuid = Guid.NewGuid().ToString(),
                SourceType = sourceType,
                Category = ResolveCategory(sourceType, sourceContext),
                Level = logEvent.Level.ToString(),
                Operation = GetScalar(logEvent, "Operation"),
                Status = GetScalar(logEvent, "Status"),
                UserId = ApplicationLogContext.UserId,
                Username = ApplicationLogContext.Username,
                MachineName = Environment.MachineName,
                PermissionScope = null,
                Message = LogMessageSanitizer.Sanitize(message),
                Details = LogMessageSanitizer.Sanitize(GetScalar(logEvent, "Details")),
                Exception = LogMessageSanitizer.Sanitize(exception),
                SourceContext = sourceContext,
                CreatedAt = logEvent.Timestamp.UtcDateTime,
                Origin = "Local"
            };
        }

        private static string ResolveSourceType(string sourceContext, LogEvent logEvent)
        {
            if (string.Equals(sourceContext, "Sync", StringComparison.Ordinal))
                return LogSourceType.Sync;

            if (string.Equals(sourceContext, "Connection", StringComparison.Ordinal) ||
                GetScalar(logEvent, "SourceType") == LogSourceType.Connection)
                return LogSourceType.Connection;

            if (!string.IsNullOrWhiteSpace(GetScalar(logEvent, "Operation")))
                return LogSourceType.Audit;

            return LogSourceType.App;
        }

        private static string ResolveCategory(string sourceType, string sourceContext)
        {
            if (sourceType == LogSourceType.Sync)
                return LogCategory.System;
            if (sourceType == LogSourceType.Connection)
                return LogCategory.System;

            return LogCategory.System;
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

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeStarted, 1) != 0)
                return;

            _channel.Writer.TryComplete();
            try
            {
                _cts.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // already disposed
            }

            try
            {
                _worker.Wait(TimeSpan.FromSeconds(10));
            }
            catch
            {
                // best effort
            }

            _cts.Dispose();
        }
    }
}
