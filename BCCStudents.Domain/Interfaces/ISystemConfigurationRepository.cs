using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// სისტემური კონფიგურაციის Repository Interface
    /// </summary>
    public interface ISystemConfigurationRepository
    {
        /// <summary>
        /// ყველა კონფიგურაციის მიღება
        /// </summary>
        List<SystemConfiguration> GetAll();

        /// <summary>
        /// კონფიგურაციის მიღება Key-ის მიხედვით
        /// </summary>
        SystemConfiguration GetByKey(string key);

        /// <summary>
        /// კონფიგურაციის დამატება ან განახლება (Upsert)
        /// </summary>
        void Upsert(SystemConfiguration configuration);

        /// <summary>
        /// კონფიგურაციის წაშლა
        /// </summary>
        void Delete(string key);

        /// <summary>
        /// კონფიგურაციის არსებობის შემოწმება
        /// </summary>
        bool Exists(string key);

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
    }
}
