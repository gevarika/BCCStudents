using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// StudentSubGroups ცხრილთან სამუშაო ინტერფეისი
    /// მოსწავლის-ქვეჯგუფის კავშირის მართვა
    /// </summary>
    public interface IStudentSubGroupRepository
    {
        #region INSERT - ჩანაწერის დამატება

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        /// <param name="studentSubGroup">StudentSubGroups ობიექტი</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        int InsertStudentSubGroup(StudentSubGroups studentSubGroup, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (მინიმალური პარამეტრებით)
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="subGroupId">ქვეჯგუფის ID</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        int InsertStudentSubGroup(int studentId, int groupId, int subGroupId, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        int InsertStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount);

        #endregion

        #region SELECT - ჩანაწერის წაკითხვა

        /// <summary>
        /// ჩანაწერის მიღება ID-ით
        /// </summary>
        StudentSubGroups GetById(int id);

        /// <summary>
        /// ჩანაწერის მიღება StudentId, GroupId და SubGroupId-ით
        /// </summary>
        StudentSubGroups GetByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// ბოლო ჩანაწერი StudentId, GroupId და SubGroupId-ით (ყველა სტატუსი)
        /// </summary>
        StudentSubGroups GetLatestByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// მოსწავლის ჩანაწერის მიღება ჯგუფისთვის
        /// </summary>
        StudentSubGroups GetByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ყველა ქვეჯგუფის მიღება
        /// </summary>
        List<StudentSubGroups> GetByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის აქტიური ქვეჯგუფების მიღება
        /// </summary>
        List<StudentSubGroups> GetActiveByStudentId(int studentId);

        /// <summary>
        /// ქვეჯგუფის ყველა მოსწავლის მიღება
        /// </summary>
        List<StudentSubGroups> GetBySubGroupId(int subGroupId);

        /// <summary>
        /// ქვეჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        List<StudentSubGroups> GetActiveBySubGroupId(int subGroupId);

        /// <summary>
        /// მოსწავლის აქტიური SubGroupId-ის მიღება ჯგუფისთვის
        /// </summary>
        int? GetActiveSubGroupId(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფის ID-ების მიღება
        /// </summary>
        List<int> GetSubGroupIdsByStudentId(int studentId);

        /// <summary>
        /// არსებობს თუ არა აქტიური კავშირი
        /// </summary>
        bool ExistsActive(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// არსებობს თუ არა კავშირი (აქტიური ან არააქტიური)
        /// </summary>
        bool ExistsAny(int studentId, int groupId, int subGroupId);

        /// <summary>
        /// არსებობს თუ არა კავშირი ჯგუფისთვის (ნებისმიერი ქვეჯგუფი)
        /// </summary>
        bool ExistsAnyForGroup(int studentId, int groupId);

        #endregion

        #region UPDATE - ჩანაწერის განახლება (სრული)

        /// <summary>
        /// ჩანაწერის სრული განახლება
        /// </summary>
        bool Update(StudentSubGroups studentSubGroup);

        #endregion

        #region UPDATE - ცალკეული ველების განახლება

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
        /// გადახდის თარიღის განახლება (alias მეთოდი PaymentDateService-ისთვის)
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

        #region DELETE - ჩანაწერის წაშლა

        /// <summary>
        /// Soft Delete - სტატუსის შეცვლა
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



