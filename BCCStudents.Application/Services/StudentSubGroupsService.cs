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

        public StudentSubGroupsService(IStudentSubGroupRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region INSERT - მოსწავლე-ქვეჯგუფის დამატება

        public int AddStudentSubGroup(StudentSubGroups studentSubGroup)
        {
            return _repository.InsertStudentSubGroup(studentSubGroup);
        }

        public int AddStudentSubGroup(int studentId, int groupId, int subGroupId)
        {
            return _repository.InsertStudentSubGroup(studentId, groupId, subGroupId);
        }

        public int AddStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            return _repository.InsertStudentSubGroupWithDiscount(studentId, groupId, subGroupId, discount);
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
            return _repository.Update(studentSubGroup);
        }

        public bool UpdateStatus(int studentId, int groupId, int subGroupId, bool status)
        {
            return _repository.UpdateStatus(studentId, groupId, subGroupId, status);
        }

        public bool UpdatePaymentStatus(int studentId, int groupId, int subGroupId, string paymentStatus)
        {
            return _repository.UpdatePaymentStatus(studentId, groupId, subGroupId, paymentStatus);
        }

        public bool UpdateDateOfPayment(int studentId, int groupId, int subGroupId, DateTime? dateOfPayment)
        {
            return _repository.UpdateDateOfPayment(studentId, groupId, subGroupId, dateOfPayment);
        }

        public bool UpdatePaymentDate(int studentId, int groupId, int subGroupId, DateTime newDate)
        {
            return _repository.UpdatePaymentDate(studentId, groupId, subGroupId, newDate);
        }

        public bool UpdateDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            return _repository.UpdateDiscount(studentId, groupId, subGroupId, discount);
        }

        public bool UpdateSubGroupId(int studentId, int groupId, int oldSubGroupId, int newSubGroupId)
        {
            return _repository.UpdateSubGroupId(studentId, groupId, oldSubGroupId, newSubGroupId);
        }

        public bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId)
        {
            return _repository.UpdateGroupId(studentId, oldGroupId, newGroupId);
        }

        #endregion

        #region DELETE - მოსწავლე-ქვეჯგუფის წაშლა

        public bool SoftDelete(int studentId, int groupId, int subGroupId)
        {
            return _repository.SoftDelete(studentId, groupId, subGroupId);
        }

        public bool HardDelete(int studentId, int groupId, int subGroupId)
        {
            return _repository.HardDelete(studentId, groupId, subGroupId);
        }

        public bool SoftDeleteAllByStudentAndGroup(int studentId, int groupId)
        {
            return _repository.SoftDeleteAllByStudentAndGroup(studentId, groupId);
        }

        public bool SoftDeleteAllByStudentId(int studentId)
        {
            return _repository.SoftDeleteAllByStudentId(studentId);
        }

        #endregion
    }
}
