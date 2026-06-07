using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// StudentGroups სერვისის ინტერფეისი
    /// მოსწავლე-ჯგუფის კავშირების მართვა
    /// </summary>
    public interface IStudentGroupsService
    {
        #region INSERT - მოსწავლე-ჯგუფის დამატება

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        int AddStudentGroup(StudentGroups studentGroup, MySqlConnection Connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (ID-ებით)
        /// </summary>
        int AddStudentGroup(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        int AddStudentGroupWithDiscount(int studentId, int groupId, double discount);

        #endregion

        #region SELECT - მოსწავლე-ჯგუფის მიღება

        /// <summary>
        /// მოსწავლე-ჯგუფის მიღება ID-ით
        /// </summary>
        StudentGroups GetById(int id);

        /// <summary>
        /// მოსწავლე-ჯგუფის მიღება StudentId და GroupId-ით
        /// </summary>
        StudentGroups GetByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// ბოლო ჩანაწერი StudentId და GroupId-ით (ყველა სტატუსი)
        /// </summary>
        StudentGroups GetLatestByStudentAndGroup(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის ჯგუფების მიღება
        /// </summary>
        List<StudentGroups> GetByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფების მიღება
        /// </summary>
        List<StudentGroups> GetActiveByStudentId(int studentId);

        /// <summary>
        /// ჯგუფის მოსწავლეების მიღება
        /// </summary>
        List<StudentGroups> GetByGroupId(int groupId);

        /// <summary>
        /// ჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        List<StudentGroups> GetActiveByGroupId(int groupId);

        /// <summary>
        /// მოსწავლის ჯგუფების მიღება გადახდებისთვის (Groups ცხრილთან JOIN)
        /// </summary>
        List<StudentGroups> GetForPayment(int studentId);

        /// <summary>
        /// მოსწავლის ჯგუფების ID-ების მიღება
        /// </summary>
        List<int> GetGroupIdsByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფების ID-ების მიღება
        /// </summary>
        List<int> GetActiveGroupIdsByStudentId(int studentId);

        /// <summary>
        /// გადაუხდელი ჯგუფების მიღება
        /// </summary>
        List<StudentGroups> GetUnpaidByStudentId(int studentId);

        /// <summary>
        /// ვადაგასული გადახდების მიღება
        /// </summary>
        List<StudentGroups> GetOverduePayments(DateTime asOfDate);

        /// <summary>
        /// არსებობის შემოწმება (აქტიური)
        /// </summary>
        bool ExistsActive(int studentId, int groupId);

        /// <summary>
        /// არსებობის შემოწმება (ნებისმიერი)
        /// </summary>
        bool ExistsAny(int studentId, int groupId);

        /// <summary>
        /// ყველა აქტიური მოსწავლე-ჯგუფის წყვილების მიღება (იმპორტისთვის)
        /// </summary>
        List<(int StudentId, int GroupId)> GetAllActiveStudentGroupPairs();

        #endregion

        #region UPDATE - მოსწავლე-ჯგუფის განახლება

        /// <summary>
        /// მოსწავლე-ჯგუფის სრული განახლება
        /// </summary>
        bool Update(StudentGroups studentGroup);

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        bool UpdateStatus(int studentId, int groupId, bool status);

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        bool UpdateStudentStatus(int studentId, int groupId, bool status);

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
        /// გადახდის სტატუსისა და თარიღის ერთად განახლება
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

        #region DELETE - მოსწავლე-ჯგუფის წაშლა

        /// <summary>
        /// Soft Delete - მოსწავლის ჯგუფიდან ამოღება
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

        #region ARCHIVE - მოსწავლის არქივირება

        /// <summary>
        /// მოსწავლის ჯგუფიდან არქივირება (სტატუსის განახლება და ჯგუფის მოსწავლეთა რაოდენობის შემცირება)
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        void ArchiveStudentFromGroup(int studentId, int groupId);

        #endregion
    }
}

