using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// StudentSubGroups სერვისის ინტერფეისი
    /// მოსწავლე-ქვეჯგუფის კავშირების მართვა
    /// </summary>
    public interface IStudentSubGroupsService
    {
        #region INSERT - მოსწავლე-ქვეჯგუფის დამატება

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        int AddStudentSubGroup(StudentSubGroups studentSubGroup);

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (ID-ებით)
        /// </summary>
        int AddStudentSubGroup(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        int AddStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount);

        #endregion

        #region SELECT - მოსწავლე-ქვეჯგუფის მიღება

        /// <summary>
        /// მოსწავლე-ქვეჯგუფის მიღება ID-ით
        /// </summary>
        StudentSubGroups GetById(int id);

        /// <summary>
        /// მოსწავლე-ქვეჯგუფის მიღება StudentId, GroupId და SubGroupId-ით
        /// </summary>
        StudentSubGroups GetByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფის მიღება ჯგუფის მიხედვით
        /// </summary>
        StudentSubGroups GetByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფების მიღება
        /// </summary>
        List<StudentSubGroups> GetByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის აქტიური ქვეჯგუფების მიღება
        /// </summary>
        List<StudentSubGroups> GetActiveByStudentId(int studentId);

        /// <summary>
        /// ქვეჯგუფის მოსწავლეების მიღება
        /// </summary>
        List<StudentSubGroups> GetBySubGroupId(int subGroupId);

        /// <summary>
        /// ქვეჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        List<StudentSubGroups> GetActiveBySubGroupId(int subGroupId);

        /// <summary>
        /// მოსწავლის აქტიური SubGroupId-ის მიღება ჯგუფის მიხედვით
        /// </summary>
        int? GetActiveSubGroupId(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფების ID-ების მიღება
        /// </summary>
        List<int> GetSubGroupIdsByStudentId(int studentId);

        /// <summary>
        /// არსებობის შემოწმება (აქტიური)
        /// </summary>
        bool ExistsActive(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// არსებობის შემოწმება (ნებისმიერი)
        /// </summary>
        bool ExistsAny(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// არსებობის შემოწმება ჯგუფის მიხედვით (ნებისმიერი ქვეჯგუფი)
        /// </summary>
        bool ExistsAnyForGroup(int studentId, int groupId);

        #endregion

        #region UPDATE - მოსწავლე-ქვეჯგუფის განახლება

        /// <summary>
        /// მოსწავლე-ქვეჯგუფის სრული განახლება
        /// </summary>
        bool Update(StudentSubGroups studentSubGroup);

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        bool UpdateStatus(int studentId, int groupId, int subGroupId, bool status);

        /// <summary>
        /// გადახდის სტატუსის განახლება
        /// </summary>
        bool UpdatePaymentStatus(int studentId, int groupId, int subGroupId, string paymentStatus);

        /// <summary>
        /// გადახდის თარიღის განახლება
        /// </summary>
        bool UpdateDateOfPayment(int studentId, int groupId, int subGroupId, DateTime? dateOfPayment);

        /// <summary>
        /// გადახდის თარიღის განახლება (alias)
        /// </summary>
        bool UpdatePaymentDate(int studentId, int groupId, int subGroupId, DateTime newDate);

        /// <summary>
        /// ფასდაკლების განახლება
        /// </summary>
        bool UpdateDiscount(int studentId, int groupId, int subGroupId, double discount);

        /// <summary>
        /// SubGroupId-ის განახლება (ქვეჯგუფის შეცვლა)
        /// </summary>
        bool UpdateSubGroupId(int studentId, int groupId, int oldSubGroupId, int newSubGroupId);

        /// <summary>
        /// GroupId-ის განახლება (ჯგუფის შეცვლა)
        /// </summary>
        bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId);

        #endregion

        #region DELETE - მოსწავლე-ქვეჯგუფის წაშლა

        /// <summary>
        /// Soft Delete - მოსწავლის ქვეჯგუფიდან ამოღება
        /// </summary>
        bool SoftDelete(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// Hard Delete - სრული წაშლა
        /// </summary>
        bool HardDelete(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// მოსწავლის ჯგუფის ყველა ქვეჯგუფიდან Soft Delete
        /// </summary>
        bool SoftDeleteAllByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ყველა ქვეჯგუფიდან Soft Delete
        /// </summary>
        bool SoftDeleteAllByStudentId(int studentId);

        #endregion
    }
}

