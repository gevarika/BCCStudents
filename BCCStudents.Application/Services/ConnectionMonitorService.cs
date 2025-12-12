using System;
using System.Threading.Tasks;
using System.Threading;
using BCCStudents.Application.Interfaces;
using System.Windows.Forms;

namespace BCCStudents.Application.Services {
    /// <summary>
    /// áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ›áƒáƒœáƒ˜áƒ¢áƒáƒ áƒ˜áƒœáƒ'áƒ˜áƒ¡ áƒ¡áƒ"áƒ áƒ•áƒ˜áƒ¡áƒ˜ - áƒžáƒ"áƒ áƒ˜áƒáƒ"áƒ£áƒšáƒáƒ" áƒáƒ›áƒáƒ¬áƒ›áƒ"áƒ'áƒ¡ MySQL áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ¡
    /// </summary>
    public class ConnectionMonitorService : IDisposable
    {
        private readonly System.Windows.Forms.Timer _monitorTimer;
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private bool _isConnected;
        private bool _isDisposed;
        private readonly SynchronizationContext _syncContext; // UI thread-áƒ˜áƒ¡ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
        private const int CHECK_INTERVAL = 30000; // 30 áƒ¬áƒáƒ›áƒ˜ (áƒ›áƒ˜áƒšáƒ˜áƒ¬áƒáƒ›áƒ"áƒ'áƒ¨áƒ˜)

        /// <summary>
        /// áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒªáƒ•áƒšáƒ˜áƒšáƒ"áƒ'áƒ˜áƒ¡ event
        /// </summary>
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// áƒ›áƒ˜áƒ›áƒ"áƒ˜áƒœáƒáƒ áƒ" áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜
        /// </summary>
        public bool IsConnected => _isConnected;


        /// <summary>
        /// áƒ™áƒáƒœáƒ¡áƒ¢áƒ áƒ£áƒ¥áƒ¢áƒáƒ áƒ˜
        /// </summary>
        public ConnectionMonitorService(IDatabaseConnectionChecker connectionChecker)
        {
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _isConnected = false;
            _isDisposed = false;
            // Windows Forms-áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ SynchronizationContext-áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ
            // áƒ—áƒ£ null-áƒ˜áƒ, áƒ•áƒ˜áƒ§áƒ”áƒœáƒ”áƒ‘áƒ— WindowsFormsSynchronizationContext-áƒ¡
            _syncContext = SynchronizationContext.Current;
            if (_syncContext == null)
            {
                // áƒ—áƒ£ áƒáƒ  áƒáƒ áƒ˜áƒ¡ UI thread-áƒ–áƒ”, áƒ•áƒ¥áƒ›áƒœáƒ˜áƒ— WindowsFormsSynchronizationContext-áƒ¡
                _syncContext = new System.Windows.Forms.WindowsFormsSynchronizationContext();
            }

            _monitorTimer = new System.Windows.Forms.Timer();
            _monitorTimer.Tick += MonitorTimer_Tick;
        }

