using BCCStudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// SubGroups ცხრილთან სამუშაო ინტერფეისი
    /// ქვეჯგუფების მართვა
    /// </summary>
    public interface ISubGroupRepository
    {
        #region INSERT - ქვეჯგუფის ჩასმა

        /// <summary>
        /// ახალი ქვეჯგუფის დამატება
        /// </summary>
        int AddSubGroup(SubGroup subGroup);

        /// <summary>
        /// ახალი ქვეჯგუფის დამატება (მინიმალური პარამეტრებით)
        /// </summary>
        int AddSubGroup(string name, int groupId);

        /// <summary>
        /// ახალი ქვეჯგუფის დამატება (ფასით)
        /// </summary>
        int AddSubGroup(string name, int groupId, decimal tuitionFee);

        #endregion

        #region SELECT - ქვეჯგუფის წაკითხვა

        /// <summary>
        /// ქვეჯგუფის მიღება ID-ით
        /// </summary>
        SubGroup GetSubGroupById(int subGroupId);

        /// <summary>
        /// ქვეჯგუფის მიღება ნომრით და ჯგუფის ID-ით
        /// </summary>
        SubGroup GetSubGroupByNumber(int groupId, int subGroupNumber);

        /// <summary>
        /// ქვეჯგუფის მიღება სახელით და ჯგუფის ID-ით
        /// </summary>
        SubGroup GetSubGroupByName(int groupId, string name);

        /// <summary>
        /// ჯგუფის პირველი ქვეჯგუფის მიღება
        /// </summary>
        SubGroup GetFirstSubGroupByGroupId(int groupId);

        /// <summary>
        /// ყველა ქვეჯგუფის მიღება
        /// </summary>
        List<SubGroup> GetAllSubGroups();

        /// <summary>
        /// ყველა აქტიური ქვეჯგუფის მიღება
        /// </summary>
        List<SubGroup> GetAllActiveSubGroups();

        /// <summary>
        /// ქვეჯგუფების მიღება ჯგუფის ID-ით
        /// </summary>
        List<SubGroup> GetSubGroupsByGroupId(int groupId);

        /// <summary>
        /// ქვეჯგუფების მიღება DataTable-ად
        /// </summary>
        DataTable GetAllSubGroupsFor();

        /// <summary>
        /// მოსწავლის ქვეჯგუფების მიღება
        /// </summary>
        List<SubGroup> GetStudentSubGroupsByStudentId(int studentId);

        /// <summary>
        /// მოსწავლის მიმდინარე ქვეჯგუფის ID-ის მიღება
        /// </summary>
        int GetCurrentStudentSubGroupId(int studentId, int groupId, bool status);

        /// <summary>
        /// ქვეჯგუფში მოსწავლეთა რაოდენობა
        /// </summary>
        int GetStudentCountInSubGroup(int subGroupId);

        /// <summary>
        /// ქვეჯგუფის არსებობის შემოწმება
        /// </summary>
        bool SubGroupExists(int subGroupId);

        /// <summary>
        /// ქვეჯგუფის არსებობის შემოწმება სახელით
        /// </summary>
        bool SubGroupExistsByName(int groupId, string name);

        #endregion

        #region UPDATE - ქვეჯგუფის განახლება (სრული)

        /// <summary>
        /// ქვეჯგუფის სრული განახლება
        /// </summary>
        bool UpdateSubGroup(SubGroup subGroup);

        #endregion

        #region UPDATE - ცალკეული ველების განახლება

        /// <summary>
        /// ქვეჯგუფის სახელის განახლება
        /// </summary>
        bool UpdateSubGroupName(int subGroupId, string newName);

        /// <summary>
        /// ქვეჯგუფის ფასის განახლება
        /// </summary>
        bool UpdateSubGroupTuitionFee(int subGroupId, decimal newFee);

        /// <summary>
        /// ქვეჯგუფის სტატუსის განახლება
        /// </summary>
        bool UpdateSubGroupStatus(int subGroupId, bool status);

        /// <summary>
        /// ჯგუფის ყველა ქვეჯგუფის სტატუსის განახლება
        /// </summary>
        bool UpdateSubGroupsStatusByGroupId(int groupId, bool status);

        #endregion

        #region UPDATE - მოსწავლეთა რაოდენობის მართვა

        /// <summary>
        /// მოსწავლეთა რაოდენობის განახლება
        /// </summary>
        bool UpdateSubGroupStudentCount(int subGroupId, int count);

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა 1-ით
        /// </summary>
        bool IncrementSubGroupCount(int subGroupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null);

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება 1-ით
        /// </summary>
        bool DecreaseStudentCount(int subGroupId);

        #endregion

        #region UPDATE - StudentSubGroups ცხრილთან მუშაობა

        /// <summary>
        /// მოსწავლის ქვეჯგუფის განახლება
        /// </summary>
        bool UpdateStudentSubGroup(int studentId, int groupId, int newSubGroupId, int oldSubGroupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფის გადახდის სტატუსის განახლება
        /// </summary>
        bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status);

        /// <summary>
        /// მოსწავლის ქვეჯგუფის გადახდის თარიღის განახლება
        /// </summary>
        void UpdateStudentSubGroupPaymentDate(int studentId, int groupId, int subGroupId, DateTime paymentDate);

        /// <summary>
        /// მოსწავლის ქვეჯგუფის სტატუსის განახლება
        /// </summary>
        bool UpdateStudentSubGroupStatus(int groupId, int studentId, int subGroupId, bool status);

        #endregion

        #region DELETE - ქვეჯგუფის წაშლა

        /// <summary>
        /// ქვეჯგუფის წაშლა (Soft Delete)
        /// </summary>
        bool DeleteSubGroup(int subGroupId);

        /// <summary>
        /// ქვეჯგუფის სრული წაშლა (Hard Delete)
        /// </summary>
        bool HardDeleteSubGroup(int subGroupId);

        /// <summary>
        /// მოსწავლის ქვეჯგუფიდან ამოღება
        /// </summary>
        bool DeleteStudentFromSubGroup(int studentId, int groupId);

        #endregion
    }
}


