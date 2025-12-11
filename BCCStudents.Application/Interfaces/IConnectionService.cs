using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// კავშირის სერვისის ინტერფეისი - მონაცემთა ბაზასთან კავშირის მართვისთვის
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// აბრუნებს ლოკალურ მონაცემთა ბაზასთან კავშირს
        /// </summary>
        MySqlConnection GetLocalConnection();

        /// <summary>
        /// ამოწმებს კავშირს მონაცემთა ბაზასთან
        /// </summary>
        /// <param name="message">შეტყობინება კავშირის შედეგის შესახებ</param>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        bool CheckConnection(out string message);
    }
}

