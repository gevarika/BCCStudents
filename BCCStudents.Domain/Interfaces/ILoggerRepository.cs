using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface ILoggerRepository
    {
        void WriteLog(string action, string status, string message, string user);
        void LogStudentAction(string action, string status, string message, string user);
        void LogPaymentAction(string action, string status, string message, string user);
        void LogGroupAction(string action, string status, string message, string user);
        void LogSMSAction(string action,string status, string message, string user);
        void LogImportAction(string action, string status, string message, string user);
    }
}


