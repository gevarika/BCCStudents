using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using ClosedXML.Excel;

namespace BCCStudents.Domain.Interfaces
{
    public interface IImportService
    {
        //void ImportStudents(string excelFilePath, Dictionary<string, List<int>> groupIds, Action<int, int> progressCallback);
        //int ImportStudentsFromList(List<Student> students, bool isActive = true, IProgress<(int current, int total)> progress = null);
        Task<BCCStudents.Domain.Entities.ImportResult> ImportStudentsAsync(
            string filePath, 
            Dictionary<string, Dictionary<string, int>> columnMappings, 
            Dictionary<string, int> sheetToGroupIdMap,
            bool isActive, 
            IProgress<(int current, int total, string worksheet)> progress,
            Action<string> externalLogMessage = null);
        List<Payment> GetFailedPayments();
        void DeleteFailedPayment(DateTime paymentDate, decimal amount, long? personalId, string description);
        bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status);
    }
}


