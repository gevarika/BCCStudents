using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// ლოკალური/სერვერის კავშირის სტატუსი ყველა ფორმაზე (ქვედა status strip).
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
            EnsureEventsHooked();
            RefreshAllLabels();
        }

        public void Attach(Form form)
        {
            if (form.IsDisposed)
                return;

            if (form is MainForm or LoginForm or RegisterForm)
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

                var localLabel = new ToolStripStatusLabel
                {
                    Name = "connectionLocalStatusLabel",
                    BorderSides = ToolStripStatusLabelBorderSides.Right,
                    BorderStyle = Border3DStyle.Etched,
                    AutoSize = true
                };

                var serverLabel = new ToolStripStatusLabel
                {
                    Name = "connectionServerStatusLabel",
                    Spring = true,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                strip.Items.Insert(0, serverLabel);
                strip.Items.Insert(0, localLabel);

                lock (_gate)
                {
                    _bindings[form] = new StatusBinding(localLabel, serverLabel);
                }

                form.FormClosed += OnAttachedFormClosed;
                ApplyLocalState(localLabel, _connectionStatusService.IsConnected);
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

            _connectionStatusService.ConnectionStatusChanged += OnLocalConnectionChanged;
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

        private void OnLocalConnectionChanged(object? sender, EventArgs e) =>
            RefreshAllLabels();

        private void OnLocalMonitorChanged(object? sender, bool isConnected) =>
            RefreshAllLabels();

        private void OnServerConnectionChanged(object? sender, ServerConnectionChangedEventArgs e)
        {
            RefreshAllLabels();

            if (_mainLocalLabel == null)
                return;

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
                        "სინქრონიზაცია დროებით შეჩერდება. დეტალები — ლოგში (app-*.log).");

                    // ძველი MessageBox (საცდელად toast-ზე გადასვლა):
                    // MessageBox.Show(
                    //     mainForm,
                    //     message + Environment.NewLine + Environment.NewLine +
                    //     "სინქრონიზაცია დროებით შეჩერდება. დეტალები — ლოგში (app-*.log).",
                    //     "სერვერი გათიშულია",
                    //     MessageBoxButtons.OK,
                    //     MessageBoxIcon.Warning);
                    return;
                }

                var wasOfflineNotified = _serverOfflineDialogShown;
                _serverOfflineDialogShown = false;

                if (wasOfflineNotified)
                {
                    WindowsToastNotifier.ShowServerOnline(
                        "სერვერი დაკავშირებულია",
                        "სერვერთან კავშირი აღდგა. სინქრონიზაცია განახლდება ავტომატურად.");

                    // ძველი MessageBox:
                    // MessageBox.Show(
                    //     mainForm,
                    //     "სერვერთან კავშირი აღდგა." + Environment.NewLine + Environment.NewLine +
                    //     "სინქრონიზაცია განახლდება ავტომატურად.",
                    //     "სერვერი დაკავშირებულია",
                    //     MessageBoxButtons.OK,
                    //     MessageBoxIcon.Information);
                }
            }

            if (mainForm.InvokeRequired)
                mainForm.BeginInvoke(HandleOnMain);
            else
                HandleOnMain();
        }

        public void RefreshAllLabels()
        {
            var localConnected = _connectionStatusService.IsConnected;
            var serverConnected = _connectionMonitor.IsServerConnected;
            var serverFailure = _connectionMonitor.LastServerConnectionFailure;

            if (_mainLocalLabel != null && !_mainLocalLabel.IsDisposed)
                ApplyLocalState(_mainLocalLabel, localConnected);

            if (_mainServerLabel != null && !_mainServerLabel.IsDisposed)
                ApplyServerState(_mainServerLabel, serverConnected, serverFailure);

            List<StatusBinding> snapshot;
            lock (_gate)
            {
                snapshot = _bindings.Values.ToList();
            }

            foreach (var binding in snapshot)
            {
                if (binding.Local.IsDisposed || binding.Server.IsDisposed)
                    continue;

                ApplyLocalState(binding.Local, localConnected);
                ApplyServerState(binding.Server, serverConnected, serverFailure);
            }
        }

        private static void ApplyLocalState(ToolStripStatusLabel label, bool isConnected)
        {
            if (isConnected)
            {
                label.Text = "✅ ლოკალური ბაზა";
                label.ForeColor = Color.Green;
            }
            else
            {
                label.Text = "❌ ლოკალური ბაზა";
                label.ForeColor = Color.Red;
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
            public StatusBinding(ToolStripStatusLabel local, ToolStripStatusLabel server)
            {
                Local = local;
                Server = server;
            }

            public ToolStripStatusLabel Local { get; }
            public ToolStripStatusLabel Server { get; }
        }
    }
}
