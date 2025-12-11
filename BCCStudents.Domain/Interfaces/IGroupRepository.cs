using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IGroupRepository
    {
        #region INSERT - ჯგუფის ჩასმა

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (სრული ობიექტით)
        /// </summary>
        int InsertGroup(Group group);

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (მხოლოდ სახელით)
        /// </summary>
        int InsertGroupByName(string name);

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (სახელი და ფასი)
        /// </summary>
        int InsertGroupWithPrice(string name, decimal price);

        /// <summary>
        /// ახალი ჯგუფის ჩასმა (სახელი, ფასი და მასწავლებელი)
        /// </summary>
        int InsertGroupWithTeacher(string name, decimal price, string teacher);

        #endregion

        #region READ - ჯგუფის წაკითხვა

        /// <summary>
        /// ჯგუფის მიღება ID-ით
        /// </summary>
        Group GetGroupById(int groupId);

        /// <summary>
        /// ჯგუფის მიღება სახელით
        /// </summary>
        Group GetGroupByName(string name);

        /// <summary>
        /// ყველა ჯგუფის მიღება
        /// </summary>
        List<Group> GetAllGroups();

        /// <summary>
        /// მხოლოდ აქტიური ჯგუფების მიღება
        /// </summary>
        List<Group> GetAllActiveGroups();

        /// <summary>
        /// ჯგუფების მიღება ID-ების სიით
        /// </summary>
        List<Group> GetGroupsByIds(List<int> ids);

        /// <summary>
        /// ჯგუფის სახელის მიღება ID-ით
        /// </summary>
        string GetGroupNameById(int groupId);

        /// <summary>
        /// ჯგუფის ფასის მიღება ID-ით
        /// </summary>
        decimal GetGroupPriceById(int groupId);

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება ID-ით
        /// </summary>
        int GetGroupStudentCountById(int groupId);

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება სახელით
        /// </summary>
        int GetGroupStudentCountByName(string name);

        /// <summary>
        /// ჯგუფების მიღება DataTable-ად
        /// </summary>
        DataTable GetGroupsAsDataTable();

        /// <summary>
        /// ჯგუფების ჩატვირთვა Dictionary-ში
        /// </summary>
        void LoadGroupsToDictionary(Dictionary<string, List<int>> groupIds);

        /// <summary>
        /// Groups ცხრილის ცარიელობის შემოწმება
        /// </summary>
        bool IsGroupsTableEmpty();

        #endregion

        #region UPDATE - ჯგუფის განახლება (სრული)

        /// <summary>
        /// ჯგუფის სრული განახლება
        /// </summary>
        bool UpdateGroup(Group group);

        #endregion

        #region UPDATE - ჯგუფის ცალკეული ველების განახლება

        /// <summary>
        /// ჯგუფის სახელის განახლება
        /// </summary>
        bool UpdateGroupName(int groupId, string newName);

        /// <summary>
        /// ჯგუფის ფასის განახლება
        /// </summary>
        bool UpdateGroupPrice(int groupId, decimal newPrice);

        /// <summary>
        /// ჯგუფის მასწავლებლის განახლება
        /// </summary>
        bool UpdateGroupTeacher(int groupId, string newTeacher);

        /// <summary>
        /// ჯგუფის სტატუსის განახლება
        /// </summary>
        bool UpdateGroupStatus(int groupId, bool newStatus);

        /// <summary>
        /// ჯგუფის კონტრაქტის შაბლონის პათის განახლება
        /// </summary>
        bool UpdateGroupContractTemplatePath(int groupId, string newPath);

        /// <summary>
        /// ჯგუფის მაქსიმალური მოსწავლეების რაოდენობის განახლება
        /// </summary>
        bool UpdateGroupMaxStudents(int groupId, int newMaxStudents);

        #endregion

        #region UPDATE - მოსწავლეთა რაოდენობის მართვა

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა 1-ით
        /// </summary>
        bool IncrementStudentCount(int groupId);

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება 1-ით
        /// </summary>
        bool DecrementStudentCount(int groupId);

        /// <summary>
        /// მოსწავლეთა რაოდენობის ხელახალი გამოთვლა
        /// </summary>
        bool RecalculateStudentCount(int groupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null);

        #endregion

        #region DELETE - ჯგუფის წაშლა

        /// <summary>
        /// ჯგუფის წაშლა (Soft Delete)
        /// </summary>
        bool DeleteGroup(int groupId);

        /// <summary>
        /// ჯგუფის სრული წაშლა (Hard Delete)
        /// </summary>
        bool HardDeleteGroup(int groupId);

        #endregion
    }
}


