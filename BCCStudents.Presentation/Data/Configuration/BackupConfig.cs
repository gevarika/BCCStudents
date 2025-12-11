using System;
using System.IO;
using System.Windows.Forms;
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

        public static string MySqlDumpPath
        {
            get => string.IsNullOrEmpty(Settings.Default.MySqlDumpPath) 
                ? @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe" 
                : Settings.Default.MySqlDumpPath;
            set
            {
                Settings.Default.MySqlDumpPath = value;
                Settings.Default.Save();
            }
        }

        public static string ServerHost
        {
            get => string.IsNullOrEmpty(Settings.Default.ServerHost) 
                ? "bccenter.ge" 
                : Settings.Default.ServerHost;
            set
            {
                Settings.Default.ServerHost = value;
                Settings.Default.Save();
            }
        }

        public static string DatabaseName
        {
            get => string.IsNullOrEmpty(Settings.Default.DatabaseName) 
                ? "bccenter_SchoolManagement_Test" 
                : Settings.Default.DatabaseName;
            set
            {
                Settings.Default.DatabaseName = value;
                Settings.Default.Save();
            }
        }

        public static string Username
        {
            get => string.IsNullOrEmpty(Settings.Default.BackupUsername) 
                ? "bccenter_schoolAdmin25" 
                : Settings.Default.BackupUsername;
            set
            {
                Settings.Default.BackupUsername = value;
                Settings.Default.Save();
            }
        }

        public static string Password
        {
            get => string.IsNullOrEmpty(Settings.Default.BackupPassword) 
                ? "@wh0X0kpPHey" 
                : Settings.Default.BackupPassword;
            set
            {
                Settings.Default.BackupPassword = value;
                Settings.Default.Save();
            }
        }

        public static DateTime LastBackupTime
        {
            get => Settings.Default.LastBackupTime;
            set
            {
                Settings.Default.LastBackupTime = value;
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

