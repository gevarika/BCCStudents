using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    /// <summary>
    /// ფონი-მენეჯერი, რომელიც პერიოდულად ამუშავებს SyncOutbox-ში დაგროვილ ჩანაწერებს.
    /// </summary>
    public sealed class UpStreamSyncManager : IDisposable, IUpStreamSyncManager
    {
        private readonly IUpStreamSyncRepository _repository;
        private readonly IUpStreamSyncService _syncService;
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly ISyncLogger _logger;
        private readonly TimeSpan _interval;
        private readonly int _maxAttempts;

        private System.Threading.Timer _timer;
        private int _isProcessing;
        private bool _disposed;

        /// <summary>
        /// Event რომელიც იძახება სინქრონიზაციის დასრულებისას
        /// </summary>
        public event EventHandler<SyncStatusEventArgs> SyncCompleted;

        public UpStreamSyncManager(
            IUpStreamSyncRepository repository,
            IUpStreamSyncService syncService,
            IDatabaseConnectionChecker connectionChecker,
            ISyncLogger logger,
            TimeSpan? interval = null,
            int maxAttempts = 5)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interval = interval ?? TimeSpan.FromSeconds(30); // დეფოლტად 30 წმ.
            _maxAttempts = Math.Max(1, maxAttempts);
        }

        /// <summary>
        /// იწყებს პერიოდულ დამუშავებას.
        /// </summary>
        public void Start()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(UpStreamSyncManager));
            if (_timer != null) return;

            // System.Threading.Timer — არა ConnectionMonitor-ის WinForms Timer (იხ. SyncPeriodicTimerRunner).
            _timer = new System.Threading.Timer(
                SyncPeriodicTimerRunner.CreateCallback(ProcessPendingAsync, _logger, "UpStream"),
                null,
                TimeSpan.Zero,
                _interval);

            _logger.Info("UpStreamSyncManager started (SyncOutbox retry loop).");
        }

        /// <summary>
        /// აჩერებს ტაიმერს (მაგ. აპის დახურვისას).
        /// </summary>
        public void Stop()
        {
            if (_disposed) return;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;
            _logger.Info("UpStreamSyncManager stopped.");
        }

        private async Task ProcessPendingAsync()
        {
            if (_disposed) return;
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                return; // უკვე მუშაობს
            }

            int successCount = 0;
            int failedCount = 0;
            List<string> errors = new List<string>();

            try
            {
                var items = await _repository.GetPendingItemsAsync(50).ConfigureAwait(false);
                if (items.Count == 0)
                {
                    return;
                }

                var serverCheck = _connectionChecker.CheckServerConnection();
                if (!serverCheck.IsConnected)
                {
                    var logDetail = serverCheck.Failure?.LogDetail
                        ?? serverCheck.Failure?.UserMessage
                        ?? "სერვერთან კავშირი ვერ დამყარდა.";
                    SyncLogThrottle.TryWarn(_logger, "upstream-server-offline",
                        $"UpStream sync გამოტოვებულია. {logDetail}",
                        SyncLogThrottle.DefaultInterval);
                    return;
                }

                foreach (var item in items)
                {
                    try
                    {
                        await ProcessSingleItemAsync(item).ConfigureAwait(false);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        errors.Add($"Item {item.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (SyncConnectionHelper.IsLikelyConnectionError(ex))
                {
                    SyncLogThrottle.TryError(_logger, "upstream-connection", "UpStreamSyncManager unhandled error.", ex, SyncLogThrottle.DefaultInterval);
                }
                else
                {
                    _logger.Error("UpStreamSyncManager unhandled error.", ex);
                }

                errors.Add(ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);

                try
                {
                    var stats = await _repository.GetStatsAsync().ConfigureAwait(false);
                    if (stats.PendingCount > 0 || stats.DeadLetterCount > 0)
                    {
                        SyncLogThrottle.TryWarn(_logger, "upstream-outbox-stats",
                            $"SyncOutbox queue: pending={stats.PendingCount}, dead-letter={stats.DeadLetterCount}",
                            SyncLogThrottle.DefaultInterval);
                    }
                }
                catch (Exception statsEx)
                {
                    if (!SyncConnectionHelper.IsLikelyConnectionError(statsEx))
                        _logger.Error("SyncOutbox stats query failed.", statsEx);
                }

                // Event-ის გამოძახება
                if (SyncCompleted != null)
                {
                    var args = new SyncStatusEventArgs
                    {
                        Success = failedCount == 0,
                        RecordsSynced = successCount,
                        Errors = errors,
                        SyncType = "UpStream"
                    };
                    SyncCompleted?.Invoke(this, args);
                }
            }
        }

        private async Task ProcessSingleItemAsync(SyncOutboxItem item)
        {
            try
            {
                var payload = item.ToPayload();
                var result = await _syncService.TrySyncImmediatelyAsync(payload).ConfigureAwait(false);
                if (result.Success)
                {
                    await _repository.MarkAsSuccessAsync(item.Id).ConfigureAwait(false);
                    return;
                }

                var giveUp = item.Attempts + 1 >= _maxAttempts;
                await _repository.MarkAsFailedAsync(item.Id, result.ErrorMessage ?? "Immediate retry failed.", giveUp).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var giveUp = item.Attempts + 1 >= _maxAttempts;
                await _repository.MarkAsFailedAsync(item.Id, ex.Message, giveUp).ConfigureAwait(false);
                if (SyncConnectionHelper.IsLikelyConnectionError(ex))
                {
                    SyncLogThrottle.TryError(_logger, "upstream-retry-connection",
                        "UpStream retry failed (server connection).", ex, SyncLogThrottle.DefaultInterval);
                }
                else
                {
                    _logger.Error($"Retry failed for SyncOutbox Id={item.Id}.", ex);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            Stop();
            _disposed = true;
        }
    }
}



