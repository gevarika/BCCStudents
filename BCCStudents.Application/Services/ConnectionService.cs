using BCCStudents.Infrastructure.Data;
using BCCStudents.Application.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly DatabaseHelper _dbHelper;

        public ConnectionService(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /*public SQLiteConnection GetConnection()
        {
            return _dbHelper.GetSQLiteConnection();
        }*/

        public MySqlConnection GetLocalConnection()
        {
            return _dbHelper.GetLocalConnection();
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


