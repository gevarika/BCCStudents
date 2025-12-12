using BCCStudents.Application.Interfaces;
using BCCStudents.Infrastructure.Data;

namespace BCCStudents.Infrastructure.Services
{
    /// <summary>
    /// მონაცემთა ბაზასთან კავშირის შემოწმების სერვისი
    /// იმპლემენტირებს IDatabaseConnectionChecker ინტერფეისს
    /// იყენებს DatabaseHelper-ს Infrastructure Layer-ში
    /// </summary>
    public class DatabaseConnectionChecker : IDatabaseConnectionChecker
    {
        private readonly DatabaseHelper _databaseHelper;

        /// <summary>
        /// კონსტრუქტორი - იღებს DatabaseHelper-ს Dependency Injection-ით
        /// </summary>
        /// <param name="databaseHelper">DatabaseHelper ინსტანსი კავშირის შესამოწმებლად</param>
        public DatabaseConnectionChecker(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new System.ArgumentNullException(nameof(databaseHelper));
        }

        /// <summary>
        /// ამოწმებს კავშირს MySQL მონაცემთა ბაზასთან
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        public bool CanConnectToMySQL()
        {
            return _databaseHelper.CanConnectToMySQL();
        }
    }
}

