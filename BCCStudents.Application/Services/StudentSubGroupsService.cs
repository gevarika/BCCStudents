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

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (სრული ობიექტით)
        /// </summary>
        public int AddStudentSubGroup(StudentSubGroups studentSubGroup)
        {
            return _repository.InsertStudentSubGroup(studentSubGroup);
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (ID-ებით)
        /// </summary>
        public int AddStudentSubGroup(int studentId, int groupId, int subGroupId)
        {
            return _repository.InsertStudentSubGroup(studentId, groupId, subGroupId);
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფში დამატება (ფასდაკლებით)
        /// </summary>
        public int AddStudentSubGroupWithDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            return _repository.InsertStudentSubGroupWithDiscount(studentId, groupId, subGroupId, discount);
        }

        #endregion

        #region SELECT - მოსწავლე-ქვეჯგუფის მიღება

        /// <summary>
        /// მოსწავლე-ქვეჯგუფის მიღება ID-ით
        /// </summary>
        public StudentSubGroups GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// მოსწავლე-ქვეჯგუფის მიღება StudentId, GroupId და SubGroupId-ით
        /// </summary>
        public StudentSubGroups GetByStudentGroupAndSubGroup(int studentId, int groupId, int subGroupId)
        {
            return _repository.GetByStudentGroupAndSubGroup(studentId, groupId, subGroupId);
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფის მიღება ჯგუფის მიხედვით
        /// </summary>
        public StudentSubGroups GetByStudentAndGroup(int studentId, int groupId)
        {
            return _repository.GetByStudentAndGroup(studentId, groupId);
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფების მიღება
        /// </summary>
        public List<StudentSubGroups> GetByStudentId(int studentId)
        {
            return _repository.GetByStudentId(studentId);
        }

        /// <summary>
        /// მოსწავლის აქტიური ქვეჯგუფების მიღება
        /// </summary>
        public List<StudentSubGroups> GetActiveByStudentId(int studentId)
        {
            return _repository.GetActiveByStudentId(studentId);
        }

        /// <summary>
        /// ქვეჯგუფის მოსწავლეების მიღება
        /// </summary>
        public List<StudentSubGroups> GetBySubGroupId(int subGroupId)
        {
            return _repository.GetBySubGroupId(subGroupId);
        }

        /// <summary>
        /// ქვეჯგუფის აქტიური მოსწავლეების მიღება
        /// </summary>
        public List<StudentSubGroups> GetActiveBySubGroupId(int subGroupId)
        {
            return _repository.GetActiveBySubGroupId(subGroupId);
        }

        /// <summary>
        /// მოსწავლის აქტიური SubGroupId-ის მიღება ჯგუფის მიხედვით
        /// </summary>
        public int? GetActiveSubGroupId(int studentId, int groupId)
        {
            return _repository.GetActiveSubGroupId(studentId, groupId);
        }

        /// <summary>
        /// მოსწავლის ქვეჯგუფების ID-ების მიღება
        /// </summary>
        public List<int> GetSubGroupIdsByStudentId(int studentId)
        {
            return _repository.GetSubGroupIdsByStudentId(studentId);
        }

        /// <summary>
        /// არსებობის შემოწმება (აქტიური)
        /// </summary>
        public bool ExistsActive(int studentId, int groupId, int subGroupId)
        {
            return _repository.ExistsActive(studentId, groupId, subGroupId);
        }

        /// <summary>
        /// არსებობის შემოწმება (ნებისმიერი)
        /// </summary>
        public bool ExistsAny(int studentId, int groupId, int subGroupId)
        {
            return _repository.ExistsAny(studentId, groupId, subGroupId);
        }

        /// <summary>
        /// არსებობის შემოწმება ჯგუფის მიხედვით (ნებისმიერი ქვეჯგუფი)
        /// </summary>
        public bool ExistsAnyForGroup(int studentId, int groupId)
        {
            return _repository.ExistsAnyForGroup(studentId, groupId);
        }

        #endregion

        #region UPDATE - მოსწავლე-ქვეჯგუფის განახლება

        /// <summary>
        /// მოსწავლე-ქვეჯგუფის სრული განახლება
        /// </summary>
        public bool Update(StudentSubGroups studentSubGroup)
        {
            return _repository.Update(studentSubGroup);
        }

        /// <summary>
        /// სტატუსის განახლება (აქტივაცია/დეაქტივაცია)
        /// </summary>
        public bool UpdateStatus(int studentId, int groupId, int subGroupId, bool status)
        {
            return _repository.UpdateStatus(studentId, groupId, subGroupId, status);
        }

        /// <summary>
        /// გადახდის სტატუსის განახლება
        /// </summary>
        public bool UpdatePaymentStatus(int studentId, int groupId, int subGroupId, string paymentStatus)
        {
            return _repository.UpdatePaymentStatus(studentId, groupId, subGroupId, paymentStatus);
        }

        /// <summary>
        /// გადახდის თარიღის განახლება
        /// </summary>
        public bool UpdateDateOfPayment(int studentId, int groupId, int subGroupId, DateTime? dateOfPayment)
        {
            return _repository.UpdateDateOfPayment(studentId, groupId, subGroupId, dateOfPayment);
        }

        /// <summary>
        /// გადახდის თარიღის განახლება (alias)
        /// </summary>
        public bool UpdatePaymentDate(int studentId, int groupId, int subGroupId, DateTime newDate)
        {
            return _repository.UpdatePaymentDate(studentId, groupId, subGroupId, newDate);
        }

        /// <summary>
        /// ფასდაკლების განახლება
        /// </summary>
        public bool UpdateDiscount(int studentId, int groupId, int subGroupId, double discount)
        {
            return _repository.UpdateDiscount(studentId, groupId, subGroupId, discount);
        }

        /// <summary>
        /// SubGroupId-ის განახლება (ქვეჯგუფის შეცვლა)
        /// </summary>
        public bool UpdateSubGroupId(int studentId, int groupId, int oldSubGroupId, int newSubGroupId)
        {
            return _repository.UpdateSubGroupId(studentId, groupId, oldSubGroupId, newSubGroupId);
        }

        /// <summary>
        /// GroupId-ის განახლება (ჯგუფის შეცვლა)
        /// </summary>
        public bool UpdateGroupId(int studentId, int oldGroupId, int newGroupId)
        {
            return _repository.UpdateGroupId(studentId, oldGroupId, newGroupId);
        }

        #endregion

        #region DELETE - მოსწავლე-ქვეჯგუფის წაშლა

        /// <summary>
        /// Soft Delete - მოსწავლის ქვეჯგუფიდან ამოღება
        /// </summary>
        public bool SoftDelete(int studentId, int groupId, int subGroupId)
        {
            return _repository.SoftDelete(studentId, groupId, subGroupId);
        }

        /// <summary>
        /// Hard Delete - სრული წაშლა
        /// </summary>
        public bool HardDelete(int studentId, int groupId, int subGroupId)
        {
            return _repository.HardDelete(studentId, groupId, subGroupId);
        }

        /// <summary>
        /// მოსწავლის ჯგუფის ყველა ქვეჯგუფიდან Soft Delete
        /// </summary>
        public bool SoftDeleteAllByStudentAndGroup(int studentId, int groupId)
        {
            return _repository.SoftDeleteAllByStudentAndGroup(studentId, groupId);
        }

        /// <summary>
        /// მოსწავლის ყველა ქვეჯგუფიდან Soft Delete
        /// </summary>
        public bool SoftDeleteAllByStudentId(int studentId)
        {
            return _repository.SoftDeleteAllByStudentId(studentId);
        }

        #endregion
    }
}

