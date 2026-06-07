using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Logging
{
    public sealed class ApplicationLogSyncManager : IDisposable, IApplicationLogSyncManager
    {
        private readonly IApplicationLogSyncService _syncService;
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly ISyncLogger _logger;
        private readonly TimeSpan _interval;
        private System.Threading.Timer _timer;
        private int _isProcessing;
        private bool _disposed;

        public ApplicationLogSyncManager(
            IApplicationLogSyncService syncService,
            IDatabaseConnectionChecker connectionChecker,
            ISyncLogger logger,
            TimeSpan? interval = null)
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interval = interval ?? TimeSpan.FromSeconds(60);
        }

        public void Start()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ApplicationLogSyncManager));
            if (_timer != null) return;

            _timer = new System.Threading.Timer(
                SyncPeriodicTimerRunner.CreateCallback(ProcessAsync, _logger, "ApplicationLogUpStream"),
                null,
                TimeSpan.FromSeconds(15),
                _interval);
        }

        public void Stop()
        {
            if (_disposed) return;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;
        }

        private async Task ProcessAsync()
        {
            if (_disposed) return;
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
                return;

            try
            {
                if (!_connectionChecker.CanConnectToServer())
                    return;

                await _syncService.SyncPendingToServerAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warn($"ApplicationLogs UpStream შეცდომა: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Stop();
        }
    }
}
