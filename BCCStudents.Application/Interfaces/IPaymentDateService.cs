using BCCStudents.Application.Services;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// გადახდის თარიღის მართვის სერვისის ინტერფეისი
    /// </summary>
    public interface IPaymentDateService
    {
        // საწყისი თარიღის დაყენება
        DateTime CalculateInitialPaymentDate(DateTime registrationDate, int? dayOfMonth = null);
        void SetInitialPaymentDate(int studentId, int groupId, DateTime registrationDate, int? preferredPaymentDay = null);
        void SetInitialPaymentDateForSubGroup(int studentId, int groupId, int subGroupId, DateTime registrationDate, int? preferredPaymentDay = null);

        // გადახდის შემდეგ თარიღის განახლება
        DateTime CalculateNextPaymentDateAfterFullPayment(DateTime currentPaymentDate);
        void UpdatePaymentDateAfterFullPayment(int studentId, int groupId, DateTime currentPaymentDate);
        void UpdateSubGroupPaymentDateAfterFullPayment(int studentId, int groupId, int subGroupId, DateTime currentPaymentDate);

        // ზედმეტი ჩარიცხვის დამუშავება
        DateTime CalculatePaymentDateWithExcessCredit(DateTime currentPaymentDate, decimal excessAmount, decimal monthlyFee);
        decimal ProcessExcessPayment(int studentId, int groupId, DateTime currentPaymentDate, decimal totalAmount, decimal monthlyFee);

        // თვის დასაწყისის პროცესი
        void ProcessMonthlyPaymentReset();
        PaymentDateStatus GetPaymentStatus(int studentId, int groupId);

        // გადახდის თარიღის ხელით ცვლილება
        void UpdateNextPaymentDate(DateTime paymentDate);
        bool ManuallySetPaymentDate(int studentId, int groupId, DateTime newPaymentDate);
        bool ChangePaymentDayOfMonth(int studentId, int groupId, int newDayOfMonth);

        // დამხმარე მეთოდები
        DateTime? GetNextPaymentDate(int studentId, int groupId);
        List<StudentGroupPaymentInfo> GetAllPaymentDates(int studentId);
    }
}

