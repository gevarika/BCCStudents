using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        bool InsertPayment(Payment payment);
        void AddPayment(int studentId, decimal amount);
        Task<bool> AddPayments(IEnumerable<Payment> payments);
        
        bool IsGroupPaidForPeriod(int studentId, int groupId, DateTime paymentDate);
        List<PaymentSummary> GetPendingPayments();
        List<PaymentSummary> GetSuccessfulPayments();
        List<int> FindStudentsByDescription(string description);
        Task<List<Payment>> GetCurrentMonthPayments(int studentId);
        List<Payment> GetStudentPayments(int studentId);
        bool PaymentExists(DateTime paymentDate, decimal amount, long? personalId, string description);
        List<PaymentSummary> GetPaymentSummaries();
        IEnumerable<SuccessfulPayment> GetImportHistory();
        // ვერ წარმატებული გადახდების მართვა
        int SaveFailedPayment(FailedPayment payment);
        IEnumerable<FailedPayment> GetFailedPayments();
        FailedPayment GetFailedPaymentById(int id);
        void ClearFailedPayments();
        void DeleteFailedPayment(DateTime paymentDate, decimal amount, long? personalId, string description);
        // ყველა გადახდის მიღება
        List<Payment> GetAllPayments();
        bool CheckImportedPaymentDuplicate(DateTime paymentDate, decimal amount, long? personalId, string description);
        int AddImportedPaymentLog(DateTime paymentDate, decimal amount, long? personalId, string description, string importSource);
        ImportedPaymentLog GetImportedPaymentLogById(int id);
        bool CheckFailedPaymentDuplicate(DateTime paymentDate, decimal amount, long? personalId, string description);
    }
}



