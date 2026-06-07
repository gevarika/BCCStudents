using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.Logging
{
    public sealed class ApplicationLogRetentionService : IDisposable, IApplicationLogRetentionService
    {
        private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(90);
        private readonly IApplicationLogRepository _repository;
        private readonly ISyncLogger _logger;
        private System.Threading.Timer _timer;
        private int _isProcessing;
        private bool _disposed;

        public ApplicationLogRetentionService(IApplicationLogRepository repository, ISyncLogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Start()
        {
            if (_disposed) return;
            if (_timer != null) return;

            _timer = new System.Threading.Timer(_ => _ = RunCleanupAsync(), null, TimeSpan.FromHours(1), TimeSpan.FromHours(24));
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;
        }

        public async Task<int> RunCleanupAsync(CancellationToken cancellationToken = default)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
                return 0;

            try
            {
                var cutoff = DateTime.UtcNow.Subtract(RetentionPeriod);
                var deleted = await _repository.DeleteOlderThanAsync(cutoff, cancellationToken).ConfigureAwait(false);
                if (deleted > 0)
                    _logger.Info($"ApplicationLogs retention: {deleted} ჩანაწერი წაიშალა (>{RetentionPeriod.TotalDays:0} დღე).");
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.Warn($"ApplicationLogs retention შეცდომა: {ex.Message}");
                return 0;
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
