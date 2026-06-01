using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IPendingStudentGroupRepository
    {
        List<int> GetGroupsForPendingStudent(int pendingStudentId);
        List<PendingStudentSubGroup> GetPendingSubGroupsByStudentId(int studentId);
        List<int> GetPendingGroupLinkIds(int pendingStudentId);
        List<int> GetPendingSubGroupLinkIds(int pendingStudentId);
        void DeleteByPendingStudentId(int pendingStudentId);
        void DeleteSubGroupsByPendingStudentId(int pendingStudentId);
    }
}

