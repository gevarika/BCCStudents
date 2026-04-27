using BCCStudents.Presentation.Properties;

namespace BCCStudents.Presentation.Data.Configuration
{
    public static class BackupConfig
    {
        // ბექაპის კონფიგურაციის პარამეტრები
        public static string BackupDirectory
        {
            get => string.IsNullOrEmpty(Settings.Default.BackupDirectory)
                ? Path.Combine(System.Windows.Forms.Application.StartupPath, "Backups")
                : Settings.Default.BackupDirectory;
            set
            {
                Settings.Default.BackupDirectory = value;
                Settings.Default.Save();
            }
        }

        public static int BackupIntervalHours
        {
            get => Settings.Default.BackupIntervalHours;
            set
            {
                Settings.Default.BackupIntervalHours = value;
                Settings.Default.Save();
            }
        }

        public static int MaxBackupFiles
        {
            get => Settings.Default.MaxBackupFiles;
            set
            {
                Settings.Default.MaxBackupFiles = value;
                Settings.Default.Save();
            }
        }

        public static bool AutoBackupEnabled
        {
            get => Settings.Default.AutoBackupEnabled;
            set
            {
                Settings.Default.AutoBackupEnabled = value;
                Settings.Default.Save();
            }
        }

        // ბექაპის კონფიგურაციის განახლება BackupManager-ში
        // შენიშვნა: ეს მეთოდი მოითხოვს BackupManager instance-ს, რომელიც უნდა იყოს dependency injection-ით გადაცემული
        public static void UpdateBackupManagerSettings()
        {
            Console.WriteLine($"BackupConfig.BackupDirectory: {BackupDirectory}");
            Console.WriteLine($"BackupConfig.BackupIntervalHours: {BackupIntervalHours}");
            Console.WriteLine($"BackupConfig.MaxBackupFiles: {MaxBackupFiles}");
            Console.WriteLine($"BackupConfig.AutoBackupEnabled: {AutoBackupEnabled}");

            // ეს მეთოდი ახლა მხოლოდ Settings-ს ინახავს
            // BackupManager instance-ის განახლება უნდა მოხდეს DI-ს მეშვეობით
        }

        // ბექაპის კონფიგურაციის შენახვა BackupManager-დან
        // შენიშვნა: ეს მეთოდი მოითხოვს BackupManager instance-ს
        public static void SaveBackupManagerSettings()
        {
            // ეს მეთოდი ახლა მხოლოდ Settings-ს ინახავს
            // BackupManager instance-ის მნიშვნელობები უნდა იყოს უკვე განახლებული
        }
    }
}

