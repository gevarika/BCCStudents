using BCCStudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Interfaces
{
    public interface IPendingStudentService
    {
        List<PendingStudent> GetAllPending();
        void Delete(int id);
        ApprovalResult ApproveStudent(int pendingStudent, decimal discountAmount);
        void Update(PendingStudent pendingStudent);
        void UpdatePartial(int id, Dictionary<string, object> fields);
    }
}


