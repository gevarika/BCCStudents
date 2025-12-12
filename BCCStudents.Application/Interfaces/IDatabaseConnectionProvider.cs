using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// ინტერფეისი მონაცემთა ბაზასთან კავშირის მისაღებად
    /// Clean Architecture-ის დაცვით - Application Layer არ დამოკიდებულია Infrastructure Layer-ზე პირდაპირ
    /// </summary>
    public interface IDatabaseConnectionProvider
    {
        /// <summary>
        /// აბრუნებს ლოკალურ მონაცემთა ბაზასთან კავშირს
        /// </summary>
        /// <returns>MySqlConnection ინსტანსი</returns>
        MySqlConnection GetLocalConnection();

        /// <summary>
        /// აბრუნებს სერვერზე მონაცემთა ბაზასთან კავშირს
        /// </summary>
        /// <returns>MySqlConnection ინსტანსი</returns>
        MySqlConnection GetServerConnection();

        /// <summary>
        /// აბრუნებს მონაცემთა ბაზასთან კავშირს (ლოკალური ან სერვერი, კონფიგურაციის მიხედვით)
        /// </summary>
        /// <returns>MySqlConnection ინსტანსი</returns>
        MySqlConnection GetMySqlConnection();
    }
}

