using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;
using System.Runtime.Versioning;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// სისტემური Windows toast შეტყობინებები (ქვედა მარჯვენა კუთხე).
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class WindowsToastNotifier
    {
        private const string ConnectionToastTag = "bcc-connection-status";

        public static void ShowServerOffline(string title, string detail, string footer)
        {
            Show(
                title,
                detail,
                footer,
                ToastScenario.Reminder);
        }

        public static void ShowServerOnline(string title, string detail)
        {
            Show(
                title,
                detail,
                null,
                ToastScenario.Default);
        }

        private static void Show(string title, string detail, string? footer, ToastScenario scenario)
        {
            try
            {
                var builder = new ToastContentBuilder()
                    .AddText(title)
                    .AddText(detail)
                    .SetToastScenario(scenario);

                if (!string.IsNullOrWhiteSpace(footer))
                    builder.AddText(footer);

                builder.Show(toast =>
                {
                    toast.Tag = ConnectionToastTag;
                    toast.Group = "bcc-connection";
                    toast.ExpirationTime = DateTimeOffset.Now.AddMinutes(5);
                });
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to show Windows toast: {Title}", title);
            }
        }
    }
}
