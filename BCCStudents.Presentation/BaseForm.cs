using BCCStudents.Application.Interfaces;
using BCCStudents.Presentation.Services;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// საერთო ბაზა — სინქრონიზაციის და კავშირის სტატუსის ზოლი.
    /// </summary>
    public partial class BaseForm : Form
    {
        protected SyncStatusControl? SyncStatusControl { get; private set; }
        protected StatusStrip? StatusStrip { get; private set; }

        protected BaseForm()
        {
            Load += BaseForm_Load;
        }

        private void BaseForm_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;

            ConnectionStatusBarHostAccessor.Attach(this);
        }

        protected void InitializeSyncStatus(IDownStreamSyncManager downStreamManager, IUpStreamSyncManager upStreamManager)
        {
            if (StatusStrip == null)
            {
                StatusStrip = new StatusStrip
                {
                    Name = "statusStrip",
                    Dock = DockStyle.Bottom
                };
                Controls.Add(StatusStrip);
            }

            SyncStatusControl = new SyncStatusControl();
            SyncStatusControl.SetInvokeControl(StatusStrip);
            var statusItems = SyncStatusControl.GetStatusItems();

            foreach (var item in statusItems)
            {
                StatusStrip.Items.Add(item);
            }

            if (downStreamManager != null)
            {
                downStreamManager.SyncCompleted += (_, args) =>
                {
                    SyncStatusControl?.UpdateDownStreamStatus(args);
                };
            }

            if (upStreamManager != null)
            {
                upStreamManager.SyncCompleted += (_, args) =>
                {
                    SyncStatusControl?.UpdateUpStreamStatus(args);
                };
            }

            ConnectionStatusBarHostAccessor.Attach(this);
        }
    }

    /// <summary>
    /// DI-ში რეგისტრირებული host-ის წვდომა BaseForm-იდან.
    /// </summary>
    internal static class ConnectionStatusBarHostAccessor
    {
        public static ConnectionStatusBarHost? Instance { get; private set; }

        public static void Initialize(ConnectionStatusBarHost host) => Instance = host;

        public static void Attach(Form form) => Instance?.Attach(form);
    }
}
