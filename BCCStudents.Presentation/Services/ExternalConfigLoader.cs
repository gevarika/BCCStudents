using BCCStudents.Presentation.Properties;
using Serilog;
using System.Text.Json;

namespace BCCStudents.Presentation.Services
{
    public static class ExternalConfigLoader
    {
        private const string ConfigFileName = "db.config.json";

        public static ExternalDbConfig Load()
        {
            try
            {
                var path = GetConfigPath();
                if (!File.Exists(path))
                {
                    return null;
                }

                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<ExternalDbConfig>(json);
            }
            catch (Exception ex)
            {
                LogError("Failed to load external DB config.", ex);
                return null;
            }
        }

        public static void ApplyOverrides(ExternalDbConfig config)
        {
            if (config == null)
            {
                return;
            }

            if (config.UseLocalDb.HasValue)
            {
                Settings.Default.UseLocalDb = config.UseLocalDb.Value;
            }

            if (config.IsTestDb.HasValue)
            {
                Settings.Default.IsTestDb = config.IsTestDb.Value;
            }

            if (!string.IsNullOrWhiteSpace(config.LocalMySqlConnectionString))
            {
                if (!IsPlaceholderConnectionString(config.LocalMySqlConnectionString))
                {
                    Settings.Default.LocalMySqlConnectionString = config.LocalMySqlConnectionString;
                }
            }

            if (!string.IsNullOrWhiteSpace(config.LocalMySqlConnectionString_Test))
            {
                if (!IsPlaceholderConnectionString(config.LocalMySqlConnectionString_Test))
                {
                    Settings.Default.LocalMySqlConnectionString_Test = config.LocalMySqlConnectionString_Test;
                }
            }

            if (!string.IsNullOrWhiteSpace(config.ServerMySqlConnectionString))
            {
                if (!IsPlaceholderConnectionString(config.ServerMySqlConnectionString))
                {
                    Settings.Default.ServerMySqlConnectionString = config.ServerMySqlConnectionString;
                }
            }

            if (!string.IsNullOrWhiteSpace(config.ServerMySqlConnectionString_Test))
            {
                if (!IsPlaceholderConnectionString(config.ServerMySqlConnectionString_Test))
                {
                    Settings.Default.ServerMySqlConnectionString_Test = config.ServerMySqlConnectionString_Test;
                }
            }
        }

        public static string ResolveInitScriptPath(ExternalDbConfig config)
        {
            var scriptPath = config?.InitScriptPath;
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                scriptPath = Path.Combine("Database", "db_init.sql");
            }

            if (Path.IsPathRooted(scriptPath))
            {
                return scriptPath;
            }

            var resolvedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptPath);
            if (File.Exists(resolvedPath))
            {
                return resolvedPath;
            }

            var databaseScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "db_init.sql");
            return databaseScriptPath;
        }

        private static bool IsPlaceholderConnectionString(string connectionString)
        {
            return connectionString.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetConfigPath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "BCCStudents");

            return Path.Combine(dir, ConfigFileName);
        }

        private static void LogError(string message, Exception ex)
        {
            Log.ForContext("SourceContext", "ExternalConfig")
                .Error(ex, message);
        }
    }
}
