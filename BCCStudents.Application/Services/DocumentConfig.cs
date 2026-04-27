using Newtonsoft.Json;

namespace BCCStudents.Application.Services
{
    public class DocumentConfig
    {
        // კონფიგურაცია ინახება AppData-ში რომ update-ისას არ წაიშალოს
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BCCStudents", "Config", "document_config.json");

        public string DownloadPath { get; set; }
        public string FileUrl { get; set; }

        public void Save()
        {
            // უზრუნველვყოფთ რომ directory არსებობს
            var directory = Path.GetDirectoryName(ConfigFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        public static DocumentConfig Load()
        {
            if (!File.Exists(ConfigFilePath))
                return new DocumentConfig();

            string json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<DocumentConfig>(json) ?? new DocumentConfig();
        }
    }
}

