using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.AutoFileDetection
{
    /// <summary>
    /// áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒáƒ¦áƒ›áƒáƒ©áƒ”áƒœáƒ˜áƒ¡ áƒ™áƒáƒœáƒ¤áƒ˜áƒ’áƒ£áƒ áƒáƒªáƒ˜áƒ
    /// </summary>
    public class AutoFileDetectionConfig
    {
        /// <summary>
        /// áƒ¡áƒáƒ¥áƒáƒ¦áƒáƒšáƒ“áƒ˜áƒ¡ áƒ’áƒ–áƒ áƒ¡áƒáƒ“áƒáƒª áƒ›áƒáƒœáƒ˜áƒ¢áƒáƒ áƒ”áƒ‘áƒ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜
        /// </summary>
        public string WatchFolderPath { get; set; } = Path.Combine(System.Windows.Forms.Application.StartupPath, "Students");

        /// <summary>
        /// áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ¡ áƒœáƒ˜áƒ›áƒ£áƒ¨áƒ˜ (áƒ›áƒáƒ’: *.xlsx, payments*.xls)
        /// </summary>
        public string FileNamePattern { get; set; } = "*.xlsx";

        /// <summary>
        /// áƒ›áƒ®áƒáƒ áƒ“áƒáƒ­áƒ”áƒ áƒ˜áƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒáƒ¤áƒáƒ áƒ—áƒáƒ”áƒ‘áƒ”áƒ‘áƒ˜
        /// </summary>
        public string[] SupportedExtensions { get; set; } = { ".xlsx", ".xls" };

        /// <summary>
        /// áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜ áƒ¬áƒ£áƒ—áƒ”áƒ‘áƒ¨áƒ˜
        /// </summary>
        public int CheckIntervalMinutes { get; set; } = 5;

        /// <summary>
        /// áƒ¤áƒ£áƒœáƒ¥áƒªáƒ˜áƒ˜áƒ¡ áƒ©áƒáƒ áƒ—áƒ•áƒ/áƒ’áƒáƒ›áƒáƒ áƒ—áƒ•áƒ
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ áƒ—áƒ•áƒ/áƒ’áƒáƒ›áƒáƒ áƒ—áƒ•áƒ
        /// </summary>
        public bool ShowNotifications { get; set; } = true;

        /// <summary>
        /// áƒ›áƒáƒ¥áƒ¡áƒ˜áƒ›áƒáƒšáƒ£áƒ áƒ˜ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ áƒáƒáƒ“áƒ”áƒœáƒáƒ‘áƒ áƒ”áƒ áƒ—áƒ“áƒ áƒáƒ£áƒšáƒáƒ“ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡
        /// </summary>
        public int MaxFilesToCheck { get; set; } = 50;

        /// <summary>
        /// áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ›áƒáƒ¥áƒ¡áƒ˜áƒ›áƒáƒšáƒ£áƒ áƒ˜ áƒ–áƒáƒ›áƒ MB-áƒ¨áƒ˜
        /// </summary>
        public long MaxFileSizeMB { get; set; } = 100;

        /// <summary>
        /// áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ›áƒáƒ¥áƒ¡áƒ˜áƒ›áƒáƒšáƒ£áƒ áƒ˜ áƒ–áƒáƒ›áƒ áƒ‘áƒáƒ˜áƒ¢áƒ”áƒ‘áƒ¨áƒ˜
        /// </summary>
        public long MaxFileSizeBytes => MaxFileSizeMB * 1024 * 1024;

        /// <summary>
        /// áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒáƒœáƒ”
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// áƒ™áƒáƒœáƒ¤áƒ˜áƒ’áƒ£áƒ áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ app.config-áƒ˜áƒ“áƒáƒœ
        /// </summary>
        public static AutoFileDetectionConfig LoadFromConfig()
        {
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
        /// áƒ™áƒáƒœáƒ¤áƒ˜áƒ’áƒ£áƒ áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ¨áƒ”áƒœáƒáƒ®áƒ•áƒ app.config-áƒ¨áƒ˜
        /// </summary>
        public void SaveToConfig()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            
            config.AppSettings.Settings["AutoFileDetection.WatchFolderPath"].Value = WatchFolderPath;
            config.AppSettings.Settings["AutoFileDetection.FileNamePattern"].Value = FileNamePattern;
            config.AppSettings.Settings["AutoFileDetection.CheckIntervalMinutes"].Value = CheckIntervalMinutes.ToString();
            config.AppSettings.Settings["AutoFileDetection.Enabled"].Value = Enabled.ToString();
            config.AppSettings.Settings["AutoFileDetection.ShowNotifications"].Value = ShowNotifications.ToString();
            config.AppSettings.Settings["AutoFileDetection.MaxFilesToCheck"].Value = MaxFilesToCheck.ToString();
            config.AppSettings.Settings["AutoFileDetection.MaxFileSizeMB"].Value = MaxFileSizeMB.ToString();
            config.AppSettings.Settings["AutoFileDetection.LogLevel"].Value = LogLevel.ToString();
            
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    /// <summary>
    /// áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒáƒœáƒ”
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}

