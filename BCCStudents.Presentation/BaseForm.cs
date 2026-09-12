using BCCStudents.Presentation.Services;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// საერთო ბაზა — კავშირის სტატუსის ზოლი.
    /// </summary>
    public partial class BaseForm : Form
    {
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
