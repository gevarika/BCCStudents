using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IStudentRepository
    {
        #region ==================== INSERT - მოსწავლის ჩასმა ====================

        /// <summary>
        /// ახალი მოსწავლის ჩასმა (სრული ობიექტით)
        /// </summary>
        /// <param name="student">მოსწავლის ობიექტი</param>
        /// <param name="connection">არსებული კავშირი (ოფციონალური - ტრანზაქციისთვის)</param>
        /// <param name="transaction">არსებული ტრანზაქცია (ოფციონალური)</param>
        /// <returns>ახალი მოსწავლის ID</returns>
        int InsertStudent(Student student, MySqlConnection connection = null, MySqlTransaction transaction = null);

        /// <summary>
        /// ახალი მოსწავლის ჩასმა (მინიმალური მონაცემებით)
        /// </summary>
        int InsertStudentBasic(string firstName, string lastName, string phone);

        /// <summary>
        /// ახალი მოსწავლის ჩასმა (სახელი, გვარი, ტელეფონი, პირადი ნომერი და მშობლის სახელი)
        /// </summary>
        int InsertStudentWithDetails(string firstName, string lastName, string phone, long personalId, string parentName);

        #endregion

        #region ==================== SELECT - მოსწავლის წაკითხვა ====================

        /// <summary>
        /// მოსწავლის მიღება ID-ით
        /// </summary>
        Student GetStudentById(int studentId);

        /// <summary>
        /// მოსწავლის მიღება კოდით
        /// </summary>
        Student GetStudentByCode(string studentCode);

        /// <summary>
        /// მოსწავლის მიღება პირადი ნომრით
        /// </summary>
        Student GetStudentByPersonalId(long personalId);

        /// <summary>
        /// მოსწავლის მიღება სახელით და გვარით
        /// </summary>
        Student GetStudentByFullName(string firstName, string lastName);

        /// <summary>
        /// ყველა მოსწავლის მიღება
        /// </summary>
        List<Student> GetAllStudents();

        /// <summary>
        /// მხოლოდ აქტიური მოსწავლეების მიღება
        /// </summary>
        List<Student> GetAllActiveStudents();

        /// <summary>
        /// არააქტიური მოსწავლეების მიღება
        /// </summary>
        List<Student> GetAllInactiveStudents();

        /// <summary>
        /// წაშლილი მოსწავლეების მიღება (IsDeleted = 1)
        /// </summary>
        List<Student> GetAllDeletedStudents();

        /// <summary>
        /// მოსწავლის ID-ის მიღება კოდით
        /// </summary>
        int? GetStudentIdByCode(string studentCode);

        /// <summary>
        /// მოსწავლის სრული სახელის მიღება ID-ით
        /// </summary>
        string GetStudentFullNameById(int studentId);

        /// <summary>
        /// მოსწავლის კოდის მიღება ID-ით
        /// </summary>
        string GetStudentCodeById(int studentId);

        /// <summary>
        /// მოსწავლის ბალანსის მიღება ID-ით
        /// </summary>
        decimal GetStudentBalanceById(int studentId);

        /// <summary>
        /// მოსწავლეების მიღება DataTable-ად (ComboBox-ებისთვის)
        /// </summary>
        DataTable GetStudentsAsDataTable();

        /// <summary>
        /// Students ცხრილის ცარიელობის შემოწმება
        /// </summary>
        bool IsStudentsTableEmpty();

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება ID-ით
        /// </summary>
        bool StudentExists(int studentId);

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება კოდით
        /// </summary>
        bool StudentExistsByCode(string studentCode);

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება პირადი ნომრით
        /// </summary>
        bool StudentExistsByPersonalId(long personalId);

        /// <summary>
        /// მოსწავლეთა რაოდენობის მიღება
        /// </summary>
        int GetStudentsCount();

        /// <summary>
        /// აქტიური მოსწავლეთა რაოდენობის მიღება
        /// </summary>
        int GetActiveStudentsCount();

        #endregion

        #region ==================== UPDATE - მოსწავლის განახლება (სრული) ====================

        /// <summary>
        /// მოსწავლის სრული განახლება (ყველა ველი)
        /// </summary>
        bool UpdateStudent(Student student);

        /// <summary>
        /// მხოლოდ კონკრეტული ველების განახლება (დინამიური)
        /// </summary>
        bool UpdateStudentFields(int studentId, Dictionary<string, object> changedFields);

        #endregion

        #region ==================== UPDATE - ცალკეული ველების განახლება ====================

        /// <summary>
        /// მოსწავლის სახელის განახლება
        /// </summary>
        bool UpdateStudentFirstName(int studentId, string firstName);

        /// <summary>
        /// მოსწავლის გვარის განახლება
        /// </summary>
        bool UpdateStudentLastName(int studentId, string lastName);

        /// <summary>
        /// მოსწავლის ტელეფონის განახლება
        /// </summary>
        bool UpdateStudentPhone(int studentId, string phone);

        /// <summary>
        /// მოსწავლის მისამართის განახლება
        /// </summary>
        bool UpdateStudentAddress(int studentId, string address);

        /// <summary>
        /// მოსწავლის ასაკის განახლება
        /// </summary>
        bool UpdateStudentAge(int studentId, int age);

        /// <summary>
        /// მოსწავლის მშობლის სახელის განახლება
        /// </summary>
        bool UpdateStudentParentName(int studentId, string parentName);

        /// <summary>
        /// მოსწავლის პირადი ნომრის განახლება
        /// </summary>
        bool UpdateStudentPersonalId(int studentId, long personalId);

        /// <summary>
        /// მოსწავლის სტატუსის განახლება (Active/Inactive)
        /// </summary>
        bool UpdateStudentStatus(int studentId, string status);

        /// <summary>
        /// მოსწავლის კოდის განახლება
        /// </summary>
        bool UpdateStudentCode(int studentId, string studentCode);

        /// <summary>
        /// მოსწავლის ინფორმაციის (შენიშვნის) განახლება
        /// </summary>
        bool UpdateStudentInfo(int studentId, string info);

        /// <summary>
        /// მოსწავლის პირადობის ბარათის გზის განახლება
        /// </summary>
        bool UpdateStudentIdCardPath(int studentId, string path);

        /// <summary>
        /// მოსწავლის დამატებითი დოკუმენტების გზის განახლება
        /// </summary>
        bool UpdateStudentAdditionalDocsPath(int studentId, string path);

        #endregion

        #region ==================== UPDATE - ბალანსის მართვა ====================

        /// <summary>
        /// მოსწავლის ბალანსის განახლება (ახალი მნიშვნელობით)
        /// </summary>
        bool UpdateStudentBalance(int studentId, decimal balance);

        /// <summary>
        /// მოსწავლის ბალანსის გაზრდა
        /// </summary>
        bool IncrementStudentBalance(int studentId, decimal amount);

        /// <summary>
        /// მოსწავლის ბალანსის შემცირება
        /// </summary>
        bool DecrementStudentBalance(int studentId, decimal amount);

        #endregion

        #region ==================== DELETE - მოსწავლის წაშლა ====================

        /// <summary>
        /// მოსწავლის წაშლა (Soft Delete - IsDeleted = 1)
        /// </summary>
        bool DeleteStudent(int studentId);

        /// <summary>
        /// მოსწავლის აღდგენა (Soft Delete-დან - IsDeleted = 0)
        /// </summary>
        bool RestoreStudent(int studentId);

        /// <summary>
        /// მოსწავლის სრული წაშლა (Hard Delete - ჩანაწერის წაშლა)
        /// გამოიყენეთ ფრთხილად!
        /// </summary>
        bool HardDeleteStudent(int studentId);

        #endregion

        #region ==================== SEARCH & FILTER - ძებნა და ფილტრაცია ====================

        /// <summary>
        /// მოსწავლეების ფილტრაცია (სახელით, ჯგუფით, თარიღით)
        /// </summary>
        DataTable FilterStudents(string name, int? groupId, int? subGroupId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// მოსწავლეების ძებნა სახელით და გვარით ჯგუფში
        /// </summary>
        List<Student> SearchStudentsByNameAndGroup(string text, int groupId);

        /// <summary>
        /// მოსწავლეების ძებნა სახელით ყველა ჯგუფში
        /// </summary>
        List<Student> SearchStudentsByNameAcrossAllGroups(string name);

        /// <summary>
        /// მოსწავლეების ძებნა ველით და მნიშვნელობით (ოფციონალური ჯგუფით)
        /// </summary>
        List<Student> SearchStudents(string fieldName, string searchText, int? groupId = null);

        /// <summary>
        /// მოსწავლეების მიღება ჯგუფების ინფორმაციით
        /// </summary>
        List<Student> GetAllStudentsWithGroups();

        /// <summary>
        /// მოსწავლის სახელების სია (Id, FullName) - ComboBox-ისთვის
        /// </summary>
        List<(int Id, string FullName)> GetStudentNames();

        /// <summary>
        /// გაუნაწილებელი მოსწავლეები (ჯგუფის გარეშე)
        /// </summary>
        DataTable GetUnassignedStudents();

        /// <summary>
        /// მოსწავლეები DataTable-ად (ფორმისთვის)
        /// </summary>
        DataTable GetAllStudentsFor();

        /// <summary>
        /// მოსწავლეების მცირე ინფორმაცია (StudentViewDto)
        /// </summary>
        List<StudentViewDto> GetAllStudentsSomeInfo();

        /// <summary>
        /// მოსწავლეები ჯგუფის მიხედვით
        /// </summary>
        List<Student> GetStudentsByGroupId(int groupId);

        /// <summary>
        /// მოსწავლის დეტალები ID-ით და GroupId-ით
        /// </summary>
        Student GetStudentDetailsById(int studentId, int groupId);

        /// <summary>
        /// მოსწავლის სახელის მიღება
        /// </summary>
        string GetStudentName(int studentId);

        #endregion

        #region ==================== DUPLICATE CHECKS - დუბლიკატების შემოწმება ====================

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება სახელით და გვარით
        /// </summary>
        bool ExistsByName(string firstName, string lastName);

        /// <summary>
        /// მოსწავლის არსებობის შემოწმება სახელით, გვარით, მშობლით და მისამართით
        /// </summary>
        bool ExistsByNameParentAddress(string firstName, string lastName, string parentName, string address);

        /// <summary>
        /// მოსწავლეების რაოდენობა მისამართით
        /// </summary>
        int CountByAddress(string address);

        #endregion

        #region ==================== STUDENT GROUPS - StudentGroups ცხრილთან მუშაობა ====================

        /// <summary>
        /// მოსწავლე-ჯგუფის კავშირის არსებობა (აქტიური)
        /// </summary>
        bool StudentGroupExists(int studentId, int groupId);

        /// <summary>
        /// მოსწავლე-ჯგუფის სტატუსის განახლება
        /// </summary>
        bool UpdateStudentStatus(int studentId, int groupId, bool status);

        /// <summary>
        /// მოსწავლე-ჯგუფის ველების განახლება
        /// </summary>
        void UpdateStudentGroupFields(StudentGroups original, StudentGroups updated);

        /// <summary>
        /// მოსწავლის ჯგუფის ID-ის განახლება
        /// </summary>
        bool UpdateStudentGroupId(int studentId, int newGroupId);

        /// <summary>
        /// მოსწავლის ყველა ჯგუფიდან ამოღება (soft delete)
        /// </summary>
        void RemoveStudentFromGroups(int studentId);

        /// <summary>
        /// მოსწავლის კონკრეტული ჯგუფიდან ამოღება (soft delete)
        /// </summary>
        void RemoveStudentFromGroup(int studentId, int groupId);

        #endregion

        #region ==================== DELETE - მოსწავლის წაშლა (userId-ით) ====================

        /// <summary>
        /// მოსწავლის წაშლა userId-ით (Soft Delete - IsDeleted = 1)
        /// </summary>
        bool DeleteStudent(int studentId, int userId);

        #endregion

        #region ==================== MIGRATION - მიგრაცია ====================

        /// <summary>
        /// StudentGroups-ის მიგრაცია
        /// </summary>
        void MigrateStudentGroups();

        #endregion

        #region ==================== STATISTICS - სტატისტიკის მეთოდები ====================

        /// <summary>
        /// მოსწავლეთა რაოდენობა ჯგუფების მიხედვით
        /// </summary>
        DataTable GetStudentCountByGroup();

        /// <summary>
        /// მოსწავლეთა რეგისტრაცია თვეების მიხედვით
        /// </summary>
        DataTable GetStudentsByMonth();

        #endregion

        #region ==================== IMPORT - იმპორტისთვის საჭირო მეთოდები ====================

        /// <summary>
        /// მოსწავლის გასაღებების მიღება (იმპორტის დროს დუბლიკატების შესამოწმებლად)
        /// </summary>
        List<StudentKey> GetStudentsForImport();

        #endregion
    }
}


