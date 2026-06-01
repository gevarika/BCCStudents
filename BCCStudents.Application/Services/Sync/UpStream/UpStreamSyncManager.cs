using BCCStudents.Application.Interfaces;
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
            ISyncLogger logger,
            TimeSpan? interval = null,
            int maxAttempts = 5)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
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

            _timer = new System.Threading.Timer(async _ => await ProcessPendingAsync().ConfigureAwait(false),
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
                _logger.Error("UpStreamSyncManager unhandled error.", ex);
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
                        _logger.Warn($"SyncOutbox queue: pending={stats.PendingCount}, dead-letter={stats.DeadLetterCount}");
                    }
                }
                catch (Exception statsEx)
                {
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
                _logger.Error($"Retry failed for SyncOutbox Id={item.Id}.", ex);
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



