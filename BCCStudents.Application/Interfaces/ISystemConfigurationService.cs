namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სისტემური კონფიგურაციის Service Interface
    /// </summary>
    public interface ISystemConfigurationService
    {
        /// <summary>
        /// სწავლის დაწყების თარიღის მიღება
        /// </summary>
        DateTime? GetStudyStartDate();

        /// <summary>
        /// სწავლის დაწყების თარიღის შენახვა
        /// </summary>
        void SetStudyStartDate(DateTime date);

        /// <summary>
        /// ნაგულისხმევი გადახდის თარიღის მიღება
        /// </summary>
        DateTime? GetDefaultPaymentDate();

        /// <summary>
        /// ნაგულისხმევი გადახდის თარიღის შენახვა
        /// </summary>
        void SetDefaultPaymentDate(DateTime date);

        /// <summary>
        /// დასვენების პერიოდების მიღება
        /// </summary>
        List<(DateTime StartDate, DateTime EndDate)> GetVacationPeriods();

        /// <summary>
        /// დასვენების პერიოდის დამატება
        /// </summary>
        void AddVacationPeriod(DateTime startDate, DateTime endDate, string description = null);

        /// <summary>
        /// დასვენების პერიოდის წაშლა
        /// </summary>
        void DeleteVacationPeriod(DateTime startDate, DateTime endDate);

        /// <summary>
        /// შეამოწმებს, არის თუ არა თარიღი დასვენების პერიოდში
        /// </summary>
        bool IsVacationDate(DateTime date);

        /// <summary>
        /// შეამოწმებს, არის თუ არა თარიღი დასვენების პერიოდში (დიაპაზონში)
        /// </summary>
        bool IsDateInVacationPeriod(DateTime date);
    }
}
