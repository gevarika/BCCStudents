using Microsoft.Toolkit.Uwp.Notifications;
using System.Runtime.InteropServices;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// COM activator unpackaged desktop toast-ებისთვის (Toolkit 7).
    /// </summary>
    [ComVisible(true)]
    [Guid(ToastActivatorClsid)]
    public sealed class BccToastNotificationActivator : NotificationActivator
    {
        internal const string ToastActivatorClsid = "A4E8B91C-3F2D-4B6E-9C1A-5D7E2F8B4C6D";

        public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId)
        {
            // საცდელი: toast-ზე დაკლიკება — ამ ეტაპზე ცარიელი.
        }
    }
}
