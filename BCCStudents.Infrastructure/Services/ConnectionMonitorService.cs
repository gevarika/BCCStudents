using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using Serilog;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// კავშირის მონიტორინგის სერვისი - პერიოდულად ამოწმებს MySQL კავშირს
    /// </summary>
    public class ConnectionMonitorService : IConnectionMonitor, IDisposable
    {
        private static readonly ILogger ConnectionLog = Log.ForContext("SourceContext", "Connection");

        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly System.Windows.Forms.Timer _monitorTimer;
        private bool _isConnected;
        private bool _isServerConnected;
        private bool _initialServerCheckCompleted;
        private bool _isDisposed;
        private readonly SynchronizationContext _syncContext;
        private const int CheckInterval = 30000;

        public bool IsConnected => _isConnected;
        public bool IsServerConnected => _isServerConnected;
        public ConnectionFailureInfo? LastServerConnectionFailure { get; private set; }

        public event EventHandler<bool>? ConnectionStatusChanged;
        public event EventHandler<ServerConnectionChangedEventArgs>? ServerConnectionStatusChanged;

        public ConnectionMonitorService(IDatabaseConnectionChecker connectionChecker)
        {
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _syncContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();

            _monitorTimer = new System.Windows.Forms.Timer { Interval = CheckInterval };
            _monitorTimer.Tick += MonitorTimer_Tick;
        }

        public void StartMonitoring()
        {
            if (_isDisposed)
                return;

            _monitorTimer.Stop();
            CheckConnection();
            _monitorTimer.Start();
        }

        public void StopMonitoring() => _monitorTimer?.Stop();

        /// <summary>
        /// WinForms Timer (UI message loop) — არა System.Threading.Timer, რომელს იყენებენ სინქის მენეჯერები.
        /// </summary>
        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isDisposed)
                CheckConnection();
        }

        private void CheckConnection()
        {
            var localResult = _connectionChecker.CheckLocalConnection();
            var serverResult = _connectionChecker.CheckServerConnection();

            if (localResult.IsConnected != _isConnected)
            {
                _isConnected = localResult.IsConnected;
                _syncContext.Post(_ => ConnectionStatusChanged?.Invoke(this, _isConnected), null);
            }

            if (!_initialServerCheckCompleted)
            {
                _initialServerCheckCompleted = true;
                _isServerConnected = serverResult.IsConnected;
                LastServerConnectionFailure = serverResult.Failure;

                if (!serverResult.IsConnected)
                    LogServerFailedAtStartup(serverResult.Failure);

                RaiseServerConnectionChanged();
                return;
            }

            if (serverResult.IsConnected == _isServerConnected)
                return;

            _isServerConnected = serverResult.IsConnected;
            LastServerConnectionFailure = serverResult.Failure;

            if (_isServerConnected)
            {
                ConnectionLog.Information("სერვერთან კავშირი აღდგა");
                LastServerConnectionFailure = null;
            }
            else
            {
                LogServerDisconnected(serverResult.Failure);
            }

            RaiseServerConnectionChanged();
        }

        private void RaiseServerConnectionChanged()
        {
            var args = new ServerConnectionChangedEventArgs
            {
                IsConnected = _isServerConnected,
                Failure = LastServerConnectionFailure
            };

            _syncContext.Post(_ => ServerConnectionStatusChanged?.Invoke(this, args), null);
        }

        private static void LogServerFailedAtStartup(ConnectionFailureInfo? failure)
        {
            if (failure != null)
            {
                ConnectionLog.Warning(
                    "სერვერთან კავშირი ვერ დამყარდა. {LogDetail}",
                    failure.LogDetail);
                return;
            }

            ConnectionLog.Warning("სერვერთან კავშირი ვერ დამყარდა");
        }

        private static void LogServerDisconnected(ConnectionFailureInfo? failure)
        {
            if (failure != null)
            {
                ConnectionLog.Warning(
                    "სერვერთან კავშირი გაწყდა. {LogDetail}",
                    failure.LogDetail);
                return;
            }

            ConnectionLog.Warning("სერვერთან კავშირი გაწყდა");
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _monitorTimer?.Stop();
            _monitorTimer?.Dispose();
            _isDisposed = true;
        }
    }
}
