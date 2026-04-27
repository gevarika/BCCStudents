using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        private static readonly string[] RequiredTables =
        {
            "Students",
            "Groups",
            "SubGroups",
            "StudentGroups",
            "StudentSubGroups",
            "Users",
            "Payments",
            "SystemConfig",
            "SyncOutbox",
            "SyncState",
            "PendingStudents",
            "PendingStudentGroups",
            "PendingStudentSubGroups",
            "ImportedFilesLog",
            "ImportedPaymentsLog",
            "FailedPayments",
            "FailedStudentImports"
        };

        public static bool TryEnsureLocalDatabase(IConfigurationService config, string initScriptPath, out string errorMessage)
        {
            errorMessage = null;

            if (config == null)
            {
                errorMessage = "Configuration service is not available.";
                return false;
            }

            if (!config.UseLocalDb)
            {
                return true;
            }

            try
            {
                var localConn = config.IsTestDb
                    ? (string.IsNullOrWhiteSpace(config.LocalMySqlConnectionString_Test)
                        ? config.LocalMySqlConnectionString
                        : config.LocalMySqlConnectionString_Test)
                    : config.LocalMySqlConnectionString;

                if (string.IsNullOrWhiteSpace(localConn))
                {
                    errorMessage = "Local MySQL connection string is empty.";
                    return false;
                }

                var builder = new MySqlConnectionStringBuilder(localConn);
                if (string.IsNullOrWhiteSpace(builder.Database))
                {
                    errorMessage = "Local MySQL connection string does not include a database name.";
                    return false;
                }

                var databaseName = builder.Database;
                builder.Database = string.Empty;

                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    if (!DatabaseExists(connection, databaseName))
                    {
                        using var createDb = new MySqlCommand($"CREATE DATABASE `{databaseName}` CHARACTER SET utf8mb4;", connection);
                        createDb.ExecuteNonQuery();
                    }
                }

                var missingTables = GetMissingTables(localConn, databaseName);
                if (missingTables.Count == 0)
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(initScriptPath) || !File.Exists(initScriptPath))
                {
                    errorMessage = $"Init script not found: {initScriptPath}";
                    return false;
                }

                using (var connection = new MySqlConnection(localConn))
                {
                    connection.Open();
                    var script = new MySqlScript(connection, File.ReadAllText(initScriptPath));
                    script.Execute();
                }

                missingTables = GetMissingTables(localConn, databaseName);
                if (missingTables.Count > 0)
                {
                    errorMessage = "Database initialized, but some required tables are still missing: " +
                                   string.Join(", ", missingTables);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError("Database initialization failed.", ex);
                errorMessage = ex.Message;
                return false;
            }
        }

        private static bool DatabaseExists(MySqlConnection connection, string databaseName)
        {
            using var cmd = new MySqlCommand(
                "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @db",
                connection);
            cmd.Parameters.AddWithValue("@db", databaseName);
            return cmd.ExecuteScalar() != null;
        }

        private static List<string> GetMissingTables(string connectionString, string databaseName)
        {
            var missing = new List<string>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using var cmd = new MySqlCommand(
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @db",
                    connection);
                cmd.Parameters.AddWithValue("@db", databaseName);

                var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existing.Add(reader.GetString(0));
                    }
                }

                foreach (var table in RequiredTables)
                {
                    if (!existing.Contains(table))
                    {
                        missing.Add(table);
                    }
                }
            }

            return missing;
        }

        private static void LogError(string message, Exception ex)
        {
            try
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "BCCStudents",
                    "logs");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, "db-init.txt");
                File.AppendAllText(path, $"[{DateTime.Now}] {message} {ex}\n\n");
            }
            catch
            {
                // ignore logging errors
            }
        }
    }
}
