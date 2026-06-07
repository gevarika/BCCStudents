using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// ინტერფეისი მონაცემთა ბაზასთან კავშირის შემოწმებისთვის
    /// Clean Architecture-ის დაცვით - Application Layer არ დამოკიდებულია Infrastructure Layer-ზე პირდაპირ
    /// </summary>
    public interface IDatabaseConnectionChecker
    {
        /// <summary>
        /// ამოწმებს კავშირს ლოკალურ MySQL მონაცემთა ბაზასთან
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        bool CanConnectToMySQL();

        /// <summary>
        /// ამოწმებს კავშირს სერვერის MySQL მონაცემთა ბაზასთან
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        bool CanConnectToServer();

        /// <summary>
        /// სერვერთან კავშირის შემოწმება დეტალური შეცდომით (ლოგისთვის).
        /// </summary>
        ConnectionCheckResult CheckServerConnection();

        /// <summary>
        /// ლოკალურ ბაზასთან კავშირის შემოწმება დეტალური შეცდომით.
        /// </summary>
        ConnectionCheckResult CheckLocalConnection();
    }
}

