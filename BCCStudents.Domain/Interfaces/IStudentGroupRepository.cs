using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// StudentGroups ცხრილთან სამუშაო ინტერფეისი
    /// მოსწავლის-ჯგუფის კავშირის მართვა
    /// </summary>
    public interface IStudentGroupRepository
    {
        #region INSERT - ჩანაწერის დამატება

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        /// <param name="studentGroup">StudentGroups ობიექტი</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        int InsertStudentGroup(StudentGroups studentGroup, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (მინიმალური პარამეტრებით)
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი ჩანაწერის ID</returns>
        int InsertStudentGroup(int studentId, int groupId, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        int InsertStudentGroupWithDiscount(int studentId, int groupId, double discount);

        #endregion

        #region SELECT - ჩანაწერის წაკითხვა

        /// <summary>
        /// ჩანაწერის მიღება ID-ით
        /// </summary>
        StudentGroups GetById(int id);

        /// <summary>
        /// ჩანაწერის მიღება StudentId და GroupId-ით
        /// </summary>
        StudentGroups GetByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ყველა ჯგუფის მიღება
        /// </summary>
        List<StudentGroups> GetByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფების მიღება
        /// </summary>
        List<StudentGroups> GetActiveByStudentId(int studentId);

        /// <summary>
        /// ჯგუფის ყველა მოსწავლის მიღება
        /// </summary>
        List<StudentGroups> GetByGroupId(int groupId);

        /// <summary>
        /// ჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        List<StudentGroups> GetActiveByGroupId(int groupId);

        /// <summary>
        /// მოსწავლის ჯგუფების მიღება გადასახდისთვის (Groups ცხრილთან JOIN)
        /// </summary>
        List<StudentGroups> GetForPayment(int studentId);

        /// <summary>
        /// მოსწავლის ჯგუფის ID-ების მიღება
        /// </summary>
        List<int> GetGroupIdsByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფის ID-ების მიღება
        /// </summary>
        List<int> GetActiveGroupIdsByStudentId(int studentId);

        /// <summary>
        /// გადაუხდელი ჯგუფების მიღება
        /// </summary>
        List<StudentGroups> GetUnpaidByStudentId(int studentId);

        /// <summary>
        /// ვადაგადაცილებული გადახდების მიღება (გადახდის თარიღი გავიდა)
        /// </summary>
        List<StudentGroups> GetOverduePayments(DateTime asOfDate);

        /// <summary>
        /// არსებობს თუ არა აქტიური კავშირი
        /// </summary>
        bool ExistsActive(int studentId, int groupId);

        /// <summary>
        /// არსებობს თუ არა კავშირი (აქტიური ან არააქტიური)
        /// </summary>
        bool ExistsAny(int studentId, int groupId);

        #endregion

        #region UPDATE - ჩანაწერის განახლება (სრული)

        /// <summary>
        /// ჩანაწერის სრული განახლება
        /// </summary>
        bool Update(StudentGroups studentGroup);

        #endregion

        #region UPDATE - ცალკეული ველების განახლება

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        bool UpdateStatus(int studentId, int groupId, bool status);

        /// <summary>
        /// გადახდის სტატუსის განახლება
        /// </summary>
        bool UpdatePaymentStatus(int studentId, int groupId, string paymentStatus);

        /// <summary>
        /// გადახდის თარიღის განახლება
        /// </summary>
        bool UpdateDateOfPayment(int studentId, int groupId, DateTime? dateOfPayment);

        /// <summary>
        /// გადახდის თარიღის განახლება (alias)
        /// </summary>
        bool UpdatePaymentDate(int studentId, int groupId, DateTime newDate);

        /// <summary>
        /// გადახდის სტატუსის და თარიღის ერთდროული განახლება
        /// </summary>
        bool UpdatePaymentStatusAndDate(int studentId, int groupId, string paymentStatus, DateTime? dateOfPayment);

        /// <summary>
        /// ფასდაკლების განახლება
        /// </summary>
        bool UpdateDiscount(int studentId, int groupId, double discount);

        /// <summary>
        /// ფასის განახლება
        /// </summary>
        bool UpdatePrice(int studentId, int groupId, decimal price);

        /// <summary>
        /// GroupId-ის განახლება (ჯგუფის შეცვლა)
        /// </summary>
        bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId);

        #endregion

        #region DELETE - ჩანაწერის წაშლა

        /// <summary>
        /// Soft Delete - სტატუსის შეცვლა
        /// </summary>
        bool SoftDelete(int studentId, int groupId);

        /// <summary>
        /// Hard Delete - სრული წაშლა
        /// </summary>
        bool HardDelete(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ყველა ჯგუფიდან Soft Delete
        /// </summary>
        bool SoftDeleteAllByStudentId(int studentId);

        #endregion

        #region IMPORT - იმპორტისთვის საჭირო მეთოდები

        /// <summary>
        /// ყველა აქტიური მოსწავლე-ჯგუფის წყვილის მიღება (იმპორტის დროს დუბლიკატების შესამოწმებლად)
        /// </summary>
        List<(int StudentId, int GroupId)> GetAllActiveStudentGroupPairs();

        #endregion
    }
}



