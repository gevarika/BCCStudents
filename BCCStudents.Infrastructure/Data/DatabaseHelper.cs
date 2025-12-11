using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using BCCStudents.Domain.Interfaces;

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

            // Graceful fallback: if the preferred target is empty, try the other one
            if (useLocal && string.IsNullOrWhiteSpace(localConn) && !string.IsNullOrWhiteSpace(serverConn))
                return new MySqlConnection(serverConn);
            if (!useLocal && string.IsNullOrWhiteSpace(serverConn) && !string.IsNullOrWhiteSpace(localConn))
                return new MySqlConnection(localConn);

            // As a last resort, build from individual settings (ServerHost/DatabaseName/Username/Password)
            string built = BuildMySqlConnectionFromSettings(isTest);
            if (!string.IsNullOrWhiteSpace(built))
                return new MySqlConnection(built);

            // Nothing configured
            throw new InvalidOperationException("No valid MySQL connection string configured. Fill Settings (Local/Server and Test/Prod) or set App.config connectionStrings.");

            
        }

		public MySqlConnection GetLocalConnection()
		{
			bool isTest = _config.IsTestDb;
			string localConnProd = _config.LocalMySqlConnectionString;
			string localConnTest = _config.LocalMySqlConnectionString_Test;
			var localConn = isTest ? (string.IsNullOrWhiteSpace(localConnTest) ? localConnProd : localConnTest) : localConnProd;
			if (string.IsNullOrWhiteSpace(localConn))
				throw new Exception("LocalMySqlConnectionString is not configured in Settings.");
			return new MySqlConnection(localConn);
		}

		private string BuildMySqlConnectionFromSettings(bool isTest)
		{
			try
			{
				string host = _config.ServerHost;
				string database = _config.DatabaseName;
				string username = _config.Username;
				string password = _config.Password;
				string port = "3306"; // default if not provided elsewhere

				if (string.IsNullOrWhiteSpace(database))
					database = isTest ? "bccstudents_local_test" : "bccstudents_local";
				if (string.IsNullOrWhiteSpace(host))
					host = "127.0.0.1";

				// Allow empty username/password in case of local trust configurations, though MySQL typically requires credentials
				string connStr = $"Server={host};Port={port};Database={database};User Id={username};Password={password};SslMode=Preferred;AllowPublicKeyRetrieval=True;CharSet=utf8mb4;";
				return connStr;
			}
			catch
			{
				return string.Empty;
			}
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

		public bool CanConnectToMySQL()
        {
            try
            {
					using (var conn = GetLocalConnection())
					{
						conn.Open();
						return true;
					}
                
            }
            catch (Exception ex)
            {
                try
                {
                    var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
                    Directory.CreateDirectory(dir);
                    var path = Path.Combine(dir, "mysql-connection-errors.txt");
                    File.AppendAllText(path, $"[{DateTime.Now}] Connection error: {ex}\n\n");
                }
                catch { }
                return false;
            }
        }
    }
}

