using BCCStudents.Application.Interfaces;
using BCCStudents.Infrastructure.Data;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// მონაცემთა ბაზასთან კავშირის მისაღებად სერვისი
    /// იმპლემენტირებს IDatabaseConnectionProvider ინტერფეისს
    /// იყენებს DatabaseHelper-ს Infrastructure Layer-ში
    /// </summary>
    public class DatabaseConnectionProvider : IDatabaseConnectionProvider
    {
        private readonly DatabaseHelper _databaseHelper;

        /// <summary>
        /// კონსტრუქტორი - იღებს DatabaseHelper-ს Dependency Injection-ით
        /// </summary>
        /// <param name="databaseHelper">DatabaseHelper ინსტანსი კავშირის მისაღებად</param>
        public DatabaseConnectionProvider(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        }

        /// <summary>Deprecated alias — server-only; იგივე GetServerConnection().</summary>
        public MySqlConnection GetLocalConnection() => _databaseHelper.GetLocalConnection();

        public MySqlConnection GetServerConnection() => _databaseHelper.GetServerConnection();

        public MySqlConnection GetMySqlConnection() => _databaseHelper.GetMySqlConnection();
    }
}

