using BCCStudents.Domain.Entities;
using System.Data;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// ქვეჯგუფის სერვისის ინტერფეისი
    /// </summary>
    public interface ISubGroupService
    {
        // ქვეჯგუფების დამატება
        void AddSubGroups(SubGroup subGroup, int subGroupCount);
        int AddSubGroup(SubGroup subGroup);

        // სტუდენტების მართვა ქვეჯგუფებში
        void DeleteStudentFromSubGroup(int studentId, int groupId);
        void RemoveStudentFromAllSubGroups(int studentId, int groupId);
        void UpdateStudentSubGroup(int studentId, int groupId, int subGroupId, int oldSubGroupId);
        bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status);
        bool UpdateStudentSubGroupStatus(int groupId, int studentId, int subGroupId, bool status);
        void UpdateStudentSubGroupPaymentDate(int studentId, int groupId, int subGroupId, DateTime paymentDate);

        // ქვეჯგუფების მიღება
        List<SubGroup> GetStudentSubGroupsByStudentId(int studentId);
        List<SubGroup> GetSubGroupsByGroupId(int groupId);
        /// <summary>აქტიური და არააქტიური ქვეჯგუფები (რედაქტირების ფორმისთვის).</summary>
        List<SubGroup> GetAllSubGroupsByGroupId(int groupId);
        DataTable GetAllSubGroupsFor();
        List<SubGroup> GetAllSubGroups();
        SubGroup GetFirstSubGroupByGroupId(int groupId);
        SubGroup GetSubGroupById(int Id);
        SubGroup GetSubGroupByNumber(int groupId, int Id);

        // სტუდენტების რაოდენობა
        int GetCurrentStudentSubGroupId(int studentId, int groupId, bool status);
        int GetStudentCountInSubGroup(int subGroupId);
        //void UpdateSubGroupStudentCount(int subGroupId, int count);
        //void IncrementSubGroupCount(int subGroupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null);
        void DecreaseStudentCount(int SubGroupId);

        // ქვეჯგუფების განახლება და წაშლა
        bool UpdateSubGroup(SubGroup subGroup);
        bool DeleteSubGroup(int subGroupId);
        bool UpdateSubGroupsStatusByGroupId(int groupId, bool status);
        bool UpdateSubGroupsTuitionFeeByGroupId(int groupId, decimal newFee);
    }
}

