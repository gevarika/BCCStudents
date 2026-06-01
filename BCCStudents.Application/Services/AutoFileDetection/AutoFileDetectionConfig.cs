using System.Configuration;
using System.Text.Json;

namespace BCCStudents.Application.Services.AutoFileDetection
{
    /// <summary>
    /// ავტომატური ფაილის აღმოჩენის კონფიგურაცია
    /// </summary>
    public class AutoFileDetectionConfig
    {
        /// <summary>
        /// საქაღალდის გზა, სადაც მონიტორება ფაილები
        /// </summary>
        public string WatchFolderPath { get; set; } = Path.Combine(System.Windows.Forms.Application.StartupPath, "Students");

        /// <summary>
        /// ფაილის სახელის ნიმუში (მაგ: *.xlsx, payments*.xls)
        /// </summary>
        public string FileNamePattern { get; set; } = "*.xlsx";

        /// <summary>
        /// მხარდაჭერილი ფაილის გაფართოებები
        /// </summary>
        public string[] SupportedExtensions { get; set; } = { ".xlsx", ".xls" };

        /// <summary>
        /// შემოწმების ინტერვალი წუთებში
        /// </summary>
        public int CheckIntervalMinutes { get; set; } = 5;

        /// <summary>
        /// ფუნქციის ჩართვა/გამორთვა
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// ავტომატური შეტყობინებების ჩართვა/გამორთვა
        /// </summary>
        public bool ShowNotifications { get; set; } = true;

        /// <summary>
        /// მაქსიმალური ფაილების რაოდენობა ერთდროულად შემოწმებისას
        /// </summary>
        public int MaxFilesToCheck { get; set; } = 50;

        /// <summary>
        /// ფაილის მაქსიმალური ზომა MB-ში
        /// </summary>
        public long MaxFileSizeMB { get; set; } = 100;

        /// <summary>
        /// ფაილის მაქსიმალური ზომა ბაიტებში
        /// </summary>
        public long MaxFileSizeBytes => MaxFileSizeMB * 1024 * 1024;

        /// <summary>
        /// ლოგირების დონე
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// კონფიგურაციის ჩატვირთვა app.config-იდან
        /// </summary>
        public static AutoFileDetectionConfig LoadFromConfig()
        {
            var externalConfigPath = GetExternalConfigPath();
            if (File.Exists(externalConfigPath))
            {
                try
                {
                    var json = File.ReadAllText(externalConfigPath);
                    var externalConfig = JsonSerializer.Deserialize<AutoFileDetectionConfig>(json);
                    if (externalConfig != null)
                    {
                        return externalConfig;
                    }
                }
                catch
                {
                    // Fall back to app.config defaults if the writable config is unreadable.
                }
            }

            return new AutoFileDetectionConfig
            {
                WatchFolderPath = ConfigurationManager.AppSettings["AutoFileDetection.WatchFolderPath"]
                    ?? Path.Combine(System.Windows.Forms.Application.StartupPath, "Students"),
                FileNamePattern = ConfigurationManager.AppSettings["AutoFileDetection.FileNamePattern"] ?? "*.xlsx",
                CheckIntervalMinutes = int.Parse(ConfigurationManager.AppSettings["AutoFileDetection.CheckIntervalMinutes"] ?? "5"),
                Enabled = bool.Parse(ConfigurationManager.AppSettings["AutoFileDetection.Enabled"] ?? "true"),
                ShowNotifications = bool.Parse(ConfigurationManager.AppSettings["AutoFileDetection.ShowNotifications"] ?? "true"),
                MaxFilesToCheck = int.Parse(ConfigurationManager.AppSettings["AutoFileDetection.MaxFilesToCheck"] ?? "50"),
                MaxFileSizeMB = long.Parse(ConfigurationManager.AppSettings["AutoFileDetection.MaxFileSizeMB"] ?? "100"),
                LogLevel = Enum.TryParse<LogLevel>(ConfigurationManager.AppSettings["AutoFileDetection.LogLevel"], out var level)
                    ? level : LogLevel.Info
            };
        }

        /// <summary>
        /// კონფიგურაციის შენახვა app.config-ში
        /// </summary>
        public void SaveToConfig()
        {
            var externalConfigPath = GetExternalConfigPath();
            Directory.CreateDirectory(Path.GetDirectoryName(externalConfigPath)!);
            File.WriteAllText(externalConfigPath, JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        private static string GetExternalConfigPath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "BCCStudents");

            return Path.Combine(dir, "auto-detection.config.json");
        }
    }

    /// <summary>
    /// ლოგირების დონე
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}

