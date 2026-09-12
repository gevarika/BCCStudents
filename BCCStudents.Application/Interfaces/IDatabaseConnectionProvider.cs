using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// მონაცემთა ბაზასთან კავშირი. Server-only რეჟიმი: ყველა getter სერვერის MySQL-ს უბრუნებს.
    /// </summary>
    public interface IDatabaseConnectionProvider
    {
        /// <summary>
        /// Deprecated alias — იგივე რაც GetServerConnection() (server-only).
        /// </summary>
        MySqlConnection GetLocalConnection();

        /// <summary>
        /// სერვერის MySQL კავშირი (ერთადერთი სამუშაო ბაზა).
        /// </summary>
        MySqlConnection GetServerConnection();

        /// <summary>
        /// სერვერის MySQL კავშირი (GetServerConnection-ის იგივე).
        /// </summary>
        MySqlConnection GetMySqlConnection();
    }
}
