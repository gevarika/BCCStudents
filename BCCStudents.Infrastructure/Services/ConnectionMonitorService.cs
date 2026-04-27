using BCCStudents.Application.Interfaces;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// კავშირის მონიტორინგის სერვისი - პერიოდულად ამოწმებს MySQL კავშირს
    /// იმპლემენტირებს IConnectionMonitor ინტერფეისს
    /// </summary>
    public class ConnectionMonitorService : IConnectionMonitor, IDisposable
    {
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly System.Windows.Forms.Timer _monitorTimer;
        private bool _isConnected;
        private bool _isDisposed;
        private readonly SynchronizationContext _syncContext; // UI thread-ის სინქრონიზაციისთვის
        private const int CHECK_INTERVAL = 30000; // 30 წამი (შეიძლება კონფიგურაციიდან)

        /// <summary>
        /// კავშირის მიმდინარე სტატუსი
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// ივენთი, რომელიც იძახება კავშირის სტატუსის ცვლილებისას
        /// </summary>
        public event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// კონსტრუქტორი - იღებს IDatabaseConnectionChecker-ს Dependency Injection-ით
        /// </summary>
        /// <param name="connectionChecker">IDatabaseConnectionChecker ინსტანსი კავშირის შესამოწმებლად</param>
        public ConnectionMonitorService(IDatabaseConnectionChecker connectionChecker)
        {
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _isConnected = false;
            _isDisposed = false;

            // Windows Forms-ის SynchronizationContext-ის მიღება
            // თუ null-ია, ვქმნით WindowsFormsSynchronizationContext-ს
            _syncContext = SynchronizationContext.Current;
            if (_syncContext == null)
            {
                // თუ არ არის UI thread-ზე, ვქმნით WindowsFormsSynchronizationContext-ს
                _syncContext = new WindowsFormsSynchronizationContext();
            }

            _monitorTimer = new System.Windows.Forms.Timer();
            _monitorTimer.Interval = CHECK_INTERVAL;
            _monitorTimer.Tick += MonitorTimer_Tick;
        }

        /// <summary>
        /// მონიტორინგის დაწყება
        /// </summary>
        public void StartMonitoring()
        {
            if (_isDisposed)
                return;

            // საწყისი შემოწმება
            CheckConnection();

            _monitorTimer.Start();
        }

        /// <summary>
        /// მონიტორინგის გაჩერება
        /// </summary>
        public void StopMonitoring()
        {
            _monitorTimer?.Stop();
        }

        /// <summary>
        /// ტაიმერის Tick ივენთის handler
        /// </summary>
        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (_isDisposed)
                return;

            CheckConnection();
        }

        /// <summary>
        /// კავშირის შემოწმება და ივენთის გამოძახება ცვლილებისას
        /// </summary>
        private void CheckConnection()
        {
            try
            {
                bool currentStatus = _connectionChecker.CanConnectToMySQL();

                if (currentStatus != _isConnected)
                {
                    bool previousStatus = _isConnected;
                    _isConnected = currentStatus;

                    // ივენთის გამოძახება UI thread-ზე
                    _syncContext.Post(state =>
                    {
                        ConnectionStatusChanged?.Invoke(this, _isConnected);
                    }, null);
                }
            }
            catch
            {
                // შეცდომის შემთხვევაში კავშირი გათიშულია
                if (_isConnected)
                {
                    _isConnected = false;
                    _syncContext.Post(state =>
                    {
                        ConnectionStatusChanged?.Invoke(this, false);
                    }, null);
                }
            }
        }

        /// <summary>
        /// რესურსების გათავისუფლება
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
}

