using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Infrastructure.Repositories
{

    public class LoggerRepository : ILoggerRepository
    {
        private static readonly string BaseLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");

        public LoggerRepository()
        {
            if (!Directory.Exists(BaseLogPath))
            {
                Directory.CreateDirectory(BaseLogPath);
            }
        }
        /// <summary>
        /// წერს ლოგ ჩანაწერს ფაილში.
        /// </summary>
        /// <param name="operationType">ოპერაციის ტიპი (მაგ. Insert, Delete, Update)</param>
        /// <param name="status">ოპერაციის სტატუსი (Success, Failed)</param>
        /// <param name="details">დეტალები (შინაარსი, რაც ჩაიწერება ლოგში)</param>
        /// <param name="user">მომხმარებლის სახელი (სურვილისამებრ)</param>

        public void WriteLog(string operationType, string status, string details, string user = "System")
        {
            WriteToFile("operation_log.txt", operationType, status, details, user);
        }

        public void LogStudentAction(string action, string status, string details, string user = "System")
        {
            WriteToFile("students_log.txt", action, status, details, user);
        }

        public void LogGroupAction(string action, string status, string details, string user = "System")
        {
            WriteToFile("groups_log.txt", action, status, details, user);
        }

        public void LogPaymentAction(string action, string status, string details, string user = "System")
        {
            WriteToFile("payments_log.txt", action, status, details, user);
        }

        public void LogSMSAction(string action, string status, string details, string user = "System")
        {
            WriteToFile("sms_log.txt", action, status, details, user);
        }
        public void LogImportAction(string action, string status, string details, string user = "System")
        {
            WriteToFile("Import_log.txt", action, status, details, user);
        }
        private void WriteToFile(string fileName, string operationType, string status, string details, string user)
        {
            try
            {
                string filePath = Path.Combine(BaseLogPath, fileName);
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Operation: {operationType} | User: {user} | Status: {status}\nDetails:\n{details}\n\n";
                File.AppendAllText(filePath, logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
    }

}


