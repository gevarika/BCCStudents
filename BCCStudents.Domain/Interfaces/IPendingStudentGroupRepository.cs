using BCCStudents.Domain.Entities;

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


