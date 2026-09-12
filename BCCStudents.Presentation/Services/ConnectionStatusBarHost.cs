using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// სერვერის კავშირის სტატუსი ყველა ფორმაზე (ქვედა status strip). Server-only რეჟიმი.
    /// </summary>
    public sealed class ConnectionStatusBarHost
    {
        private readonly IConnectionMonitor _connectionMonitor;
        private readonly IConnectionStatusService _connectionStatusService;
        private readonly object _gate = new();
        private readonly Dictionary<Form, StatusBinding> _bindings = new();
        private Form? _mainForm;
        private ToolStripStatusLabel? _mainLocalLabel;
        private ToolStripStatusLabel? _mainServerLabel;
        private bool _serverOfflineDialogShown;
        private bool _eventsHooked;

        public ConnectionStatusBarHost(
            IConnectionMonitor connectionMonitor,
            IConnectionStatusService connectionStatusService)
        {
            _connectionMonitor = connectionMonitor;
            _connectionStatusService = connectionStatusService;
        }

        public void BindMainForm(Form mainForm, ToolStripStatusLabel localLabel, ToolStripStatusLabel serverLabel)
        {
            _mainForm = mainForm;
            _mainLocalLabel = localLabel;
            _mainServerLabel = serverLabel;
            // ლოკალური ლეიბლი აღარ სჭირდება — მხოლოდ სერვერი
            if (_mainLocalLabel != null)
                _mainLocalLabel.Visible = false;
            EnsureEventsHooked();
            RefreshAllLabels();
        }

        public void Attach(Form form)
        {
            if (form.IsDisposed)
                return;

            if (form is MainForm or LoginForm)
                return;

            lock (_gate)
            {
                if (_bindings.ContainsKey(form))
                    return;
            }

            void AttachCore()
            {
                if (form.IsDisposed)
                    return;

                var strip = form.Controls.OfType<StatusStrip>().FirstOrDefault();
                if (strip == null)
                {
                    strip = new StatusStrip
                    {
                        Name = "connectionStatusStrip",
                        Dock = DockStyle.Bottom
                    };
                    form.Controls.Add(strip);
                }

                var serverLabel = new ToolStripStatusLabel
                {
                    Name = "connectionServerStatusLabel",
                    Spring = true,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                strip.Items.Insert(0, serverLabel);

                lock (_gate)
                {
                    _bindings[form] = new StatusBinding(serverLabel);
                }

                form.FormClosed += OnAttachedFormClosed;
                ApplyServerState(
                    serverLabel,
                    _connectionMonitor.IsServerConnected,
                    _connectionMonitor.LastServerConnectionFailure);
            }

            if (form.InvokeRequired)
                form.BeginInvoke(AttachCore);
            else
                AttachCore();

            EnsureEventsHooked();
        }

        private void EnsureEventsHooked()
        {
            if (_eventsHooked)
                return;

            _connectionStatusService.ConnectionStatusChanged += OnConnectionChanged;
            _connectionMonitor.ConnectionStatusChanged += OnLocalMonitorChanged;
            _connectionMonitor.ServerConnectionStatusChanged += OnServerConnectionChanged;
            _eventsHooked = true;
        }

        private void OnAttachedFormClosed(object? sender, FormClosedEventArgs e)
        {
            if (sender is not Form form)
                return;

            form.FormClosed -= OnAttachedFormClosed;
            lock (_gate)
            {
                _bindings.Remove(form);
            }
        }

        private void OnConnectionChanged(object? sender, EventArgs e) =>
            RefreshAllLabels();

        private void OnLocalMonitorChanged(object? sender, bool isConnected) =>
            RefreshAllLabels();

        private void OnServerConnectionChanged(object? sender, ServerConnectionChangedEventArgs e)
        {
            RefreshAllLabels();

            var mainForm = _mainForm;
            if (mainForm == null || mainForm.IsDisposed)
                return;

            void HandleOnMain()
            {
                if (!e.IsConnected)
                {
                    if (_serverOfflineDialogShown)
                        return;

                    _serverOfflineDialogShown = true;
                    var message = e.Failure?.UserMessage ?? "სერვერთან კავშირი ვერ დამყარდა.";
                    WindowsToastNotifier.ShowServerOffline(
                        "სერვერი გათიშულია",
                        message,
                        "მონაცემებთან მუშაობა შეუძლებელია სანამ კავშირი არ აღდგება. დეტალები — ლოგში (app-*.log).");
                    return;
                }

                var wasOfflineNotified = _serverOfflineDialogShown;
                _serverOfflineDialogShown = false;

                if (wasOfflineNotified)
                {
                    WindowsToastNotifier.ShowServerOnline(
                        "სერვერი დაკავშირებულია",
                        "სერვერთან კავშირი აღდგა.");
                }
            }

            if (mainForm.InvokeRequired)
                mainForm.BeginInvoke(HandleOnMain);
            else
                HandleOnMain();
        }

        public void RefreshAllLabels()
        {
            var serverConnected = _connectionMonitor.IsServerConnected;
            var serverFailure = _connectionMonitor.LastServerConnectionFailure;

            if (_mainServerLabel != null && !_mainServerLabel.IsDisposed)
                ApplyServerState(_mainServerLabel, serverConnected, serverFailure);

            List<StatusBinding> snapshot;
            lock (_gate)
            {
                snapshot = _bindings.Values.ToList();
            }

            foreach (var binding in snapshot)
            {
                if (binding.Server.IsDisposed)
                    continue;

                ApplyServerState(binding.Server, serverConnected, serverFailure);
            }
        }

        private static void ApplyServerState(
            ToolStripStatusLabel label,
            bool isConnected,
            ConnectionFailureInfo? failure)
        {
            if (isConnected)
            {
                label.Text = "✅ სერვერი";
                label.ForeColor = Color.Green;
                return;
            }

            var shortReason = failure?.UserMessage ?? "გათიშულია";
            label.Text = $"❌ სერვერი: {shortReason}";
            label.ForeColor = Color.Red;
        }

        private sealed class StatusBinding
        {
            public StatusBinding(ToolStripStatusLabel server)
            {
                Server = server;
            }

            public ToolStripStatusLabel Server { get; }
        }
    }
}
