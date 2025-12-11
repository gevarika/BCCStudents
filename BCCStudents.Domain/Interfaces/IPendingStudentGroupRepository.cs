using BCCStudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCCStudents.Domain.Interfaces
{
    public interface IPendingStudentGroupRepository
    {
        //void AddGroupForPendingStudent(int pendingStudentId, int groupId);
        List<int> GetGroupsForPendingStudent(int pendingStudentId);
        List<PendingStudentSubGroup> GetPendingSubGroupsByStudentId(int studentId);
        void DeleteByPendingStudentId(int pendingStudentId);
    }
}


