using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services
{
    public class PendingStudentService : IPendingStudentService
    {
        private readonly IPendingStudentRepository _pendingRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IStudentCodeGenerator _studentCodeGenerator;
        private readonly IGroupRepository _groupRepository;
        private readonly IPendingStudentGroupRepository _pendingGroupRepo;
        private readonly IStudentGroupRepository _studentGroupRepo;
        private readonly IStudentSubGroupRepository _studentSubGroupRepo;
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        public PendingStudentService(
            IGroupRepository groupRepository,
            IPendingStudentRepository pendingRepo,
            IStudentRepository studentRepo,
            IStudentCodeGenerator studentCodeGenerator,
            IPendingStudentGroupRepository pendingStudentGroupRepository,
            IStudentGroupRepository studentGroupRepo,
            IStudentSubGroupRepository studentSubGroupRepo,
            ISubGroupRepository subGroupRepository,
            IUpStreamChangeTracker upStreamChangeTracker)
        {
            _pendingRepo = pendingRepo;
            _studentRepo = studentRepo;
            _studentCodeGenerator = studentCodeGenerator;
            _pendingGroupRepo = pendingStudentGroupRepository;
            _groupRepository = groupRepository;
            _studentGroupRepo = studentGroupRepo;
            _studentSubGroupRepo = studentSubGroupRepo;
            _subGroupRepository = subGroupRepository;
            _upStreamChangeTracker = upStreamChangeTracker;
        }

        public List<PendingStudent> GetAllPending()
        {
            return _pendingRepo.GetAll();
        }

        public void Delete(int Id)
        {
            _pendingRepo.Delete(Id);
        }

        public ApprovalResult ApproveStudent(int pendingStudentId, decimal discountAmount)
        {
            var pendingStudent = _pendingRepo.GetById(pendingStudentId);
            if (pendingStudent == null)
            {
                return new ApprovalResult { IsSuccess = false, Message = "სტუდენტი ვერ მოიძებნა." };
            }

            var groupIds = _pendingGroupRepo.GetGroupsForPendingStudent(pendingStudentId);
            var pendingSubGroups = _pendingGroupRepo.GetPendingSubGroupsByStudentId(pendingStudentId);

            return ConfirmPendingStudent(pendingStudent, groupIds, pendingSubGroups, Convert.ToDouble(discountAmount));
        }

        public ApprovalResult ConfirmPendingStudent(PendingStudent pendingStudent, List<int> selectedGroupIds, List<PendingStudentSubGroup> pendingSubGroups, double discountAmount)
        {
            if (selectedGroupIds.Count > 0)
            {
                var discountPercent = Convert.ToDecimal(discountAmount);
                var postSyncActions = new List<Action>();

                // 1. დამატება Students ცხრილში
                var student = new Student
                {
                    FirstName = pendingStudent.FirstName,
                    LastName = pendingStudent.LastName,
                    Age = pendingStudent.Age,
                    ParentName = pendingStudent.ParentName,
                    PhoneNumber = pendingStudent.PhoneNumber,
                    Id_Numb = pendingStudent.Id_Numb,
                    Address = pendingStudent.Address,
                    RegistrationDate = pendingStudent.RegistrationDate,
                    StudentCode = _studentCodeGenerator.GenerateStudentCode(),
                    User_Id = pendingStudent.UserId,
                    Balance = pendingStudent.Balance,
                    Status = true
                };
                int studentId = _studentRepo.InsertStudent(student);

                if (studentId > 0)
                {
                    postSyncActions.Add(() => SyncStudentSnapshot(studentId, SyncOperationType.Insert));

                    // 2. ჯგუფ(ებ)ში დამატება და რაოდენობის გაზრდა
                    foreach (int groupId in selectedGroupIds)
                    {
                        var group = _groupRepository.GetGroupById(groupId);
                        if (group == null)
                        {
                            continue;
                        }

                        var discountedGroupPrice = GetDiscountedAmount(group.Price, discountPercent);

                        // StudentGroups ცხრილში ჩასმა
                        var studentGroup = new StudentGroups
                        {
                            StudentId = studentId,
                            GroupId = groupId,
                            PaymentStatus = "Pending",
                            DateOfPayment = DateTime.Today.AddMonths(1), // გადახდის თარიღი: დღეს + 1 თვე
                            Price = discountedGroupPrice,
                            Discount = discountAmount, // ფასდაკლების პროცენტი
                            Status = true
                        };
                        _studentGroupRepo.InsertStudentGroup(studentGroup);

                        // ჯგუფის მოსწავლეთა რაოდენობის გაზრდა
                        _groupRepository.IncrementStudentCount(groupId);

                        var groupIdCopy = groupId;
                        var studentIdCopyForGroup = studentId;
                        postSyncActions.Add(() => SyncGroupSnapshot(groupIdCopy, SyncOperationType.Update));
                        postSyncActions.Add(() => SyncStudentGroupSnapshot(studentIdCopyForGroup, groupIdCopy, SyncOperationType.Update));
                    }

                    // 2.5 ქვეჯგუფებში დამატება
                    foreach (var subGroup in pendingSubGroups)
                    {
                        var subGroupEntity = _subGroupRepository.GetSubGroupById(subGroup.SubGroupId);
                        if (subGroupEntity == null)
                        {
                            continue;
                        }

                        var discountedSubGroupPrice = GetDiscountedAmount(subGroupEntity.TuitionFee, discountPercent);

                        // StudentSubGroups ცხრილში ჩასმა
                        var studentSubGroup = new StudentSubGroups
                        {
                            StudentId = studentId,
                            GroupId = subGroup.GroupId,
                            SubGroupId = subGroup.SubGroupId,
                            PaymentStatus = "Pending",
                            DateOfPayment = DateTime.Today.AddMonths(1), // გადახდის თარიღი: დღეს + 1 თვე
                            Price = discountedSubGroupPrice,
                            Discount = discountAmount, // ფასდაკლების პროცენტი
                            Status = true
                        };
                        _studentSubGroupRepo.InsertStudentSubGroup(studentSubGroup);

                        // ქვეჯგუფის მოსწავლეთა რაოდენობის გაზრდა
                        _subGroupRepository.IncrementSubGroupCount(subGroup.SubGroupId, null, null);

                        var subGroupIdCopy = subGroup.SubGroupId;
                        var subGroupGroupIdCopy = subGroup.GroupId;
                        var studentIdCopyForSubGroup = studentId;
                        postSyncActions.Add(() => SyncSubGroupSnapshot(subGroupIdCopy, SyncOperationType.Update));
                        postSyncActions.Add(() => SyncStudentSubGroupSnapshot(studentIdCopyForSubGroup, subGroupGroupIdCopy, subGroupIdCopy, SyncOperationType.Update));
                    }

                    // 3. Pending ჩანაწერების წაშლა (ლოკალურად + სერვერზე)
                    var pendingStudentId = pendingStudent.Id;
                    var pendingGroupLinkIds = _pendingGroupRepo.GetPendingGroupLinkIds(pendingStudentId);
                    var pendingSubGroupLinkIds = _pendingGroupRepo.GetPendingSubGroupLinkIds(pendingStudentId);

                    foreach (var linkId in pendingSubGroupLinkIds)
                    {
                        var linkIdCopy = linkId;
                        postSyncActions.Add(() => _upStreamChangeTracker.TrackDelete("PendingStudentSubGroups", linkIdCopy));
                    }

                    foreach (var linkId in pendingGroupLinkIds)
                    {
                        var linkIdCopy = linkId;
                        postSyncActions.Add(() => _upStreamChangeTracker.TrackDelete("PendingStudentGroups", linkIdCopy));
                    }

                    postSyncActions.Add(() => _upStreamChangeTracker.TrackDelete("PendingStudents", pendingStudentId));

                    _pendingGroupRepo.DeleteSubGroupsByPendingStudentId(pendingStudentId);
                    _pendingGroupRepo.DeleteByPendingStudentId(pendingStudentId);
                    _pendingRepo.Delete(pendingStudentId);

                    foreach (var action in postSyncActions)
                    {
                        TryExecuteSyncAction(action);
                    }
                }
                else
                {
                    return new ApprovalResult { IsSuccess = false, Message = "მოსწავლისთვის დამატება ვერ მოხერხდა." };
                }

                return new ApprovalResult { IsSuccess = true, Message = "მოსწავლე წარმატებით დაემატა." };
            }
            else
            {
                return new ApprovalResult { IsSuccess = false, Message = "მოსწავლისთვის ჯგუფი ვერ მოიძებნა." };
            }
        }

        private static decimal GetDiscountedAmount(decimal baseAmount, decimal discountPercent)
        {
            var calculator = new DiscountCalculator(
                tuitionFee: baseAmount,
                discountAmount: discountPercent
            );

            return calculator.GetFinalAmount();
        }

        public void Update(PendingStudent pendingStudent)
        {
            _pendingRepo.Update(pendingStudent);
        }

        public void UpdatePartial(int id, Dictionary<string, object> fields)
        {
            _pendingRepo.UpdatePartial(id, fields);
        }

        #region Sync Helpers

        private void TryExecuteSyncAction(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch
            {
                // სინქრონიზაციის შეცდომები არ უნდა დაბლოკოს სამუშაო ნაკადი
            }
        }

        private void SyncStudentSnapshot(int studentId, SyncOperationType operation)
        {
            try
            {
                var student = _studentRepo.GetStudentById(studentId);
                if (student != null)
                {
                    _upStreamChangeTracker.TrackStudentChange(studentId, operation, student);
                }
            }
            catch { }
        }

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
            catch { }
        }

        private void SyncSubGroupSnapshot(int subGroupId, SyncOperationType operation)
        {
            try
            {
                var subGroup = _subGroupRepository.GetSubGroupById(subGroupId);
                if (subGroup != null)
                {
                    _upStreamChangeTracker.TrackSubGroupChange(subGroupId, operation, subGroup);
                }
            }
            catch { }
        }

        private void SyncStudentGroupSnapshot(int studentId, int groupId, SyncOperationType operation)
        {
            try
            {
                var snapshot = _studentGroupRepo.GetByStudentAndGroup(studentId, groupId);
                if (snapshot != null)
                {
                    _upStreamChangeTracker.TrackStudentGroupChange(snapshot.Id, operation, snapshot);
                }
            }
            catch { }
        }

        private void SyncStudentSubGroupSnapshot(int studentId, int groupId, int subGroupId, SyncOperationType operation)
        {
            try
            {
                var snapshot = _studentSubGroupRepo.GetByStudentGroupAndSubGroup(studentId, groupId, subGroupId);
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


