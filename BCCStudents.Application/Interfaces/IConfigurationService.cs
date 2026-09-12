namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// კონფიგურაციის სერვისის ინტერფეისი - Properties.Settings-ის ჩანაცვლება
    /// </summary>
    public interface IConfigurationService
    {
        // Database selection & connections
        bool IsTestDb { get; set; }
        bool UseLocalDb { get; set; }
        string LocalMySqlConnectionString { get; set; }
        string ServerMySqlConnectionString { get; set; }
        string LocalMySqlConnectionString_Test { get; set; }
        string ServerMySqlConnectionString_Test { get; set; }
        //string ServerHost { get; set; }
        //string DatabaseName { get; set; }
        //string Username { get; set; }
        //string Password { get; set; }
        Task SaveConnectionSettingsAsync(string settingName, string server, string port, string database, string username, string password);

        // SMS configuration
        bool SmsEnabled { get; set; }
        string SmsApiKey { get; set; }
        string SmsText_Registration { get; set; }
        string SmsText_Payment { get; set; }
        string SmsText_UpcomingReminder { get; set; }
        string SmsText_OverdueReminder { get; set; }

        // Backup configuration
        string BackupDirectory { get; set; }
        int BackupIntervalMinutes { get; set; }
        int BackupIntervalHours { get; set; }
        int MaxBackupFiles { get; set; }
        bool AutoBackupEnabled { get; set; }
        string MySqlDumpPath { get; set; }
        string BackupUsername { get; set; }
        string BackupPassword { get; set; }
        DateTime LastBackupTime { get; set; }
        bool DbChangedSinceLastBackup { get; set; }

        // Update flags
        bool AutoUpdateEnabled { get; set; }
        bool UpgradeRequired { get; set; }

        // Admin
        string AdminCode { get; set; }
        string UsedUsernames { get; set; }
        //Payments
        bool UseFullBalanceForAutoPayment { get; set; }
        bool AllowPartialPayments { get; set; }
        void Save();

        //Files and directories
        // ახალი მეთოდი ავტო-დეტექციის პარამეტრებისთვის
        void SaveAutoDetectionSettings(bool enabled, string watchPath, string pattern);

        // ასევე დაგჭირდებათ მათი წაკითხვა ფორმის ჩატვირთვისას
        bool IsAutoDetectionEnabled();
        string GetWatchFolderPath();
        string GetFileNamePattern();
        /// <summary>
        /// წაიკითხავს კავშირის სტრიქონს App.config-დან
        /// </summary>
        /// <param name="connectionStringName">კავშირის სტრიქონის სახელი App.config-ში</param>
        /// <returns>კავშირის სტრიქონი, ან null თუ არ მოიძებნა</returns>
        string GetConnectionString(string connectionStringName);

        /// <summary>
        /// ინახავს კავშირის სტრიქონს App.config-ში
        /// </summary>
        /// <param name="connectionStringName">კავშირის სტრიქონის სახელი App.config-ში</param>
        /// <param name="connectionString">კავშირის სტრიქონი შესანახად</param>
        void SaveConnectionString(string connectionStringName, string connectionString);
    }
}

