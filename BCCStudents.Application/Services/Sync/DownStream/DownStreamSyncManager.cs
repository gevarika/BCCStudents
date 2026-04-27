using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    /// <summary>
    /// პერიოდულად იძახებს DownStreamSyncService-ს (მაგ. ყოველ 5 წუთში).
    /// </summary>
    public sealed class DownStreamSyncManager : IDisposable, IDownStreamSyncManager
    {
        private readonly IDownStreamSyncService _downStreamSyncService;
        private readonly ISyncLogger _logger;
        private readonly TimeSpan _interval;
        private System.Threading.Timer _timer;
        private int _isRunning;
        private bool _disposed;

        /// <summary>
        /// Event რომელიც იძახება სინქრონიზაციის დასრულებისას
        /// </summary>
        public event EventHandler<SyncStatusEventArgs> SyncCompleted;

        public DownStreamSyncManager(IDownStreamSyncService downStreamSyncService, ISyncLogger logger, TimeSpan? interval = null)
        {
            _downStreamSyncService = downStreamSyncService ?? throw new ArgumentNullException(nameof(downStreamSyncService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interval = interval ?? TimeSpan.FromMinutes(5);
        }

        public void Start()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DownStreamSyncManager));
            if (_timer != null) return;

            _timer = new System.Threading.Timer(async _ => await ExecuteAsync().ConfigureAwait(false),
                null,
                TimeSpan.Zero,
                _interval);

            _logger.Info("DownStreamSyncManager started.");
        }

        public void Stop()
        {
            if (_disposed) return;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;
            _logger.Info("DownStreamSyncManager stopped.");
        }

        private async Task ExecuteAsync()
        {
            if (_disposed) return;
            if (Interlocked.Exchange(ref _isRunning, 1) == 1)
            {
                return;
            }

            BCCStudents.Domain.Entities.SyncResult result = null;
            try
            {
                result = await _downStreamSyncService.SyncFromServerAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error("DownStream periodic sync failed.", ex);
                result = new BCCStudents.Domain.Entities.SyncResult();
                result.AddError(ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
                // Event-ის გამოძახება UI thread-ზე
                if (SyncCompleted != null && result != null)
                {
                    var args = new SyncStatusEventArgs
                    {
                        Success = result.Success,
                        RecordsSynced = result.Tables.Sum(t => t.RecordsSynced),
                        Errors = result.Errors.ToList(),
                        SyncType = "DownStream"
                    };
                    SyncCompleted?.Invoke(this, args);
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



