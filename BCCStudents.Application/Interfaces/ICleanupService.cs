namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// გაწმენდის სერვისის ინტერფეისი - მონაცემების განულებისთვის
    /// </summary>
    public interface ICleanupService
    {
        /// <summary>
        /// აღადგენს ყველა მონაცემს (მხოლოდ ადმინისტრატორს შეუძლია)
        /// </summary>
        /// <exception cref="System.UnauthorizedAccessException">თუ მომხმარებელი არ არის ადმინისტრატორი</exception>
        void ResetAllData();
    }
}

