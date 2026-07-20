using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Services;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Data
{
    public class DatabaseHelper
    {
        private readonly IConfigurationService _config;

        public DatabaseHelper(IConfigurationService config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /*public SQLiteConnection GetSQLiteConnection()
        {
            string connName = Properties.Settings.Default.IsTestDb ? "SQLiteConnection_Test" : "SQLiteConnection";
            var connSetting = ConfigurationManager.ConnectionStrings[connName];
            if (connSetting == null)
                throw new Exception($"Connection string '{connName}' not found in config!");
            return new SQLiteConnection(connSetting.ConnectionString);
            return new SQLiteConnection(_sqliteConnectionString);
        }*/

        public MySqlConnection GetMySqlConnection()
        {


            // Use settings-based switch: local vs server, and test vs prod variants with fallback to prod if test is empty
            bool isTest = _config.IsTestDb;
            bool useLocal = _config.UseLocalDb;
            string localConnProd = _config.LocalMySqlConnectionString;
            string serverConnProd = _config.ServerMySqlConnectionString;
            string localConnTest = _config.LocalMySqlConnectionString_Test;
            string serverConnTest = _config.ServerMySqlConnectionString_Test;
            var localConn = isTest ? (string.IsNullOrWhiteSpace(localConnTest) ? localConnProd : localConnTest) : localConnProd;
            var serverConn = isTest ? (string.IsNullOrWhiteSpace(serverConnTest) ? serverConnProd : serverConnTest) : serverConnProd;

            if (useLocal && !string.IsNullOrWhiteSpace(localConn))
                return new MySqlConnection(localConn);
            if (!useLocal && !string.IsNullOrWhiteSpace(serverConn))
                return new MySqlConnection(serverConn);

            // Graceful fallback: if სასურველი მიზანი ცარიელია, სცადე მეორე
            if (useLocal && string.IsNullOrWhiteSpace(localConn) && !string.IsNullOrWhiteSpace(serverConn))
                return new MySqlConnection(serverConn);
            if (!useLocal && string.IsNullOrWhiteSpace(serverConn) && !string.IsNullOrWhiteSpace(localConn))
                return new MySqlConnection(localConn);

            // თუ ორივე ცარიელია, მივუთითოთ, რომ Settings-ში სრული connection string-ები არ არის დაყენებული
            throw new InvalidOperationException("No valid MySQL connection string configured in Settings (Local/Server, Test/Prod).");


        }

        public MySqlConnection GetLocalConnection()
        {
            bool isTest = false;//_config.IsTestDb;  // სატესტო ბაზაზე გადართვა დროებით გათიშულია
            string localConnProd = _config.LocalMySqlConnectionString;
            string localConnTest = _config.LocalMySqlConnectionString_Test;
            var localConn = isTest ? (string.IsNullOrWhiteSpace(localConnTest) ? localConnProd : localConnTest) : localConnProd;
            if (string.IsNullOrWhiteSpace(localConn))
                throw new Exception("LocalMySqlConnectionString is not configured in Settings.");
            return new MySqlConnection(localConn);
        }

        public MySqlConnection GetServerConnection()
        {
            bool isTest = _config.IsTestDb;
            string serverConnProd = _config.ServerMySqlConnectionString;
            string serverConnTest = _config.ServerMySqlConnectionString_Test;
            var serverConn = isTest ? (string.IsNullOrWhiteSpace(serverConnTest) ? serverConnProd : serverConnTest) : serverConnProd;
            if (string.IsNullOrWhiteSpace(serverConn))
                throw new Exception("ServerMySqlConnectionString is not configured in Settings.");

            return new MySqlConnection(serverConn);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var conn = GetMySqlConnection())
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool CanConnectToMySQL() => CheckLocalConnection().IsConnected;

        public bool CanConnectToServer() => CheckServerConnection().IsConnected;

        public ConnectionCheckResult CheckLocalConnection() =>
            TryOpenConnectionWithDetails(GetLocalConnection, "local");

        public ConnectionCheckResult CheckServerConnection() =>
            TryProbeConnection(GetServerConnection, "server");

        private static ConnectionCheckResult TryOpenConnectionWithDetails(
            Func<MySqlConnection> getConnection,
            string target)
        {
            try
            {
                using var conn = getConnection();
                conn.Open();
                return ConnectionCheckResult.Ok();
            }
            catch (Exception ex)
            {
                return ConnectionCheckResult.Failed(ConnectionFailureParser.Parse(ex, target));
            }
        }

        /// <summary>
        /// სერვერის ჯანმრთელობის შემოწმება — pooling გარეშე; timeout ცული/არასტაბილური ინტერნეტისთვის (15+10 წმ).
        /// იძახება ConnectionMonitor-იდან (~30 წმ) და სინქიდან (CheckServerConnection).
        /// შენიშვნა: MySql.Data SSL-ის შიდა timeout ზოგჯერ ცალკე thread-ზე ისროლებს exception-ს — იხ. SyncPeriodicTimerRunner / Program.UnhandledException.
        /// </summary>
        private static ConnectionCheckResult TryProbeConnection(
            Func<MySqlConnection> getConnection,
            string target)
        {
            try
            {
                using var baseConn = getConnection();
                var probeBuilder = new MySqlConnectionStringBuilder(baseConn.ConnectionString)
                {
                    ConnectionTimeout = 15,
                    Pooling = false,
                    DefaultCommandTimeout = 10
                };

                using var conn = new MySqlConnection(probeBuilder.ConnectionString);
                conn.Open();
                using var cmd = new MySqlCommand("SELECT 1", conn) { CommandTimeout = 10 };
                cmd.ExecuteScalar();
                return ConnectionCheckResult.Ok();
            }
            catch (Exception ex)
            {
                return ConnectionCheckResult.Failed(ConnectionFailureParser.Parse(ex, target));
            }
        }
    }
}

