using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;
using System.Runtime.Versioning;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// Windows Action Center toast-ების რეგისტრაცია (unpackaged WinForms).
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static class WindowsToastBootstrap
    {
        public const string AppUserModelId = "BCCStudents.StudentManagement";

        private static bool _registered;

        public static void Initialize()
        {
            if (_registered)
                return;

            try
            {
                DesktopNotificationManagerCompat.RegisterAumidAndComServer<BccToastNotificationActivator>(AppUserModelId);
                _registered = true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Windows toast registration failed; connection alerts will not use toasts");
            }
        }
    }
}
