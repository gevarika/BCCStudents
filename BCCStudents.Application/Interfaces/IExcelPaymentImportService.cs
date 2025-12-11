using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IExcelPaymentImportService
    {
        Task<ImportResult> ImportPaymentsFromExcel(string filePath, Action<int, int> progressCallback = null);
        Task<ImportResult> ImportSelectedPayments(string filePath, IEnumerable<DataRow> selectedRows, Action<int, int> progressCallback = null);
        IEnumerable<FailedPayment> GetFailedPayments();
    }
}
