using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// StudentGroups სერვისი
    /// მოსწავლე-ჯგუფის კავშირების მართვა
    /// </summary>
    public class StudentGroupsService : IStudentGroupsService
    {
        private readonly IStudentGroupRepository _repository;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;
        private readonly IGroupRepository _groupRepository;

        public StudentGroupsService(IStudentGroupRepository repository, IUpStreamChangeTracker upStreamChangeTracker, IGroupRepository groupRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _upStreamChangeTracker = upStreamChangeTracker;
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
        }

        #region INSERT - მოსწავლე-ჯგუფის დამატება

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        public int AddStudentGroup(StudentGroups studentGroup, MySqlConnection connection = null, MySqlTransaction transaction = null)
        {
            return _repository.InsertStudentGroup(studentGroup, connection, transaction);
        }

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (ID-ებით)
        /// </summary>
        public int AddStudentGroup(int studentId, int groupId)
        {
            return _repository.InsertStudentGroup(studentId, groupId);
        }

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        public int AddStudentGroupWithDiscount(int studentId, int groupId, double discount)
        {
            return _repository.InsertStudentGroupWithDiscount(studentId, groupId, discount);
        }

        #endregion

        #region SELECT - მოსწავლე-ჯგუფის მიღება

        /// <summary>
        /// მოსწავლე-ჯგუფის მიღება ID-ით
        /// </summary>
        public StudentGroups GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// მოსწავლე-ჯგუფის მიღება StudentId და GroupId-ით
        /// </summary>
        public StudentGroups GetByStudentAndGroup(int studentId, int groupId)
        {
            return _repository.GetByStudentAndGroup(studentId, groupId);
        }

        /// <summary>
        /// მოსწავლის ჯგუფების მიღება
        /// </summary>
        public List<StudentGroups> GetByStudentId(int studentId)
        {
            return _repository.GetByStudentId(studentId);
        }

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფების მიღება
        /// </summary>
        public List<StudentGroups> GetActiveByStudentId(int studentId)
        {
            return _repository.GetActiveByStudentId(studentId);
        }

        /// <summary>
        /// ჯგუფის მოსწავლეების მიღება
        /// </summary>
        public List<StudentGroups> GetByGroupId(int groupId)
        {
            return _repository.GetByGroupId(groupId);
        }

        /// <summary>
        /// ჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        public List<StudentGroups> GetActiveByGroupId(int groupId)
        {
            return _repository.GetActiveByGroupId(groupId);
        }

        /// <summary>
        /// მოსწავლის ჯგუფების მიღება გადახდებისთვის (Groups ცხრილთან JOIN)
        /// </summary>
        public List<StudentGroups> GetForPayment(int studentId)
        {
            return _repository.GetForPayment(studentId);
        }

        /// <summary>
        /// მოსწავლის ჯგუფების ID-ების მიღება
        /// </summary>
        public List<int> GetGroupIdsByStudentId(int studentId)
        {
            return _repository.GetGroupIdsByStudentId(studentId);
        }

        /// <summary>
        /// მოსწავლის აქტიური ჯგუფების ID-ების მიღება
        /// </summary>
        public List<int> GetActiveGroupIdsByStudentId(int studentId)
        {
            return _repository.GetActiveGroupIdsByStudentId(studentId);
        }

        /// <summary>
        /// გადაუხდელი ჯგუფების მიღება
        /// </summary>
        public List<StudentGroups> GetUnpaidByStudentId(int studentId)
        {
            return _repository.GetUnpaidByStudentId(studentId);
        }

        /// <summary>
        /// ვადაგასული გადახდების მიღება
        /// </summary>
        public List<StudentGroups> GetOverduePayments(DateTime asOfDate)
        {
            return _repository.GetOverduePayments(asOfDate);
        }

        /// <summary>
        /// არსებობის შემოწმება (აქტიური)
        /// </summary>
        public bool ExistsActive(int studentId, int groupId)
        {
            return _repository.ExistsActive(studentId, groupId);
        }

        /// <summary>
        /// არსებობის შემოწმება (ნებისმიერი)
        /// </summary>
        public bool ExistsAny(int studentId, int groupId)
        {
            return _repository.ExistsAny(studentId, groupId);
        }

        /// <summary>
        /// ყველა აქტიური მოსწავლე-ჯგუფის წყვილების მიღება (იმპორტისთვის)
        /// </summary>
        public List<(int StudentId, int GroupId)> GetAllActiveStudentGroupPairs()
        {
            return _repository.GetAllActiveStudentGroupPairs();
        }

        #endregion

        #region UPDATE - მოსწავლე-ჯგუფის განახლება

        /// <summary>
        /// მოსწავლე-ჯგუფის სრული განახლება
        /// </summary>
        public bool Update(StudentGroups studentGroup)
        {
            var ok = _repository.Update(studentGroup);
            if (ok)
            {
                TrySyncStudentGroup(studentGroup.StudentId, studentGroup.GroupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        public bool UpdateStatus(int studentId, int groupId, bool status)
        {
            var ok = _repository.UpdateStatus(studentId, groupId, status);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// Updates the status of a student-group link (e.g., to 'გადატანილი').
        /// </summary>
        public bool UpdateStudentStatus(int studentId, int groupId, bool status)
        {
            return UpdateStatus(studentId, groupId, status);
        }
        /// <summary>
        /// გადახდის სტატუსის განახლება
        /// </summary>
        public bool UpdatePaymentStatus(int studentId, int groupId, string paymentStatus)
        {
            var ok = _repository.UpdatePaymentStatus(studentId, groupId, paymentStatus);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// გადახდის თარიღის განახლება
        /// </summary>
        public bool UpdateDateOfPayment(int studentId, int groupId, DateTime? dateOfPayment)
        {
            var ok = _repository.UpdateDateOfPayment(studentId, groupId, dateOfPayment);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// გადახდის თარიღის განახლება (alias)
        /// </summary>
        public bool UpdatePaymentDate(int studentId, int groupId, DateTime newDate)
        {
            var ok = _repository.UpdatePaymentDate(studentId, groupId, newDate);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// გადახდის სტატუსისა და თარიღის ერთად განახლება
        /// </summary>
        public bool UpdatePaymentStatusAndDate(int studentId, int groupId, string paymentStatus, DateTime? dateOfPayment)
        {
            var ok = _repository.UpdatePaymentStatusAndDate(studentId, groupId, paymentStatus, dateOfPayment);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// ფასდაკლების განახლება
        /// </summary>
        public bool UpdateDiscount(int studentId, int groupId, double discount)
        {
            var ok = _repository.UpdateDiscount(studentId, groupId, discount);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// ფასის განახლება
        /// </summary>
        public bool UpdatePrice(int studentId, int groupId, decimal price)
        {
            var ok = _repository.UpdatePrice(studentId, groupId, price);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// GroupId-ის განახლება (ჯგუფის შეცვლა)
        /// </summary>
        public bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId)
        {
            var ok = _repository.UpdateGroupId(studentId, oldGroupId, newGroupId);
            if (ok)
            {
                TrySyncStudentGroup(studentId, oldGroupId, SyncOperationType.Update);
                TrySyncStudentGroup(studentId, newGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        #endregion

        #region DELETE - მოსწავლე-ჯგუფის წაშლა

        /// <summary>
        /// Soft Delete - მოსწავლის ჯგუფიდან ამოღება
        /// </summary>
        public bool SoftDelete(int studentId, int groupId)
        {
            var ok = _repository.SoftDelete(studentId, groupId);
            if (ok)
            {
                TrySyncStudentGroup(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// Hard Delete - სრული წაშლა
        /// </summary>
        public bool HardDelete(int studentId, int groupId)
        {
            var snapshot = GetByStudentAndGroup(studentId, groupId);
            var ok = _repository.HardDelete(studentId, groupId);
            if (ok && snapshot != null)
            {
                _upStreamChangeTracker.TrackStudentGroupChange(snapshot.Id, SyncOperationType.Delete, snapshot);
            }
            return ok;
        }

        /// <summary>
        /// მოსწავლის ყველა ჯგუფიდან Soft Delete
        /// </summary>
        public bool SoftDeleteAllByStudentId(int studentId)
        {
            var snapshots = GetByStudentId(studentId);
            var ok = _repository.SoftDeleteAllByStudentId(studentId);
            if (ok)
            {
                foreach (var snapshot in snapshots)
                {
                    TrySyncStudentGroup(snapshot.StudentId, snapshot.GroupId, SyncOperationType.Update);
                }
            }
            return ok;
        }

        #endregion

        #region ARCHIVE - მოსწავლის არქივირება

        /// <summary>
        /// მოსწავლის ჯგუფიდან არქივირება (სტატუსის განახლება და ჯგუფის მოსწავლეთა რაოდენობის შემცირება)
        /// </summary>
        /// <param name="studentId">მოსწავლის ID</param>
        /// <param name="groupId">ჯგუფის ID</param>
        public void ArchiveStudentFromGroup(int studentId, int groupId)
        {
            UpdateStudentStatus(studentId, groupId, false);
            _groupRepository.DecrementStudentCount(groupId);
            TrySyncGroup(groupId);
        }

        #endregion

        #region Sync Helpers

        private void TrySyncStudentGroup(int studentId, int groupId, SyncOperationType operation)
        {
            try
            {
                var snapshot = GetByStudentAndGroup(studentId, groupId);
                if (snapshot != null)
                {
                    _upStreamChangeTracker.TrackStudentGroupChange(snapshot.Id, operation, snapshot);
                }
            }
            catch { }
        }

        private void TrySyncGroup(int groupId)
        {
            try
            {
                var group = _groupRepository.GetGroupById(groupId);
                if (group != null)
                {
                    _upStreamChangeTracker.TrackGroupChange(groupId, SyncOperationType.Update, group);
                }
            }
            catch { }
        }

        #endregion
    }
}

