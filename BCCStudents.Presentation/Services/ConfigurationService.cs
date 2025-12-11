using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Presentation.Properties;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// კონფიგურაციის სერვისის იმპლემენტაცია - Properties.Settings-ის გამოყენებით
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        public bool IsTestDb
        {
            get => Settings.Default.IsTestDb;
            set { Settings.Default.IsTestDb = value; }
        }

        public bool UseLocalDb
        {
            get => Settings.Default.UseLocalDb;
            set { Settings.Default.UseLocalDb = value; }
        }

        public string LocalMySqlConnectionString
        {
            get => Settings.Default.LocalMySqlConnectionString;
            set { Settings.Default.LocalMySqlConnectionString = value; }
        }

        public string ServerMySqlConnectionString
        {
            get => Settings.Default.ServerMySqlConnectionString;
            set { Settings.Default.ServerMySqlConnectionString = value; }
        }

        public string LocalMySqlConnectionString_Test
        {
            get => Settings.Default.LocalMySqlConnectionString_Test;
            set { Settings.Default.LocalMySqlConnectionString_Test = value; }
        }

        public string ServerMySqlConnectionString_Test
        {
            get => Settings.Default.ServerMySqlConnectionString_Test;
            set { Settings.Default.ServerMySqlConnectionString_Test = value; }
        }

        public string ServerHost
        {
            get => Settings.Default.ServerHost;
            set { Settings.Default.ServerHost = value; }
        }

        public string DatabaseName
        {
            get => Settings.Default.DatabaseName;
            set { Settings.Default.DatabaseName = value; }
        }

        public string Username
        {
            get => Settings.Default.Username;
            set { Settings.Default.Username = value; }
        }

        public string Password
        {
            get => Settings.Default.Password;
            set { Settings.Default.Password = value; }
        }

        public bool SmsEnabled
        {
            get => Settings.Default.SmsEnabled;
            set { Settings.Default.SmsEnabled = value; }
        }

        public string SmsApiKey
        {
            get => Settings.Default.SmsApiKey;
            set { Settings.Default.SmsApiKey = value; }
        }

        public string AdminCode
        {
            get => Settings.Default.AdminCode;
            set { Settings.Default.AdminCode = value; }
        }

        public bool DbChangedSinceLastBackup
        {
            get => Settings.Default.DbChangedSinceLastBackup;
            set { Settings.Default.DbChangedSinceLastBackup = value; }
        }

        public DateTime LastBackupTime
        {
            get => Settings.Default.LastBackupTime;
            set { Settings.Default.LastBackupTime = value; }
        }

        public string BackupDirectory
        {
            get => Settings.Default.BackupDirectory;
            set { Settings.Default.BackupDirectory = value; }
        }

        public int BackupIntervalMinutes
        {
            get => Settings.Default.BackupIntervalMinutes;
            set { Settings.Default.BackupIntervalMinutes = value; }
        }

        public int BackupIntervalHours
        {
            get => Settings.Default.BackupIntervalHours;
            set { Settings.Default.BackupIntervalHours = value; }
        }

        public int MaxBackupFiles
        {
            get => Settings.Default.MaxBackupFiles;
            set { Settings.Default.MaxBackupFiles = value; }
        }

        public string BackupUsername
        {
            get => Settings.Default.BackupUsername;
            set { Settings.Default.BackupUsername = value; }
        }

        public string BackupPassword
        {
            get => Settings.Default.BackupPassword;
            set { Settings.Default.BackupPassword = value; }
        }

        public void Save()
        {
            Settings.Default.Save();
        }

        /// <summary>
        /// წაიკითხავს Base64-ით კოდირებულ დაშიფრულ სტრიქონს App.config-დან და გაშიფრავს DPAPI-ის გამოყენებით
        /// </summary>
        /// <param name="appConfigKey">App.config-ში key-ის სახელი (appSettings-ისთვის) ან connectionStrings-ისთვის name</param>
        /// <param name="isConnectionString">true თუ ეს ConnectionStrings სექციიდანაა, false თუ appSettings-იდან</param>
        /// <returns>გაშიფრული სტრიქონი, ან null თუ key არ მოიძებნა ან შეცდომა მოხდა</returns>
        public string GetDecryptedConnectionStringFromConfig(string appConfigKey, bool isConnectionString = true)
        {
            try
            {
                string encryptedBase64 = null;

                if (isConnectionString)
                {
                    // წავიკითხოთ ConnectionStrings სექციიდან
                    var connectionString = ConfigurationManager.ConnectionStrings[appConfigKey];
                    if (connectionString == null)
                        return null;
                    
                    encryptedBase64 = connectionString.ConnectionString;
                }
                else
                {
                    // წავიკითხოთ appSettings-იდან
                    encryptedBase64 = ConfigurationManager.AppSettings[appConfigKey];
                }

                if (string.IsNullOrWhiteSpace(encryptedBase64))
                    return null;

                // Base64-დან გადაყვანა
                byte[] encryptedData;
                try
                {
                    encryptedData = Convert.FromBase64String(encryptedBase64);
                }
                catch (FormatException)
                {
                    // თუ Base64 არ არის, შეიძლება უკვე გაშიფრული იყოს
                    return encryptedBase64;
                }

                // DPAPI-ით გაშიფრავა
                // DataProtectionScope.CurrentUser - მხოლოდ მიმდინარე მომხმარებლისთვის
                // DataProtectionScope.LocalMachine - ყველა მომხმარებლისთვის ამ მანქანაზე
                byte[] decryptedData = ProtectedData.Unprotect(
                    encryptedData,
                    null, // optionalEntropy - null = მხოლოდ DPAPI
                    DataProtectionScope.CurrentUser // ან LocalMachine თუ სხვა მომხმარებლებიც უნდა წაიკითხონ
                );

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (CryptographicException ex)
            {
                // DPAPI შეცდომა - შეიძლება სხვა მომხმარებლის მიერ იყოს დაშიფრული
                System.Diagnostics.Debug.WriteLine($"DPAPI decryption failed for key '{appConfigKey}': {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading/decrypting config key '{appConfigKey}': {ex.Message}");
                return null;
            }
        }
    }
}
