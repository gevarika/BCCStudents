using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// StudentSubGroups სერვისი
    /// მოსწავლე-ქვეჯგუფის კავშირების მართვა
    /// </summary>
    public class StudentSubGroupsService : IStudentSubGroupsService
    {
        private readonly IStudentSubGroupRepository _repository;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        public StudentSubGroupsService(IStudentSubGroupRepository repository, IUpStreamChangeTracker upStreamChangeTracker)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));
        }

        #region INSERT - მოსწავლე-ქვეჯგუფის დამატება

        public int AddStudentSubGroup(StudentSubGroups studentSubGroup)
        {
            var id = _repository.InsertStudentSubGroup(studentSubGroup);
            if (id > 0)
            {
                TrySyncStudentSubGroup(studentSubGroup.StudentId, studentSubGroup.GroupId, studentSubGroup.SubGroupId, SyncOperationType.Insert);
            }
            return id;
        }

        public int AddStudentSubGroup(int studentId, int groupId, int subGroupId)
        {
            var id = _repository.InsertStudentSubGroup(studentId, groupId, subGroupId);
            if (id > 0)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Insert);
            }
            return id;
        }

        public int AddStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            var id = _repository.InsertStudentSubGroupWithDiscount(studentId, groupId, subGroupId, discount);
            if (id > 0)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Insert);
            }
            return id;
        }

        #endregion

        #region SELECT - მოსწავლე-ქვეჯგუფის მიღება

        public StudentSubGroups GetById(int id)
        {
            return _repository.GetById(id);
        }

        public StudentSubGroups GetByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId)
        {
            return _repository.GetByStudentGroupAndSubGroup(studentId, groupId, subGroupId);
        }

        public StudentSubGroups GetByStudentAndGroup(int studentId, int groupId)
        {
            return _repository.GetByStudentAndGroup(studentId, groupId);
        }

        public List<StudentSubGroups> GetByStudentId(int studentId)
        {
            return _repository.GetByStudentId(studentId);
        }

        public List<StudentSubGroups> GetActiveByStudentId(int studentId)
        {
            return _repository.GetActiveByStudentId(studentId);
        }

        public List<StudentSubGroups> GetBySubGroupId(int subGroupId)
        {
            return _repository.GetBySubGroupId(subGroupId);
        }

        public List<StudentSubGroups> GetActiveBySubGroupId(int subGroupId)
        {
            return _repository.GetActiveBySubGroupId(subGroupId);
        }

        public int? GetActiveSubGroupId(int studentId, int groupId)
        {
            return _repository.GetActiveSubGroupId(studentId, groupId);
        }

        public List<int> GetSubGroupIdsByStudentId(int studentId)
        {
            return _repository.GetSubGroupIdsByStudentId(studentId);
        }

        public bool ExistsActive(int studentId, int groupId, int subGroupId)
        {
            return _repository.ExistsActive(studentId, groupId, subGroupId);
        }

        public bool ExistsAny(int studentId, int groupId, int subGroupId)
        {
            return _repository.ExistsAny(studentId, groupId, subGroupId);
        }

        public bool ExistsAnyForGroup(int studentId, int groupId)
        {
            return _repository.ExistsAnyForGroup(studentId, groupId);
        }

        #endregion

        #region UPDATE - მოსწავლე-ქვეჯგუფის განახლება

        public bool Update(StudentSubGroups studentSubGroup)
        {
            var ok = _repository.Update(studentSubGroup);
            if (ok)
            {
                TrySyncStudentSubGroup(studentSubGroup.StudentId, studentSubGroup.GroupId, studentSubGroup.SubGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdateStatus(int studentId, int groupId, int subGroupId, bool status)
        {
            var ok = _repository.UpdateStatus(studentId, groupId, subGroupId, status);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdatePaymentStatus(int studentId, int groupId, int subGroupId, string paymentStatus)
        {
            var ok = _repository.UpdatePaymentStatus(studentId, groupId, subGroupId, paymentStatus);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdateDateOfPayment(int studentId, int groupId, int subGroupId, DateTime? dateOfPayment)
        {
            var ok = _repository.UpdateDateOfPayment(studentId, groupId, subGroupId, dateOfPayment);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdatePaymentDate(int studentId, int groupId, int subGroupId, DateTime newDate)
        {
            var ok = _repository.UpdatePaymentDate(studentId, groupId, subGroupId, newDate);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdateDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            var ok = _repository.UpdateDiscount(studentId, groupId, subGroupId, discount);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdateSubGroupId(int studentId, int groupId, int oldSubGroupId, int newSubGroupId)
        {
            var ok = _repository.UpdateSubGroupId(studentId, groupId, oldSubGroupId, newSubGroupId);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, oldSubGroupId, SyncOperationType.Update);
                TrySyncStudentSubGroup(studentId, groupId, newSubGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId)
        {
            var ok = _repository.UpdateGroupId(studentId, oldGroupId, newGroupId);
            if (ok)
            {
                var snapshots = GetByStudentId(studentId)
                    .Where(s => s.GroupId == oldGroupId || s.GroupId == newGroupId)
                    .ToList();
                foreach (var snapshot in snapshots)
                {
                    TrySyncStudentSubGroup(snapshot.StudentId, snapshot.GroupId, snapshot.SubGroupId, SyncOperationType.Update);
                }
            }
            return ok;
        }

        #endregion

        #region DELETE - მოსწავლე-ქვეჯგუფის წაშლა

        public bool SoftDelete(int studentId, int groupId, int subGroupId)
        {
            var ok = _repository.SoftDelete(studentId, groupId, subGroupId);
            if (ok)
            {
                TrySyncStudentSubGroup(studentId, groupId, subGroupId, SyncOperationType.Update);
            }
            return ok;
        }

        public bool HardDelete(int studentId, int groupId, int subGroupId)
        {
            var snapshot = GetByStudentGroupAndSubGroup(studentId, groupId, subGroupId);
            var ok = _repository.HardDelete(studentId, groupId, subGroupId);
            if (ok && snapshot != null)
            {
                _upStreamChangeTracker.TrackStudentSubGroupChange(snapshot.Id, SyncOperationType.Delete, snapshot);
            }
            return ok;
        }

        public bool SoftDeleteAllByStudentAndGroup(int studentId, int groupId)
        {
            var snapshots = GetByStudentId(studentId).Where(s => s.GroupId == groupId).ToList();
            var ok = _repository.SoftDeleteAllByStudentAndGroup(studentId, groupId);
            if (ok)
            {
                foreach (var snapshot in snapshots)
                {
                    TrySyncStudentSubGroup(snapshot.StudentId, snapshot.GroupId, snapshot.SubGroupId, SyncOperationType.Update);
                }
            }
            return ok;
        }

        public bool SoftDeleteAllByStudentId(int studentId)
        {
            var snapshots = GetByStudentId(studentId);
            var ok = _repository.SoftDeleteAllByStudentId(studentId);
            if (ok)
            {
                foreach (var snapshot in snapshots)
                {
                    TrySyncStudentSubGroup(snapshot.StudentId, snapshot.GroupId, snapshot.SubGroupId, SyncOperationType.Update);
                }
            }
            return ok;
        }

        #endregion

        #region Sync Helpers

        private void TrySyncStudentSubGroup(int studentId, int groupId, int subGroupId, SyncOperationType operation)
        {
            try
            {
                var snapshot = GetByStudentGroupAndSubGroup(studentId, groupId, subGroupId);
                if (snapshot != null)
                {
                    _upStreamChangeTracker.TrackStudentSubGroupChange(snapshot.Id, operation, snapshot);
                }
            }
            catch { }
        }

        #endregion
    }
}

