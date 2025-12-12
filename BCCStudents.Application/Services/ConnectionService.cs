using BCCStudents.Application.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public ConnectionService(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new System.ArgumentNullException(nameof(connectionProvider));
        }

        /*public SQLiteConnection GetConnection()
        {
            return _dbHelper.GetSQLiteConnection();
        }*/

        public MySqlConnection GetLocalConnection()
        {
            return _connectionProvider.GetLocalConnection();
        }

        public bool CheckConnection(out string message)
        {
            try
            {
                using (var connection = GetLocalConnection())
                {
                    connection.Open();
                    message = "დაკავშირებულია მონაცემთა ბაზასთან";
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                message = "მონაცემთა ბაზასთან კავშირი შეწყდა: " + ex.Message;
                return false;
            }
        }
    }
}


