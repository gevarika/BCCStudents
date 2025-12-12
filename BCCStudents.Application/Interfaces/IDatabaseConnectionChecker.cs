namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// ინტერფეისი მონაცემთა ბაზასთან კავშირის შემოწმებისთვის
    /// Clean Architecture-ის დაცვით - Application Layer არ დამოკიდებულია Infrastructure Layer-ზე პირდაპირ
    /// </summary>
    public interface IDatabaseConnectionChecker
    {
        /// <summary>
        /// ამოწმებს კავშირს MySQL მონაცემთა ბაზასთან
        /// </summary>
        /// <returns>true თუ კავშირი დამყარებულია, false თუ არა</returns>
        bool CanConnectToMySQL();
    }
}

