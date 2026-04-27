using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace BCCStudents.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IStudentGroupsService _studentGroupsService;
        //private readonly IStudentService _studentService;
        private readonly ISubGroupService _subGroupService;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        public GroupService(IDatabaseConnectionProvider connectionProvider, IGroupRepository groupRepository, IStudentGroupsService studentGroupsService, ISubGroupService subGroupService, IUpStreamChangeTracker upStreamChangeTracker)
        {
            _groupRepository = groupRepository;
            _studentGroupsService = studentGroupsService;
            //_studentService = studentService;
            _subGroupService = subGroupService;
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));
        }

        #region ==================== INSERT - ჯგუფის დამატება ====================

        /// <summary>
        /// ახალი ჯგუფის დამატება (სრული ობიექტით)
        /// </summary>
        public int? AddGroup(Group group)
        {
            var id = _groupRepository.InsertGroup(group);
            if (id > 0)
            {
                SyncGroupSnapshot(id, SyncOperationType.Insert);
            }
            return id;
        }

        /// <summary>
        /// ახალი ჯგუფის დამატება (მხოლოდ სახელით)
        /// </summary>
        public int? AddGroupByName(string name)
        {
            var id = _groupRepository.InsertGroupByName(name);
            if (id > 0)
            {
                SyncGroupSnapshot(id, SyncOperationType.Insert);
            }
            return id;
        }

        /// <summary>
        /// ახალი ჯგუფის დამატება (სახელი და ფასი)
        /// </summary>
        public int? AddGroupWithPrice(string name, decimal price)
        {
            var id = _groupRepository.InsertGroupWithPrice(name, price);
            if (id > 0)
            {
                SyncGroupSnapshot(id, SyncOperationType.Insert);
            }
            return id;
        }

        /// <summary>
        /// ახალი ჯგუფის დამატება (სახელი, ფასი და მასწავლებელი)
        /// </summary>
        public int? AddGroupWithTeacher(string name, decimal price, string teacher)
        {
            var id = _groupRepository.InsertGroupWithTeacher(name, price, teacher);
            if (id > 0)
            {
                SyncGroupSnapshot(id, SyncOperationType.Insert);
            }
            return id;
        }

        #endregion
        #region ==================== SELECT - ჯგუფის მიღება ====================

        /// <summary>
        /// ჯგუფის მიღება ID-ით
        /// </summary>
        public Group GetGroupById(int groupId)
        {
            return _groupRepository.GetGroupById(groupId);
        }

        /// <summary>
        /// ჯგუფის მიღება სახელით
        /// </summary>
        public Group GetGroupByName(string name)
        {
            return _groupRepository.GetGroupByName(name);
        }

        /// <summary>
        /// ყველა ჯგუფის მიღება
        /// </summary>
        public List<Group> GetAllGroups()
        {
            return _groupRepository.GetAllGroups();
        }

        /// <summary>
        /// მხოლოდ აქტიური ჯგუფების მიღება
        /// </summary>
        public List<Group> GetAllActiveGroups()
        {
            return _groupRepository.GetAllActiveGroups();
        }

        /// <summary>
        /// ჯგუფების მიღება ID-ების სიით
        /// </summary>
        public List<Group> GetGroupsByIds(List<int> ids)
        {
            return _groupRepository.GetGroupsByIds(ids);
        }

        /// <summary>
        /// ჯგუფის სახელის მიღება ID-ით
        /// </summary>
        public string GetGroupName(int groupId)
        {
            return _groupRepository.GetGroupNameById(groupId);
        }

        /// <summary>
        /// ჯგუფის ფასის მიღება ID-ით
        /// </summary>
        public decimal GetGroupPrice(int groupId)
        {
            return _groupRepository.GetGroupPrice(groupId);
        }

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება ID-ით
        /// </summary>
        public int GetGroupStudentCount(int groupId)
        {
            return _groupRepository.GetGroupStudentCountById(groupId);
        }

        /// <summary>
        /// ჯგუფის მოსწავლეთა რაოდენობის მიღება სახელით
        /// </summary>
        public int GetGroupStudentCountByName(string name)
        {
            return _groupRepository.GetGroupStudentCountByName(name);
        }

        /// <summary>
        /// ჯგუფების მიღება DataTable-ად (ComboBox-ებისთვის)
        /// </summary>
        public DataTable GetGroupsAsDataTable()
        {
            return _groupRepository.GetGroupsAsDataTable();
        }

        /// <summary>
        /// ჯგუფების ჩატვირთვა Dictionary-ში
        /// </summary>
        public void LoadGroupsToDictionary(Dictionary<string, List<int>> groupIds)
        {
            _groupRepository.LoadGroupsToDictionary(groupIds);
        }

        /// <summary>
        /// Groups ცხრილის ცარიელობის შემოწმება
        /// </summary>
        public bool IsGroupsTableEmpty()
        {
            return _groupRepository.IsGroupsTableEmpty();
        }

        #endregion
        #region ==================== UPDATE - ჯგუფის ცალკეული ველების განახლება ====================

        /// <summary>
        /// ჯგუფის სახელის განახლება
        /// </summary>
        public bool UpdateGroupName(int groupId, string newName)
        {
            var result = _groupRepository.UpdateGroupName(groupId, newName);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        /// <summary>
        /// ჯგუფის ფასის განახლება
        /// </summary>
        public bool UpdateGroupPrice(int groupId, decimal newPrice)
        {
            var result = _groupRepository.UpdateGroupPrice(groupId, newPrice);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        /// <summary>
        /// ჯგუფის მასწავლებლის განახლება
        /// </summary>
        public bool UpdateGroupTeacher(int groupId, string newTeacher)
        {
            var result = _groupRepository.UpdateGroupTeacher(groupId, newTeacher);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        /// <summary>
        /// ჯგუფის სტატუსის განახლება
        /// </summary>
        public bool UpdateGroupStatus(int groupId, bool newStatus)
        {
            var result = _groupRepository.UpdateGroupStatus(groupId, newStatus);
            if (result)
            {
                // ქვეჯგუფების სტატუსიც განვაახლოთ
                //_subGroupService.UpdateSubGroupsStatusByGroupId(groupId, newStatus);
                SyncGroupSnapshot(groupId, SyncOperationType.Update);
            }
            return result;
        }

        /// <summary>
        /// ჯგუფის კონტრაქტის შაბლონის პათის განახლება
        /// </summary>
        public bool UpdateGroupContractTemplatePath(int groupId, string newPath)
        {
            var result = _groupRepository.UpdateGroupContractTemplatePath(groupId, newPath);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        /// <summary>
        /// ჯგუფის მაქსიმალური მოსწავლეების რაოდენობის განახლება
        /// </summary>
        public bool UpdateGroupMaxStudents(int groupId, int newMaxStudents)
        {
            var result = _groupRepository.UpdateGroupMaxStudents(groupId, newMaxStudents);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        #endregion
        #region ==================== UPDATE - ჯგუფის განახლება (სრული) ====================

        /// <summary>
        /// ჯგუფის სრული განახლება
        /// </summary>
        public bool UpdateGroup(Group group)
        {
            try
            {
                // შევამოწმოთ სტატუსი შეიცვალა თუ არა
                var currentGroup = _groupRepository.GetGroupById(group.Id);
                bool statusChanged = currentGroup != null && currentGroup.Status != group.Status;

                // განვაახლოთ ჯგუფი
                var result = _groupRepository.UpdateGroup(group);

                // თუ სტატუსი შეიცვალა, განვაახლოთ ქვეჯგუფებიც
                if (statusChanged)
                {
                    _subGroupService.UpdateSubGroupsStatusByGroupId(group.Id, group.Status);
                }

                SyncGroupSnapshot(group.Id, SyncOperationType.Update);
                return result;
            }
            catch
            {
                return false;
            }
        }

        #endregion
        #region ==================== UPDATE - მოსწავლეთა რაოდენობის მართვა ====================

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა 1-ით
        /// </summary>
        public bool IncrementGroupStudentCount(int groupId)
        {
            var result = _groupRepository.IncrementStudentCount(groupId);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება 1-ით
        /// </summary>
        public bool DecrementGroupStudentCount(int groupId)
        {
            var result = _groupRepository.DecrementStudentCount(groupId);
            if (result) SyncGroupSnapshot(groupId, SyncOperationType.Update);
            return result;
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის გაზრდა N-ით (ციკლით)
        /// </summary>
        public void IncrementGroupStudentCountBy(int groupId, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _groupRepository.IncrementStudentCount(groupId);
            }
            SyncGroupSnapshot(groupId, SyncOperationType.Update);
        }

        /// <summary>
        /// მოსწავლეთა რაოდენობის შემცირება N-ით (ციკლით)
        /// </summary>
        public void DecrementGroupStudentCountBy(int groupId, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _groupRepository.DecrementStudentCount(groupId);
            }
            SyncGroupSnapshot(groupId, SyncOperationType.Update);
        }

        public bool RecalculateStudentCount(int groupId, MySqlConnection externalConnection, MySqlTransaction externalTransaction)
        {
            // ბიზნეს ლოგიკა (ვალიდაცია, ნებართვები) აქ დაემატება.
            // შემდეგ, დაუყოვნებლივ გადაეცემა Repository-ს:
            return _groupRepository.RecalculateStudentCount(groupId, externalConnection, externalTransaction);
        }
        #endregion

        #region ==================== VALIDATION - მოსწავლეების რაოდენობის ვალიდაცია ====================

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ჯგუფში მოსწავლის დამატება
        /// </summary>
        public bool CanAddStudentToGroup(int groupId)
        {
            var group = _groupRepository.GetGroupById(groupId);
            if (group == null)
                return false;

            // თუ MaxStudents = 0, ნიშნავს რომ შეზღუდვა არ არის
            if (group.MaxStudents == 0)
                return true;

            return group.StudentCount < group.MaxStudents;
        }

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ქვეჯგუფში მოსწავლის დამატება
        /// </summary>
        public bool CanAddStudentToSubGroup(int subGroupId)
        {
            var subGroup = _subGroupService.GetSubGroupById(subGroupId);
            if (subGroup == null)
                return false;

            // თუ MaxStudents = 0, ნიშნავს რომ შეზღუდვა არ არის
            if (subGroup.MaxStudents == 0)
                return true;

            return subGroup.StudentCount < subGroup.MaxStudents;
        }

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ჯგუფში MaxStudents-ის შეცვლა
        /// </summary>
        public bool CanUpdateGroupMaxStudents(int groupId, int newMaxStudents)
        {
            var group = _groupRepository.GetGroupById(groupId);
            if (group == null)
                return false;

            // თუ newMaxStudents = 0, ნიშნავს რომ შეზღუდვა არ არის - ყოველთვის შეიძლება
            if (newMaxStudents == 0)
                return true;

            // ახალი მაქსიმუმი არ უნდა იყოს ნაკლები არსებულ მოსწავლეების რაოდენობაზე
            return newMaxStudents >= group.StudentCount;
        }

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ქვეჯგუფში MaxStudents-ის შეცვლა
        /// </summary>
        public bool CanUpdateSubGroupMaxStudents(int subGroupId, int newMaxStudents)
        {
            var subGroup = _subGroupService.GetSubGroupById(subGroupId);
            if (subGroup == null)
                return false;

            // თუ newMaxStudents = 0, ნიშნავს რომ შეზღუდვა არ არის - ყოველთვის შეიძლება
            if (newMaxStudents == 0)
                return true;

            // ახალი მაქსიმუმი არ უნდა იყოს ნაკლები არსებულ მოსწავლეების რაოდენობაზე
            return newMaxStudents >= subGroup.StudentCount;
        }

        /// <summary>
        /// შეამოწმებს შეიძლება თუ არა ჯგუფის ქვეჯგუფების მოსწავლეების საერთო რაოდენობა გადააჭარბოს ჯგუფის MaxStudents-ს
        /// </summary>
        public bool CanGroupAccommodateTotalStudents(int groupId, int newTotalStudents)
        {
            var group = _groupRepository.GetGroupById(groupId);
            if (group == null)
                return false;

            // თუ MaxStudents = 0, ნიშნავს რომ შეზღუდვა არ არის
            if (group.MaxStudents == 0)
                return true;

            return newTotalStudents <= group.MaxStudents;
        }

        #endregion

        #region ==================== DELETE - ჯგუფის წაშლა ====================

        /// <summary>
        /// ჯგუფის წაშლა (Soft Delete)
        /// </summary>
        public bool DeleteGroup(int groupId)
        {
            var snapshot = _groupRepository.GetGroupById(groupId);
            var result = _groupRepository.DeleteGroup(groupId);
            if (result && snapshot != null)
            {
                _upStreamChangeTracker.TrackGroupChange(groupId, SyncOperationType.Delete, snapshot);
            }
            return result;
        }

        /// <summary>
        /// ჯგუფის სრული წაშლა (Hard Delete)
        /// </summary>
        public bool HardDeleteGroup(int groupId)
        {
            var snapshot = _groupRepository.GetGroupById(groupId);
            var result = _groupRepository.HardDeleteGroup(groupId);
            if (result && snapshot != null)
            {
                _upStreamChangeTracker.TrackGroupChange(groupId, SyncOperationType.Delete, snapshot);
            }
            return result;
        }

        #endregion
        #region ==================== Sync Helpers ====================

        /// <summary>
        /// ჯგუფის სინქრონიზაცია სერვერთან
        /// </summary>
        private void SyncGroupSnapshot(int groupId, SyncOperationType operation)
        {
            try
            {
                var group = _groupRepository.GetGroupById(groupId);
                if (group != null)
                {
                    _upStreamChangeTracker.TrackGroupChange(groupId, operation, group);
                }
            }
            catch
            {
                // სინქრონიზაციის შეცდომა არ უნდა შეაჩეროს მთავარი ოპერაცია
            }
        }

        #endregion
    }
}




