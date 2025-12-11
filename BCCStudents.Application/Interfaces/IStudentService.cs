using System;
using System.Collections.Generic;
using System.Data;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// სტუდენტის სერვისის ინტერფეისი
    /// </summary>
    public interface IStudentService
    {
        // მიგრაცია
        void MigrateStudentGroups();

        // სტუდენტების მიღება
        DataTable GetUnassignedStudents();
        DataTable GetAllStudentsFor();
        List<StudentViewDto> GetAllStudentsSomeInfo();
        List<Student> GetAllStudents();
        DataTable FilterStudents(string name, int? groupId, int? subGroupId, DateTime? startDate, DateTime? endDate);
        List<(int Id, string FullName)> GetStudentNames();
        string GetStudentName(int studentId);
        List<Student> GetStudentsByGroupId(int groupId);
        List<Student> SearchStudentsByNameAndGroup(string text, int groupId);
        List<Student> GetAllStudentsWithGroups();
        List<Student> SearchStudentsByNameAcrossAllGroups(string name);
        List<Student> SearchStudents(string fieldName, string searchText, int? groupId = null);
        Student GetStudentDetailsById(int studentId, int groupId);

        // დუბლიკატების შემოწმება
        bool ExistsStudentByName(string firstName, string lastName);
        bool ExistsStudentByNameParentAddress(string firstName, string lastName, string parentName, string address);
        int CountStudentsByAddress(string address);

        // სტუდენტების დამატება, განახლება, წაშლა
        bool AddStudent(Student student, List<int> groupIds, int userId, bool printContract, OperationResultContext result, out int studentId);
        void DeleteStudent(int studentId, int userId);
        void UpdateStudent(Student student);
        void UpdateStudentFields(int studentId, Dictionary<string, object> changedFields);
        bool UpdateStudentStatus(int studentId, int groupId, bool status);

        // ჯგუფებთან მუშაობა
        void UpdateStudentGroupFields(StudentGroups original, StudentGroups updated);
        void AddStudentToGroup(int studentId, int groupId, bool status = true, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null);
        void AddStudentToGroup(int studentId, int groupId, bool status, DateTime? dateOfPayment, string paymentStatus, decimal price, double discount);
        bool IsStudentInGroup(int studentId, int groupId);
        int? UpdateStudentGroupId(int studentId, int newGroupId);
        void RemoveStudentFromGroup(int studentId, int groupId);

        // ქვეჯგუფებთან მუშაობა
        void AddStudentToSubGroup(int studentId, int groupId, int subGroupId, string paymentStatus, DateTime? dateOfPayment, decimal price, double discount, bool status);
        void UpdateStudentSubGroupId(int studentId, int newGroupId, int newSubGroupId);

        // გამოთვლები
        decimal CalculateFinalFee(decimal baseFee, decimal discountPercentage);

        // JSON ოპერაციები
        List<Student> LoadStudentsFromJson();
        void SaveStudentsToJson(List<Student> students);
        void DeleteStudentFromJson(List<Student> students, int index);
    }
}

