using BCCStudents.Domain.Interfaces;
using BCCStudents.Presentation.Properties;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BCCStudents.Presentation.Services
{
    /// <summary>
    /// კონფიგურაციის სერვისის იმპლემენტაცია - Properties.Settings-ის გამოყენებით
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        // Database selection & connections
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

        /*public string ServerHost
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
        }*/

        public async Task SaveConnectionSettingsAsync(string settingName, string server, string port, string database, string username, string password)
        {
            string protectedPassword = Protect(password);
            string connStr = $"Server={server};Port={port};Database={database};User={username};Password={protectedPassword};SslMode=Preferred;";

            await Task.Run(() =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var section = config.ConnectionStrings;

                // გამოიყენეთ გადაცემული სახელი (მაგ: "LocalMySqlConnectionString")
                if (section.ConnectionStrings[settingName] != null)
                {
                    section.ConnectionStrings[settingName].ConnectionString = connStr;
                }
                else
                {
                    // თუ ასეთი სახელით ვერ იპოვა, ამატებს ახალს
                    section.ConnectionStrings.Add(new ConnectionStringSettings(settingName, connStr, "MySql.Data.MySqlClient"));
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            });
        }

        // DPAPI დამხმარე მეთოდი
        private string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            // შიფრავს მონაცემებს მიმდინარე მომხმარებლის დონეზე
            byte[] protectedData = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedData);
        }

        // SMS configuration
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

        public string SmsText_Registration
        {
            get => Settings.Default.SmsText_Registration;
            set { Settings.Default.SmsText_Registration = value; }
        }

        public string SmsText_Payment
        {
            get => Settings.Default.SmsText_Payment;
            set { Settings.Default.SmsText_Payment = value; }
        }

        public string SmsText_UpcomingReminder
        {
            get => Settings.Default.SmsText_UpcomingReminder;
            set { Settings.Default.SmsText_UpcomingReminder = value; }
        }

        public string SmsText_OverdueReminder
        {
            get => Settings.Default.SmsText_OverdueReminder;
            set { Settings.Default.SmsText_OverdueReminder = value; }
        }

        // Backup configuration
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

        public bool AutoBackupEnabled
        {
            get => Settings.Default.AutoBackupEnabled;
            set { Settings.Default.AutoBackupEnabled = value; }
        }

        public string MySqlDumpPath
        {
            get => Settings.Default.MySqlDumpPath;
            set { Settings.Default.MySqlDumpPath = value; }
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

        public DateTime LastBackupTime
        {
            get => Settings.Default.LastBackupTime;
            set { Settings.Default.LastBackupTime = value; }
        }

        public bool DbChangedSinceLastBackup
        {
            get => Settings.Default.DbChangedSinceLastBackup;
            set { Settings.Default.DbChangedSinceLastBackup = value; }
        }

        // Sync / update flags
        public bool AutoDownstreamSyncEnabled
        {
            get => Settings.Default.AutoDownstreamSyncEnabled;
            set { Settings.Default.AutoDownstreamSyncEnabled = value; }
        }

        public bool AutoUpstreamSyncEnabled
        {
            get => Settings.Default.AutoUpstreamSyncEnabled;
            set { Settings.Default.AutoUpstreamSyncEnabled = value; }
        }

        public bool AutoUpdateEnabled
        {
            get => Settings.Default.AutoUpdateEnabled;
            set { Settings.Default.AutoUpdateEnabled = value; }
        }

        public bool UpgradeRequired
        {
            get => Settings.Default.UpgradeRequired;
            set { Settings.Default.UpgradeRequired = value; }
        }

        // Admin
        public string AdminCode
        {
            get => Settings.Default.AdminCode;
            set { Settings.Default.AdminCode = value; }
        }

        public string UsedUsernames
        {
            get => Settings.Default.UsedUsernames;
            set { Settings.Default.UsedUsernames = value; }
        }
        public void Save()
        {
            Settings.Default.Save();
        }
        //Payments
        public bool UseFullBalanceForAutoPayment
        {
            get => Settings.Default.UseFullBalanceForAutoPayment;
            set { Settings.Default.UseFullBalanceForAutoPayment = value; }
        }

        public bool AllowPartialPayments
        {
            get => Settings.Default.AllowPartialPayments;
            set { Settings.Default.AllowPartialPayments = value; }
        }

        //Files and directories
        public void SaveAutoDetectionSettings(bool enabled, string watchPath, string pattern)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            UpdateSetting(config, "AutoFileDetection.Enabled", enabled.ToString());
            UpdateSetting(config, "AutoFileDetection.WatchFolderPath", watchPath);
            UpdateSetting(config, "AutoFileDetection.FileNamePattern", pattern);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        // დამხმარე მეთოდი კოდის შესამცირებლად
        private void UpdateSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }

        public bool IsAutoDetectionEnabled()
        {
            var value = ConfigurationManager.AppSettings["AutoFileDetection.Enabled"];
            return bool.TryParse(value, out bool result) ? result : true; // Default არის true
        }

        public string GetWatchFolderPath()
        {
            return ConfigurationManager.AppSettings["AutoFileDetection.WatchFolderPath"] ?? string.Empty;
        }

        public string GetFileNamePattern()
        {
            return ConfigurationManager.AppSettings["AutoFileDetection.FileNamePattern"] ?? "*.xlsx";
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

        /// <summary>
        /// წაიკითხავს კავშირის სტრიქონს App.config-დან
        /// </summary>
        public string GetConnectionString(string connectionStringName)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
                return connectionString?.ConnectionString;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading connection string '{connectionStringName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// ინახავს კავშირის სტრიქონს App.config-ში
        /// </summary>
        public void SaveConnectionString(string connectionStringName, string connectionString)
        {
            try
            {
                // მივიღოთ config ფაილის path ConfigurationManager-ის მეშვეობით
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string configPath = config.FilePath;

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(configPath);

                System.Xml.XmlNode node = xmlDoc.SelectSingleNode($"//connectionStrings/add[@name='{connectionStringName}']");
                if (node != null)
                {
                    node.Attributes["connectionString"].Value = connectionString;
                }
                else
                {
                    // თუ კვანძი არ არსებობს, შევქმნათ ახალი
                    System.Xml.XmlNode connectionStringsNode = xmlDoc.SelectSingleNode("//connectionStrings");
                    if (connectionStringsNode != null)
                    {
                        System.Xml.XmlElement newElement = xmlDoc.CreateElement("add");
                        newElement.SetAttribute("name", connectionStringName);
                        newElement.SetAttribute("connectionString", connectionString);
                        connectionStringsNode.AppendChild(newElement);
                    }
                }

                xmlDoc.Save(configPath);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving connection string '{connectionStringName}': {ex.Message}");
                throw;
            }
        }
    }
}
