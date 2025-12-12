namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// კონფიგურაციის სერვისის ინტერფეისი - Properties.Settings-ის ჩანაცვლება
    /// </summary>
    public interface IConfigurationService
    {
        bool IsTestDb { get; set; }
        bool UseLocalDb { get; set; }
        string LocalMySqlConnectionString { get; set; }
        string ServerMySqlConnectionString { get; set; }
        string LocalMySqlConnectionString_Test { get; set; }
        string ServerMySqlConnectionString_Test { get; set; }
        string ServerHost { get; set; }
        string DatabaseName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool SmsEnabled { get; set; }
        string SmsApiKey { get; set; }
        string AdminCode { get; set; }
        bool DbChangedSinceLastBackup { get; set; }
        DateTime LastBackupTime { get; set; }
        string BackupDirectory { get; set; }
        int BackupIntervalMinutes { get; set; }
        int BackupIntervalHours { get; set; }
        int MaxBackupFiles { get; set; }
        string BackupUsername { get; set; }
        string BackupPassword { get; set; }
        
        void Save();
        
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