        /// <summary>
        /// áƒ›áƒáƒœáƒ˜áƒ¢áƒáƒ áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ
        /// </summary>
        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ConnectionMonitorService));

            // áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ áƒ¡áƒ˜áƒœáƒ¥áƒ áƒáƒœáƒ£áƒšáƒáƒ“, áƒ áƒáƒ› áƒ•áƒ˜áƒªáƒáƒ“áƒ”áƒ— áƒ¡áƒáƒ¬áƒ§áƒ˜áƒ¡áƒ˜ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜ (áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ—áƒáƒœ)
            try
            {
                _isConnected = CanConnectToServer();
            }
            catch
            {
                _isConnected = false;
            }

            // áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ Timer-áƒ˜áƒ— - áƒ§áƒáƒ•áƒ”áƒšáƒ—áƒ•áƒ˜áƒ¡ 30 áƒ¬áƒáƒ›áƒ˜áƒáƒœáƒ˜ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜
            _monitorTimer.Interval = CHECK_INTERVAL;
            _monitorTimer.Start();
        }

        /// <summary>
        /// Timer-áƒ˜áƒ¡ áƒ’áƒáƒ¨áƒ•áƒ”áƒ‘áƒ 30 áƒ¬áƒáƒ›áƒ˜áƒáƒœáƒ˜ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜áƒ—
        /// </summary>
        private void StartTimer()
        {
            if (_isDisposed)
                return;

            // Timer-áƒ˜áƒ¡ áƒ›áƒáƒ áƒ—áƒ•áƒ UI thread-áƒ–áƒ”
            _syncContext.Post(_ =>
            {
                if (!_isDisposed)
                {
                    _monitorTimer.Stop();
                    _monitorTimer.Interval = CHECK_INTERVAL;
                    _monitorTimer.Start();
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] Timer áƒ’áƒáƒ¨áƒ•áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ {CHECK_INTERVAL / 1000} áƒ¬áƒáƒ›áƒ˜áƒ¡ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜áƒ—. Timer Enabled: {_monitorTimer.Enabled}");
#endif
                }
            }, null);
        }

        /// <summary>
        /// áƒ›áƒáƒœáƒ˜áƒ¢áƒáƒ áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ’áƒáƒ©áƒ”áƒ áƒ”áƒ‘áƒ
        /// </summary>
        public void Stop()
        {
            _monitorTimer?.Stop();
        }

        /// <summary>
        /// Timer-áƒ˜áƒ¡ Tick event handler
        /// </summary>
        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (_isDisposed)
                return;

            // Timer-áƒ˜áƒ¡ áƒ’áƒáƒ©áƒ”áƒ áƒ”áƒ‘áƒ, áƒ áƒáƒ› áƒáƒ  áƒ’áƒáƒ›áƒ”áƒáƒ áƒ“áƒ”áƒ¡ Tick event
            _monitorTimer.Stop();

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] Timer Tick - {DateTime.Now:HH:mm:ss}, _isConnected: {_isConnected}");
#endif

            // async void-áƒ˜áƒ¡ áƒœáƒáƒªáƒ•áƒšáƒáƒ“ Task.Run-áƒ˜áƒ—
            _ = Task.Run(async () =>
            {
                await CheckConnectionAsync();
            });
        }

        /// <summary>
        /// áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ (áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ—áƒáƒœ)
        /// </summary>
        private async Task CheckConnectionAsync()
        {
            System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] CheckConnectionAsync áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ - {DateTime.Now:HH:mm:ss}");
            
            try
            {
                // áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ—áƒáƒœ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ
                bool currentStatus = await Task.Run(() => 
                {
                    System.Diagnostics.Debug.WriteLine("[ConnectionMonitor] CanConnectToServer() áƒ’áƒáƒ›áƒáƒ«áƒáƒ®áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ");
                    bool result = CanConnectToServer();
                    System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] CanConnectToServer() áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ˜: {result}");
                    return result;
                });
                
                System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] currentStatus: {currentStatus}, _isConnected: {_isConnected}");
                
                // áƒ—áƒ£ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜ áƒ¨áƒ”áƒ˜áƒªáƒ•áƒáƒšáƒ
                if (currentStatus != _isConnected)
                {
                    bool previousStatus = _isConnected;
                    _isConnected = currentStatus;
                    
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜ áƒ¨áƒ”áƒ˜áƒªáƒ•áƒáƒšáƒ: {previousStatus} -> {currentStatus}");
#endif
                    
                    // Timer-áƒ˜áƒ¡ áƒ’áƒáƒ’áƒ áƒ«áƒ”áƒšáƒ”áƒ‘áƒ 30 áƒ¬áƒáƒ›áƒ˜áƒáƒœáƒ˜ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜áƒ—
                    StartTimer();
                    
                    // Event-áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ«áƒáƒ®áƒ”áƒ‘áƒ (áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒáƒ¨áƒ˜ MainForm-áƒ¨áƒ˜ áƒ’áƒáƒ›áƒáƒ•áƒ áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ)
                    OnConnectionStatusChanged(previousStatus, currentStatus);
                }
                else
                {
                    // áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜ áƒáƒ  áƒ¨áƒ”áƒªáƒ•áƒšáƒ˜áƒšáƒ, Timer-áƒ˜áƒ¡ áƒ’áƒáƒ’áƒ áƒ«áƒ”áƒšáƒ”áƒ‘áƒ 30 áƒ¬áƒáƒ›áƒ˜áƒáƒœáƒ˜ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜áƒ—
                    StartTimer();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] Exception: {ex.GetType().Name} - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] StackTrace: {ex.StackTrace}");
                
                // áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒáƒ¨áƒ˜, áƒ—áƒ£ áƒáƒ“áƒ áƒ” áƒ˜áƒ§áƒ áƒ“áƒáƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜, áƒ›áƒ˜áƒ•áƒ˜áƒ©áƒœáƒ˜áƒáƒ— áƒ áƒáƒ› áƒ’áƒáƒ˜áƒ—áƒ˜áƒ¨áƒ
                if (_isConnected)
                {
                    bool previousStatus = _isConnected;
                    _isConnected = false;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("[ConnectionMonitor] áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ áƒ’áƒáƒ˜áƒ—áƒ˜áƒ¨áƒ (exception-áƒ˜áƒ¡ áƒ’áƒáƒ›áƒ)");
#endif
                    StartTimer();
                    OnConnectionStatusChanged(previousStatus, false);
                }
                else
                {
                    // áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ áƒ™áƒ•áƒšáƒáƒ• áƒáƒ  áƒáƒ áƒ˜áƒ¡, Timer-áƒ˜áƒ¡ áƒ’áƒáƒ’áƒ áƒ«áƒ”áƒšáƒ”áƒ‘áƒ
                    StartTimer();
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] CheckConnectionAsync áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜áƒ - {DateTime.Now:HH:mm:ss}");
        }

        /// <summary>
        /// áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ—áƒáƒœ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ
        /// </summary>
        private bool CanConnectToServer()
        {
            try
            {
                {
                    // Connection timeout-áƒ˜áƒ¡ áƒ“áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ (5 áƒ¬áƒáƒ›áƒ˜)
                    //connection.ConnectionTimeout = 5;
                bool result = _connectionChecker.CanConnectToMySQL();
                System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] CanConnectToServer: áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒáƒ'áƒ£áƒšáƒ˜áƒ: {result}");
                return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] CanConnectToServer: áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ - {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Event-áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ«áƒáƒ®áƒ”áƒ‘áƒ
        /// </summary>
        protected virtual void OnConnectionStatusChanged(bool previousStatus, bool currentStatus)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒªáƒ•áƒšáƒ˜áƒšáƒ”áƒ‘áƒ: {previousStatus} -> {currentStatus}");
#endif
            
            if (ConnectionStatusChanged != null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[ConnectionMonitor] Event-áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ«áƒáƒ®áƒ”áƒ‘áƒ, subscribers: {ConnectionStatusChanged.GetInvocationList().Length}");
#endif
                ConnectionStatusChanged.Invoke(this, new ConnectionStatusChangedEventArgs
                {
                    PreviousStatus = previousStatus,
                    CurrentStatus = currentStatus,
                    Timestamp = DateTime.Now,
                    RetryAttempt = 0,
                    MaxRetriesExceeded = false
                });
            }
#if DEBUG
            else
            {
                System.Diagnostics.Debug.WriteLine("[ConnectionMonitor] Event-áƒ¡ áƒáƒ  áƒáƒ¥áƒ•áƒ¡ subscribers!");
            }
#endif
        }

        /// <summary>
        /// áƒ áƒ”áƒ¡áƒ£áƒ áƒ¡áƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒ—áƒáƒ•áƒ˜áƒ¡áƒ£áƒ¤áƒšáƒ”áƒ‘áƒ
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _monitorTimer?.Stop();
            _monitorTimer?.Dispose();
            _isDisposed = true;
        }
    }

    /// <summary>
    /// áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒªáƒ•áƒšáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ event arguments
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// áƒ¬áƒ˜áƒœáƒ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜
        /// </summary>
        public bool PreviousStatus { get; set; }

        /// <summary>
        /// áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜
        /// </summary>
        public bool CurrentStatus { get; set; }

        /// <summary>
        /// áƒªáƒ•áƒšáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒ
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// áƒ›áƒªáƒ“áƒ”áƒšáƒáƒ‘áƒ”áƒ‘áƒ˜áƒ¡ áƒ áƒáƒáƒ“áƒ”áƒœáƒáƒ‘áƒ
        /// </summary>
        public int RetryAttempt { get; set; }

        /// <summary>
        /// áƒ›áƒáƒ¥áƒ¡áƒ˜áƒ›áƒáƒšáƒ£áƒ áƒ˜ áƒ›áƒªáƒ“áƒ”áƒšáƒáƒ‘áƒ”áƒ‘áƒ˜ áƒ’áƒáƒ“áƒáƒªáƒ˜áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜áƒ
        /// </summary>
        public bool MaxRetriesExceeded { get; set; }
    }
}



