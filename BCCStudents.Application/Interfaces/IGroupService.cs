using BCCStudents.Domain.Entities;
using MySql.Data.MySqlClient;
using System.Data;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// ჯგუფის სერვისის ინტერფეისი
    /// </summary>
    public interface IGroupService
    {
        #region INSERT - ჯგუფის დამატება

        /// <summary>
        /// ახალი ჯგუფის დამატება (სრული ობიექტით)
        /// </summary>
        int? AddGroup(Group group);

        /// <summary>
        /// ახალი ჯგუფის დამატება (მხოლოდ სახელით)
        /// </summary>
        int? AddGroupByName(string name);

        /// <summary>
        /// ახალი ჯგუფის დამატება (სახელი და ფასი)
        /// </summary>
        int? AddGroupWithPrice(string name, decimal price);

        /// <summary>
        /// ახალი ჯგუფის დამატება (სახელი, ფასი და მასწავლებელი)
        /// </summary>
        int? AddGroupWithTeacher(string name, decimal price, string teacher);

        #endregion

        #region SELECT - ჯგუფის მიღება

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
        string GetGroupName(int groupId);

        /// <summary>
        /// ჯგუფის ფასის მიღება ID-ით
        /// </summary>
        decimal GetGroupPrice(int groupId);

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება ID-ით
        /// </summary>
        int GetGroupStudentCount(int groupId);

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება სახელით
        /// </summary>
        int GetGroupStudentCountByName(string name);

        /// <summary>
        /// ჯგუფების მიღება DataTable-ად (ComboBox-ებისთვის)
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

        #region UPDATE - ჯგუფის განახლება

        /// <summary>
        /// ჯგუფის სრული განახლება
        /// </summary>
        bool UpdateGroup(Group group);

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
        bool IncrementGroupStudentCount(int groupId);

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება 1-ით
        /// </summary>
        bool DecrementGroupStudentCount(int groupId);

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა N-ით (ციკლით)
        /// </summary>
        void IncrementGroupStudentCountBy(int groupId, int count);

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება N-ით (ციკლით)
        /// </summary>
        void DecrementGroupStudentCountBy(int groupId, int count);

        bool RecalculateStudentCount(int groupId, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null);
        #endregion

        #region VALIDATION - მოსწავლეების რაოდენობის ვალიდაცია

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ჯგუფში მოსწავლის დამატება
        /// </summary>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <returns>true, თუ შეიძლება დამატება; false, თუ ჯგუფი სავსეა</returns>
        bool CanAddStudentToGroup(int groupId);

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ქვეჯგუფში მოსწავლის დამატება
        /// </summary>
        /// <param name="subGroupId">ქვეჯგუფის ID</param>
        /// <returns>true, თუ შეიძლება დამატება; false, თუ ქვეჯგუფი სავსეა</returns>
        bool CanAddStudentToSubGroup(int subGroupId);

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ჯგუფში MaxStudents-ის შეცვლა
        /// (შეამოწმებს რომ ახალი მაქსიმუმი არ იყოს ნაკლები არსებულ მოსწავლეების რაოდენობაზე)
        /// </summary>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="newMaxStudents">ახალი მაქსიმალური რაოდენობა</param>
        /// <returns>true, თუ შეიძლება შეცვლა; false, თუ ახალი მაქსიმუმი ნაკლებია არსებულ რაოდენობაზე</returns>
        bool CanUpdateGroupMaxStudents(int groupId, int newMaxStudents);

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ქვეჯგუფში MaxStudents-ის შეცვლა
        /// </summary>
        /// <param name="subGroupId">ქვეჯგუფის ID</param>
        /// <param name="newMaxStudents">ახალი მაქსიმალური რაოდენობა</param>
        /// <returns>true, თუ შეიძლება შეცვლა; false, თუ ახალი მაქსიმუმი ნაკლებია არსებულ რაოდენობაზე</returns>
        bool CanUpdateSubGroupMaxStudents(int subGroupId, int newMaxStudents);

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ჯგუფის ქვეჯგუფების მოსწავლეების საერთო რაოდენობა გადააჭარბოს ჯგუფის MaxStudents-ს
        /// </summary>
        /// <param name="groupId">ჯგუფის ID</param>
        /// <param name="newTotalStudents">ქვეჯგუფების მოსწავლეების საერთო რაოდენობა</param>
        /// <returns>true, თუ შეიძლება; false, თუ გადააჭარბებს</returns>
        bool CanGroupAccommodateTotalStudents(int groupId, int newTotalStudents);

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
