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

        /// <summary>
        /// აბრუნებს ლოკალურ მონაცემთა ბაზასთან კავშირს
        /// </summary>
        /// <returns>MySqlConnection ინსტანსი</returns>
        public MySqlConnection GetLocalConnection()
        {
            return _databaseHelper.GetLocalConnection();
        }

        /// <summary>
        /// აბრუნებს სერვერზე მონაცემთა ბაზასთან კავშირს
        /// </summary>
        /// <returns>MySqlConnection ინსტანსი</returns>
        public MySqlConnection GetServerConnection()
        {
            return _databaseHelper.GetServerConnection();
        }

        /// <summary>
        /// აბრუნებს მონაცემთა ბაზასთან კავშირს (ლოკალური ან სერვერი - კონფიგურაციის მიხედვით)
        /// </summary>
        /// <returns>MySqlConnection ინსტანსი</returns>
        public MySqlConnection GetMySqlConnection()
        {
            return _databaseHelper.GetMySqlConnection();
        }
    }
}

