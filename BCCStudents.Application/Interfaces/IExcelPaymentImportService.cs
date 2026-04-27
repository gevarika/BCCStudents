using BCCStudents.Domain.Entities;
using System.Data;

namespace BCCStudents.Domain.Interfaces
{
    public interface IExcelPaymentImportService
    {
        Task<ImportResult> ImportPaymentsFromExcel(string filePath, Action<int, int> progressCallback = null);
        Task<ImportResult> ImportSelectedPayments(string filePath, IEnumerable<DataRow> selectedRows, Action<int, int> progressCallback = null);
        IEnumerable<FailedPayment> GetFailedPayments();
    }
}
