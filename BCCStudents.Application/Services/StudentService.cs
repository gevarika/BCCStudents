using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace BCCStudents.Application.Services
{
    public partial class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        //private readonly IGroupRepository _groupRepository;
        private readonly ISubGroupService _subGroupService;
        private readonly ILoggerRepository _loggerRepository;
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IStudentGroupsService _studentGroupsService;
        private readonly IStudentSubGroupRepository _studentSubGroupRepository;
        private readonly DocumentService _documentService;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;
        private readonly IStudentJsonService _studentJsonService;
        private readonly IStudentCodeGenerator _studentCodeGenerator;
        private readonly IGroupRepository _groupRepository;
        private readonly IApplicationStatus _appStatus;

        public StudentService(IStudentRepository studentRepository,
            IGroupRepository groupRepository,
            ILoggerRepository loggerRepository,
            ISubGroupService subGroupService,
            ISubGroupRepository subGroupRepository,
            IStudentGroupsService studentGroupsService,
            IStudentSubGroupRepository studentSubGroupRepository,
            IStudentCodeGenerator studentCodeGenerator,
            IDatabaseConnectionProvider connectionProvider,
            DocumentService documentService,
            IServiceProvider serviceProvider,
            IUpStreamChangeTracker upStreamChangeTracker,
            IStudentJsonService studentJsonService,
            IApplicationStatus appStatus
            )
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _subGroupRepository = subGroupRepository;
            _studentGroupsService = studentGroupsService;
            _studentSubGroupRepository = studentSubGroupRepository;
            _loggerRepository = loggerRepository;
            //_studentCodeGenerator = studentCodeGenerator ?? throw new ArgumentNullException(nameof(studentCodeGenerator));
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _documentService = documentService;
            _studentCodeGenerator = studentCodeGenerator;
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));
            _studentJsonService = studentJsonService ?? throw new ArgumentNullException(nameof(studentJsonService));
            _appStatus = appStatus ?? throw new ArgumentNullException(nameof(appStatus));
        }

        /// <summary>
        /// მოსწავლის სხვა ჯგუფში გადატანა
        /// </summary>
        public void MigrateStudentToGroup(int studentId, int oldGroupId, int newGroupId)
        {
            // ძველი ჯგუფიდან დეაქტივაცია
            _studentGroupsService.UpdateStudentStatus(studentId, oldGroupId, false);
            _groupRepository.DecrementStudentCount(oldGroupId);

            // ახალ ჯგუფში დამატება
            AddStudentToGroup(studentId, newGroupId, true);
            _groupRepository.IncrementStudentCount(newGroupId);

            // ახალ ჯგუფის პირველ ქვეჯგუფში დამატება
            try
            {
                var subGroup = _subGroupService.GetFirstSubGroupByGroupId(newGroupId);
                if (subGroup != null)
                {
                    AddStudentToSubGroup(studentId, newGroupId, subGroup.Id, "Pending", null, 0, 0, true);
                }
            }
            catch (Exception ex)
            {
                // Avoid silent partial migration; keep flow but leave an audit trail.
                _loggerRepository.WriteLog(
                    "Student Migration",
                    "Warning",
                    $"StudentId={studentId}, OldGroupId={oldGroupId}, NewGroupId={newGroupId}, SubGroupAssignmentFailed: {ex.Message}",
                    "System");
            }

            // ძველი ჯგუფის ქვეჯგუფებიდან წაშლა
            _subGroupService.RemoveStudentFromAllSubGroups(studentId, oldGroupId);
        }
        public DataTable GetUnassignedStudents()
        { return _studentRepository.GetUnassignedStudents(); }
        public DataTable GetAllStudentsFor()
        { return _studentRepository.GetAllStudentsFor(); }
        public List<StudentViewDto> GetAllStudentsSomeInfo()
        { return _studentRepository.GetAllStudentsSomeInfo(); }
        public List<Student> GetAllStudents()
        { return _studentRepository.GetAllStudents(); }
        public DataTable FilterStudents(string name, int? groupId, int? subGroupId, DateTime? startDate, DateTime? endDate)
        { return _studentRepository.FilterStudents(name, groupId, subGroupId, startDate, endDate); }
        public List<(int Id, string FullName)> GetStudentNames()
        { return _studentRepository.GetStudentNames(); }
        public string GetStudentName(int studentId)
        { return _studentRepository.GetStudentName(studentId); }
        public List<Student> GetStudentsByGroupId(int groupId)
        {
            return _studentRepository.GetStudentsByGroupId(groupId);
        }
        public List<Student> SearchStudentsByNameAndGroup(string text, int groupId)
        { return _studentRepository.SearchStudentsByNameAndGroup(text, groupId); }
        public List<Student> GetAllStudentsWithGroups()
        {
            return _studentRepository.GetAllStudentsWithGroups();
        }

        public List<Student> GetStudentsInMultipleGroups()
        {
            return _studentRepository.GetStudentsInMultipleGroups();
        }
        public List<Student> SearchStudentsByNameAcrossAllGroups(string name)
        {
            return _studentRepository.SearchStudentsByNameAcrossAllGroups(name);
        }

        public List<Student> SearchStudents(string fieldName, string searchText, int? groupId = null)
        {
            return _studentRepository.SearchStudents(fieldName, searchText, groupId);
        }

        // Duplicate checks
        public bool ExistsStudentByName(string firstName, string lastName)
        { return _studentRepository.ExistsByName(firstName, lastName); }
        public bool ExistsStudentByNameParentAddress(string firstName, string lastName, string parentName, string address)
        { return _studentRepository.ExistsByNameParentAddress(firstName, lastName, parentName, address); }
        public int CountStudentsByAddress(string address)
        { return _studentRepository.CountByAddress(address); }

        public bool AddStudent(Student student, List<int> groupIds, int userId, bool printContract, OperationResultContext result, out int studentId)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. Student add operation is blocked.");

            if (student.Discount > 0 && string.IsNullOrWhiteSpace(student.Info))
            {
                result.AddError("რეგისტრაცია", "ფასდაკლების მითითებისას სავალდებულია მოსწავლის სტატუსი (Info).");
                studentId = 0;
                return false;
            }

            studentId = 0;
            var postCommitSyncActions = new List<Action>();
            var studentSyncScheduled = false;

            using (var connection = _connectionProvider.GetLocalConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        bool allSuccess = true;
                        int ret = 0;
                        if (printContract)
                        {
                            var selectedGroups = _groupRepository.GetGroupsByIds(groupIds);

                            // თითოეული ჯგუფისთვის შევქმნათ თავისი replacementData
                            var templatePaths = new List<string>();
                            var replacementDatas = new List<Dictionary<string, string>>();
                            foreach (var group in selectedGroups)
                            {
                                if (string.IsNullOrWhiteSpace(group.ContractTemplatePath))
                                {
                                    result.AddError("ხელშეკრულების ბეჭდვა", $"ჯგუფს {group.Name} არ აქვს ხელშეკრულების შაბლონი მიმაგრებული");
                                    transaction.Rollback();
                                    return false;
                                }
                                var groupPrice = _groupRepository.GetGroupPrice(group.Id);
                                var discountedPrice = new DiscountCalculator(tuitionFee: groupPrice, discountAmount: Convert.ToDecimal(student.Discount));
                                decimal finalPrice = discountedPrice.GetFinalAmount();
                                var replacementData = new Dictionary<string, string>
                                {
                                    { "{FirstName}", student.FirstName },
                                    { "{LastName}", student.LastName },
                                    { "{PhoneNumber}", student.PhoneNumber },
                                    { "{IdNumber}", student.Id_Numb.ToString() },
                                    { "{Address}", student.Address },
                                    { "{Price}", finalPrice.ToString("N2") },
                                    { "{ParrentName}", student.ParentName },
                                    // სხვა საჭირო ველები...
                                };
                                templatePaths.Add(Path.Combine(System.Windows.Forms.Application.StartupPath, group.ContractTemplatePath));
                                replacementDatas.Add(replacementData);
                            }
                            _documentService.GenerateAndPrintContractsBatch(templatePaths, replacementDatas);

                            // 2. ხელმოწერის დადასტურება მხოლოდ ერთხელ ყველა ხელშეკრულებისთვის
                            var confirm = MessageBox.Show(
                                "გთხოვთ, ყველა ხელშეკრულებას მოაწეროს ხელი მშობელმა და დაადასტურეთ გაგრძელება.",
                                "ხელმოწერის დადასტურება",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (confirm != DialogResult.Yes)
                            {
                                MessageBox.Show("ხელმოწერის გარეშე რეგისტრაცია არ მოხდება.", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                transaction.Rollback();
                                return false;
                            }
                        }
                        // 2. თუ ყველა ხელმოწერა დადასტურდა ან ხელშეკრულების ბეჭდვა არ იყო საჭირო, მხოლოდ მაშინ ჩაწერე ბაზაში
                        student.User_Id = userId;
                        int sid = _studentRepository.InsertStudent(student, connection, transaction);
                        if (sid == 0) { allSuccess = false; }
                        studentId = sid;
                        var insertedStudentId = studentId;
                        if (insertedStudentId > 0 && !studentSyncScheduled)
                        {
                            studentSyncScheduled = true;
                            postCommitSyncActions.Add(() => SyncStudentSnapshot(insertedStudentId, SyncOperationType.Insert));
                        }
                        foreach (var groupId in groupIds)
                        {
                            var groupPrice = _groupRepository.GetGroupPrice(groupId);

                            // ფასდაკლების გამოთვლა DiscountCalculator კლასით
                            var discount = student.Discount; // პროცენტი (მაგ: 10, 20, 50)
                            var discountCalculator = new DiscountCalculator(groupPrice, (decimal)discount);
                            var discountedPrice = discountCalculator.GetFinalAmount();

                            var studentGroup = new StudentGroups
                            {
                                StudentId = studentId,
                                GroupId = groupId,
                                PaymentStatus = "Pending",
                                Price = discountedPrice, // ფასდაკლებული ფასი
                                Discount = discount,     // ფასდაკლების პროცენტი
                                DateOfPayment = student.DateOfPayment ?? DateTime.Today.AddMonths(1),
                                Status = true
                            };
                            int sgId = _studentGroupsService.AddStudentGroup(studentGroup, connection, transaction);
                            if (sgId == 0) { allSuccess = false; break; }
                            _groupRepository.RecalculateStudentCount(groupId, connection, transaction);
                            var groupIdCopy = groupId;
                            var studentIdCopyForGroup = insertedStudentId;
                            postCommitSyncActions.Add(() => SyncGroupSnapshot(groupIdCopy, SyncOperationType.Update));
                            postCommitSyncActions.Add(() => SyncStudentGroupSnapshot(studentIdCopyForGroup, groupIdCopy, SyncOperationType.Update));
                            var subGroup = _subGroupRepository.GetFirstSubGroupByGroupId(groupId);
                            if (subGroup != null)
                            {
                                var studentSubGroup = new StudentSubGroups
                                {
                                    StudentId = studentId,
                                    GroupId = subGroup.GroupId,
                                    SubGroupId = subGroup.Id,
                                    PaymentStatus = "Pending",
                                    Price = discountedPrice, // ფასდაკლებული ფასი
                                    Discount = discount,     // ფასდაკლების პროცენტი
                                    DateOfPayment = student.DateOfPayment ?? DateTime.Today.AddMonths(1),
                                    Status = true
                                };
                                int ssgId = _studentSubGroupRepository.InsertStudentSubGroup(studentSubGroup, connection, transaction);
                                if (ssgId == 0) { allSuccess = false; break; }
                                _subGroupRepository.IncrementSubGroupCount(subGroup.Id, connection, transaction);
                                var subGroupIdCopy = subGroup.Id;
                                var studentIdCopyForSubGroup = insertedStudentId;
                                var subGroupGroupIdCopy = subGroup.GroupId;
                                postCommitSyncActions.Add(() => SyncSubGroupSnapshot(subGroupIdCopy, SyncOperationType.Update));
                                postCommitSyncActions.Add(() => SyncStudentSubGroupSnapshot(studentIdCopyForSubGroup, subGroupGroupIdCopy, subGroupIdCopy, SyncOperationType.Update));
                            }
                        }
                        _loggerRepository.LogStudentAction("Register", "Success", $"დარეგისტრირდა სტუდენტი: {student.FirstName} {student.LastName}", userId.ToString());
                        ret = studentId;
                        if (allSuccess)
                        {
                            transaction.Commit();
                            foreach (var action in postCommitSyncActions)
                            {
                                TryExecuteSyncAction(action);
                            }
                            return true;
                        }
                        else
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Transaction rolled back: " + ex.Message);
                        _loggerRepository.WriteLog("Student Add", "Failure", ex.ToString(), userId.ToString());
                        return false;
                    }
                }
            }
        }
        public void DeleteStudent(int studentId, int userId)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. Student delete operation is blocked.");
            var snapshot = _studentRepository.GetStudentById(studentId);
            // 1. წავშალოთ ჯგუფებთან კავშირი
            _studentRepository.RemoveStudentFromGroups(studentId);

            // 2. წავშალოთ სტუდენტი
            _studentRepository.DeleteStudent(studentId, userId);

            // 3. ჩავწეროთ ლოგი
            _loggerRepository.LogStudentAction("Delete", "Success", $"წაიშალა სტუდენტი ID: {studentId}", userId.ToString());

            if (snapshot != null)
            {
                _upStreamChangeTracker.TrackStudentChange(studentId, SyncOperationType.Delete, snapshot);
            }
        }

        public decimal CalculateFinalFee(decimal baseFee, decimal discountPercentage)
        {
            decimal discountAmount = baseFee * (discountPercentage / 100);
            decimal finalFee = baseFee - discountAmount;

            return finalFee < 0 ? 0 : finalFee;
        }

        public Student GetStudentDetailsById(int studentId, int groupId)
        {
            return _studentRepository.GetStudentDetailsById(studentId, groupId);
        }
        public void UpdateStudent(Student student)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. Student update operation is blocked.");
            student.UpdatedAt = DateTime.Now;
            _studentRepository.UpdateStudent(student);
            SyncStudentSnapshot(student.Id, SyncOperationType.Update);
        }

        /// <summary>
        /// მხოლოდ კონკრეტული ველების განახლება
        /// </summary>
        public void UpdateStudentFields(int studentId, Dictionary<string, object> changedFields)
        {
            if (changedFields == null || changedFields.Count == 0) return;
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. Student field update operation is blocked.");

            _studentRepository.UpdateStudentFields(studentId, changedFields);
            SyncStudentSnapshot(studentId, SyncOperationType.Update);
        }



        public void UpdateStudentGroupFields(StudentGroups original, StudentGroups updated)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. Student group update operation is blocked.");
            _studentRepository.UpdateStudentGroupFields(original, updated);
            SyncStudentGroupSnapshot(original.StudentId, original.GroupId, SyncOperationType.Update);
        }

        public bool UpdateStudentStatus(int studentId, int groupId, bool status)
        {
            var ok = _studentRepository.UpdateStudentStatus(studentId, groupId, status);
            if (ok)
            {
                SyncStudentGroupSnapshot(studentId, groupId, SyncOperationType.Update);
            }
            return ok;
        }

        /// <summary>
        /// Adds a student to a group with the specified status (default: 'Active').
        /// </summary>
        public void AddStudentToGroup(int studentId, int groupId, bool status = true, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. AddStudentToGroup operation is blocked.");
            // ჯგუფის ფასის მიღება
            var groupPrice = _groupRepository.GetGroupPrice(groupId);

            var studentGroup = new StudentGroups
            {
                StudentId = studentId,
                GroupId = groupId,
                PaymentStatus = "Pending",
                DateOfPayment = DateTime.Today.AddMonths(1), // გადახდის თარიღი: დღეს + 1 თვე
                Price = groupPrice,
                Discount = 0, // default ფასდაკლება = 0
                Status = status
            };
            _studentGroupsService.AddStudentGroup(studentGroup, externalConnection, externalTransaction);
            _groupRepository.RecalculateStudentCount(groupId, externalConnection, externalTransaction);
            SyncStudentGroupSnapshot(studentId, groupId, SyncOperationType.Update);
            SyncGroupSnapshot(groupId, SyncOperationType.Update);
        }

        // Overload used by UI when აქვს სრულ ველებს
        public void AddStudentToGroup(int studentId, int groupId, bool status, DateTime? dateOfPayment, string paymentStatus, decimal price, double discount)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. AddStudentToGroup operation is blocked.");
            var studentGroup = new StudentGroups
            {
                StudentId = studentId,
                GroupId = groupId,
                PaymentStatus = paymentStatus,
                DateOfPayment = dateOfPayment ?? DateTime.Today.AddMonths(1), // თუ null-ია, default = დღეს + 1 თვე
                Price = price,
                Discount = discount,
                Status = status
            };
            _studentGroupsService.AddStudentGroup(studentGroup);
            _groupRepository.RecalculateStudentCount(groupId, null, null);
            SyncStudentGroupSnapshot(studentId, groupId, SyncOperationType.Update);
            SyncGroupSnapshot(groupId, SyncOperationType.Update);
        }

        public void AddStudentToSubGroup(int studentId, int groupId, int subGroupId, string paymentStatus, DateTime? dateOfPayment, decimal price, double discount, bool status)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. AddStudentToSubGroup operation is blocked.");
            var studentSubGroup = new StudentSubGroups
            {
                StudentId = studentId,
                GroupId = groupId,
                SubGroupId = subGroupId,
                PaymentStatus = paymentStatus,
                DateOfPayment = dateOfPayment ?? DateTime.Today.AddMonths(1), // თუ null-ია, default = დღეს + 1 თვე
                Price = price,
                Discount = discount,
                Status = status
            };
            int result = _studentSubGroupRepository.InsertStudentSubGroup(studentSubGroup);
            if (result > 0)
            {
                _subGroupRepository.IncrementSubGroupCount(subGroupId, null, null);
                SyncStudentSubGroupSnapshot(studentId, groupId, subGroupId, SyncOperationType.Update);
                SyncSubGroupSnapshot(subGroupId, SyncOperationType.Update);
            }
            else
            {
                // თუ InsertStudentSubGroup წარუმატებელია, ვნახოთ რა მოხდა
                System.Diagnostics.Debug.WriteLine($"InsertStudentSubGroup failed for StudentId={studentId}, GroupId={groupId}, SubGroupId={subGroupId}");
            }
        }

        /// <summary>
        /// ამოწმებს არის თუ არა მოსწავლე ჯგუფში (აქტიური ჩანაწერი)
        /// </summary>
        public bool IsStudentInGroup(int studentId, int groupId)
        {
            return _studentRepository.StudentGroupExists(studentId, groupId);
        }

        /// <summary>
        /// განაახლებს მოსწავლის ჯგუფს, შეინარჩუნებს არსებულ DateOfPayment, PaymentStatus, Price, Discount ველებს
        /// აბრუნებს ძველ GroupId-ს
        /// </summary>
        public int? UpdateStudentGroupId(int studentId, int newGroupId)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. UpdateStudentGroupId operation is blocked.");
            var oldGroupId = GetCurrentGroupId(studentId);
            if (!oldGroupId.HasValue)
            {
                return null; // არ მოიძებნა აქტიური ჯგუფი
            }

            var success = _studentRepository.UpdateStudentGroupId(studentId, newGroupId);
            if (success)
            {
                // განვაახლოთ StudentCount ორივე ჯგუფისთვის
                _groupRepository.RecalculateStudentCount(oldGroupId.Value, null, null);
                _groupRepository.RecalculateStudentCount(newGroupId, null, null);

                // სინქრონიზაცია
                SyncStudentGroupSnapshot(studentId, newGroupId, SyncOperationType.Update);
                SyncGroupSnapshot(oldGroupId.Value, SyncOperationType.Update);
                SyncGroupSnapshot(newGroupId, SyncOperationType.Update);
            }

            return success ? oldGroupId : null;
        }

        /// <summary>
        /// განაახლებს მოსწავლის ქვეჯგუფს, შეინარჩუნებს არსებულ DateOfPayment, PaymentStatus, Price, Discount ველებს
        /// </summary>
        public void UpdateStudentSubGroupId(int studentId, int newGroupId, int newSubGroupId)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. UpdateStudentSubGroupId operation is blocked.");
            // ვიღებთ ძველ GroupId-ს StudentSubGroups-დან, რადგან StudentSubGroups-ში ჩანაწერი კვლავ ძველ GroupId-თან არის დაკავშირებული
            // (UpdateStudentGroupId განაახლებს მხოლოდ StudentGroups-ში GroupId-ს, არა StudentSubGroups-ში)
            var oldGroupId = GetCurrentGroupIdFromStudentSubGroups(studentId);
            if (!oldGroupId.HasValue)
            {
                // თუ ვერ მოიძებნა, სცადე ახალი GroupId-თვის (შეიძლება უკვე განახლებული იყოს)
                oldGroupId = newGroupId;
            }

            // გამოვიყენოთ SubGroupRepository-ის მეთოდი, რომელიც შეინარჩუნებს სხვა ველებს
            // მნიშვნელოვანია: გამოვიყენოთ ძველი GroupId ჩანაწერის მოსაძებნად
            var oldSubGroupId = _subGroupRepository.GetCurrentStudentSubGroupId(studentId, oldGroupId.Value, default);

            // თუ ძველი GroupId-თვის ვერ მოიძებნა, სცადე ახალი GroupId-თვის
            if (oldSubGroupId <= 0 && oldGroupId.Value != newGroupId)
            {
                oldSubGroupId = _subGroupRepository.GetCurrentStudentSubGroupId(studentId, newGroupId, default);
                if (oldSubGroupId > 0)
                {
                    oldGroupId = newGroupId;
                }
            }

            if (oldSubGroupId > 0)
            {
                // განვაახლოთ SubGroupId ძველი GroupId-თვის
                _subGroupRepository.UpdateStudentSubGroup(studentId, oldGroupId.Value, newSubGroupId, oldSubGroupId);

                // თუ GroupId შეიცვალა, განვაახლოთ GroupId-ც StudentSubGroups-ში
                if (oldGroupId.Value != newGroupId)
                {
                    UpdateStudentSubGroupGroupId(studentId, oldGroupId.Value, newGroupId);
                }

                // განვაახლოთ StudentCount ორივე ქვეჯგუფისთვის
                if (oldSubGroupId != newSubGroupId)
                {
                    _subGroupRepository.DecreaseStudentCount(oldSubGroupId);
                    SyncSubGroupSnapshot(oldSubGroupId, SyncOperationType.Update);
                }

                _subGroupRepository.IncrementSubGroupCount(newSubGroupId, null, null);
                SyncStudentSubGroupSnapshot(studentId, newGroupId, newSubGroupId, SyncOperationType.Update);
                SyncSubGroupSnapshot(newSubGroupId, SyncOperationType.Update);
            }
        }

        /// <summary>
        /// იღებს მოსწავლის GroupId-ს StudentSubGroups ცხრილიდან
        /// </summary>
        private int? GetCurrentGroupIdFromStudentSubGroups(int studentId)
        {
            try
            {
                using (var conn = _connectionProvider.GetLocalConnection())
                {
                    conn.Open();
                    var query = "SELECT GroupId FROM StudentSubGroups WHERE StudentId = @studentId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL) LIMIT 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// განაახლებს StudentSubGroups-ში GroupId-ს
        /// </summary>
        private void UpdateStudentSubGroupGroupId(int studentId, int oldGroupId, int newGroupId)
        {
            try
            {
                using (var conn = _connectionProvider.GetLocalConnection())
                {
                    conn.Open();
                    var query = @"
                        UPDATE StudentSubGroups
                        SET GroupId = @newGroupId, UpdatedAt = @UpdatedAt
                        WHERE StudentId = @studentId AND GroupId = @oldGroupId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newGroupId", newGroupId);
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        cmd.Parameters.AddWithValue("@oldGroupId", oldGroupId);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// იღებს მოსწავლის ამჟამინდელ აქტიურ ჯგუფის ID-ს
        /// </summary>
        private int? GetCurrentGroupId(int studentId)
        {
            try
            {
                using (var conn = _connectionProvider.GetLocalConnection())
                {
                    conn.Open();
                    var query = "SELECT GroupId FROM StudentGroups WHERE StudentId = @studentId AND Status = 1 AND (IsDeleted = 0 OR IsDeleted IS NULL) LIMIT 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Removes a student from a specific group.
        /// </summary>
        public void RemoveStudentFromGroup(int studentId, int groupId)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. RemoveStudentFromGroup operation is blocked.");
            // 1. ვიღებთ snapshots-ს სინქრონიზაციისთვის (soft delete-ის წინ)
            var studentGroupSnapshot = GetStudentGroupSnapshot(studentId, groupId);
            var studentSubGroupSnapshots = GetStudentSubGroupSnapshots(studentId, groupId);

            // 2. ვიღებთ SubGroupIds-ს StudentCount-ის განახლებისთვის
            var subGroupIds = new List<int>();
            try
            {
                using (var conn = _connectionProvider.GetLocalConnection())
                {
                    conn.Open();
                    var query = "SELECT DISTINCT SubGroupId FROM StudentSubGroups WHERE StudentId=@sid AND GroupId=@gid AND (IsDeleted=0 OR IsDeleted IS NULL) AND Status=1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", studentId);
                        cmd.Parameters.AddWithValue("@gid", groupId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                subGroupIds.Add(Convert.ToInt32(r["SubGroupId"]));
                            }
                        }
                    }
                }
            }
            catch { }

            // 3. Soft delete: StudentGroups და StudentSubGroups (ლოკალურ ბაზაში Status=0, IsDeleted=1)
            _studentRepository.RemoveStudentFromGroup(studentId, groupId);

            // 4. UpStream სინქრონიზაცია: StudentGroups
            // მნიშვნელოვანია: ვიყენებთ Update ოპერაციას Status=false-ით, რადგან სერვერზეც უნდა გავაკეთოთ soft delete
            if (studentGroupSnapshot != null)
            {
                // ვქმნით განახლებულ snapshot-ს Status=false-ით
                var updatedSnapshot = new StudentGroups
                {
                    Id = studentGroupSnapshot.Id,
                    StudentId = studentGroupSnapshot.StudentId,
                    GroupId = studentGroupSnapshot.GroupId,
                    Status = false, // Soft delete
                    PaymentStatus = studentGroupSnapshot.PaymentStatus,
                    DateOfPayment = studentGroupSnapshot.DateOfPayment,
                    Price = studentGroupSnapshot.Price,
                    Discount = studentGroupSnapshot.Discount,
                    UpdatedAt = DateTime.Now
                };
                _upStreamChangeTracker.TrackStudentGroupChange(updatedSnapshot.Id, SyncOperationType.Update, updatedSnapshot);
            }

            // 5. UpStream სინქრონიზაცია: StudentSubGroups
            // მნიშვნელოვანია: ვიყენებთ Update ოპერაციას Status=false-ით
            foreach (var subGroupSnapshot in studentSubGroupSnapshots)
            {
                if (subGroupSnapshot != null)
                {
                    // ვქმნით განახლებულ snapshot-ს Status=false-ით
                    var updatedSubGroupSnapshot = new StudentSubGroups
                    {
                        Id = subGroupSnapshot.Id,
                        StudentId = subGroupSnapshot.StudentId,
                        GroupId = subGroupSnapshot.GroupId,
                        SubGroupId = subGroupSnapshot.SubGroupId,
                        Status = false, // Soft delete
                        PaymentStatus = subGroupSnapshot.PaymentStatus,
                        DateOfPayment = subGroupSnapshot.DateOfPayment,
                        Price = subGroupSnapshot.Price,
                        Discount = subGroupSnapshot.Discount,
                        UpdatedAt = DateTime.Now
                    };
                    _upStreamChangeTracker.TrackStudentSubGroupChange(updatedSubGroupSnapshot.Id, SyncOperationType.Update, updatedSubGroupSnapshot);
                }
            }

            // 6. Local recounts after soft-delete: Groups
            try
            {
                _groupRepository.RecalculateStudentCount(groupId, null, null);
                SyncGroupSnapshot(groupId, SyncOperationType.Update);
            }
            catch { }

            // 7. Local recounts after soft-delete: SubGroups
            foreach (var subGroupId in subGroupIds)
            {
                try
                {
                    _subGroupRepository.DecreaseStudentCount(subGroupId);
                    SyncSubGroupSnapshot(subGroupId, SyncOperationType.Update);
                }
                catch { }
            }
        }

        // ↓↓↓ JSON-დან მონაცემების ოპერაციები ↓↓↓
        public List<Student> LoadStudentsFromJson()
        {
            return _studentJsonService.LoadStudents();
        }

        public void SaveStudentsToJson(List<Student> students)
        {
            _studentJsonService.SaveStudents(students);
        }

        public void DeleteStudentFromJson(List<Student> students, int index)
        {
            if (index >= 0 && index < students.Count)
            {
                students.RemoveAt(index);
                SaveStudentsToJson(students);
            }
        }

        #region Sync Helpers

        /// <summary>
        /// Executes sync actions safely so that upstream failures do not break the primary transaction.
        /// </summary>
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

        /// <summary>
        /// Loads the latest student snapshot and tracks it via UpStreamChangeTracker.
        /// </summary>
        private void SyncStudentSnapshot(int studentId, SyncOperationType operation)
        {
            try
            {
                var student = _studentRepository.GetStudentById(studentId);
                if (student != null)
                {
                    _upStreamChangeTracker.TrackStudentChange(studentId, operation, student);
                }
            }
            catch { }
        }

        /// <summary>
        /// Loads the latest group snapshot (StudentCount, Status...) and sends it upstream.
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
            catch { }
        }

        /// <summary>
        /// Loads the latest subgroup snapshot and synchronises it.
        /// </summary>
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

        /// <summary>
        /// Reads StudentGroups row (StudentId + GroupId) and sends it upstream.
        /// </summary>
        private void SyncStudentGroupSnapshot(int studentId, int groupId, SyncOperationType operation)
        {
            try
            {
                var snapshot = GetStudentGroupSnapshot(studentId, groupId);
                if (snapshot != null)
                {
                    _upStreamChangeTracker.TrackStudentGroupChange(snapshot.Id, operation, snapshot);
                }
            }
            catch { }
        }

        /// <summary>
        /// Reads StudentSubGroups row (StudentId + GroupId + SubGroupId) and sends it upstream.
        /// </summary>
        private void SyncStudentSubGroupSnapshot(int studentId, int groupId, int subGroupId, SyncOperationType operation)
        {
            try
            {
                var snapshot = GetStudentSubGroupSnapshot(studentId, groupId, subGroupId);
                if (snapshot != null)
                {
                    _upStreamChangeTracker.TrackStudentSubGroupChange(snapshot.Id, operation, snapshot);
                }
            }
            catch { }
        }

        /// <summary>
        /// Fetches the most recent StudentGroups entry for the given student/group pair.
        /// </summary>
        private StudentGroups GetStudentGroupSnapshot(int studentId, int groupId)
        {
            try
            {
                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();
                    const string sql = @"SELECT Id, StudentId, GroupId, PaymentStatus, DateOfPayment, Price, Discount, Status, UpdatedAt
                                         FROM StudentGroups
                                         WHERE StudentId = @sid AND GroupId = @gid
                                         ORDER BY Id DESC
                                         LIMIT 1";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@sid", studentId);
                        command.Parameters.AddWithValue("@gid", groupId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new StudentGroups
                                {
                                    Id = reader.GetInt32("Id"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    GroupId = reader.GetInt32("GroupId"),
                                    PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                                    DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                                    Price = reader["Price"] == DBNull.Value ? 0 : reader.GetDecimal("Price"),
                                    Discount = reader["Discount"] == DBNull.Value ? 0 : reader.GetDouble("Discount"),
                                    Status = reader["Status"] != DBNull.Value && reader.GetBoolean("Status"),
                                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                                };
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Fetches all StudentSubGroups entries for the given student/group pair.
        /// </summary>
        private List<StudentSubGroups> GetStudentSubGroupSnapshots(int studentId, int groupId)
        {
            var snapshots = new List<StudentSubGroups>();
            try
            {
                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();
                    const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId, Status, PaymentStatus, DateOfPayment, Price, Discount, UpdatedAt
                                         FROM StudentSubGroups
                                         WHERE StudentId = @sid AND GroupId = @gid AND (IsDeleted=0 OR IsDeleted IS NULL) AND Status=1";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@sid", studentId);
                        command.Parameters.AddWithValue("@gid", groupId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                snapshots.Add(new StudentSubGroups
                                {
                                    Id = reader.GetInt32("Id"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    GroupId = reader.GetInt32("GroupId"),
                                    SubGroupId = reader.GetInt32("SubGroupId"),
                                    Status = reader["Status"] != DBNull.Value && reader.GetBoolean("Status"),
                                    PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                                    DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                                    Price = reader["Price"] == DBNull.Value ? 0 : reader.GetDecimal("Price"),
                                    Discount = reader["Discount"] == DBNull.Value ? 0 : reader.GetDouble("Discount"),
                                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return snapshots;
        }

        private StudentSubGroups GetStudentSubGroupSnapshot(int studentId, int groupId, int subGroupId)
        {
            try
            {
                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();
                    const string sql = @"SELECT Id, StudentId, GroupId, SubGroupId, Status, PaymentStatus, DateOfPayment, Price, Discount, UpdatedAt
                                         FROM StudentSubGroups
                                         WHERE StudentId = @sid AND GroupId = @gid AND SubGroupId = @subId
                                         ORDER BY Id DESC
                                         LIMIT 1";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@sid", studentId);
                        command.Parameters.AddWithValue("@gid", groupId);
                        command.Parameters.AddWithValue("@subId", subGroupId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new StudentSubGroups
                                {
                                    Id = reader.GetInt32("Id"),
                                    StudentId = reader.GetInt32("StudentId"),
                                    GroupId = reader.GetInt32("GroupId"),
                                    SubGroupId = reader.GetInt32("SubGroupId"),
                                    Status = reader["Status"] != DBNull.Value && reader.GetBoolean("Status"),
                                    PaymentStatus = reader["PaymentStatus"] == DBNull.Value ? null : reader.GetString("PaymentStatus"),
                                    DateOfPayment = reader["DateOfPayment"] == DBNull.Value ? (DateTime?)null : reader.GetDateTime("DateOfPayment"),
                                    Price = reader["Price"] == DBNull.Value ? 0 : reader.GetDecimal("Price"),
                                    Discount = reader["Discount"] == DBNull.Value ? 0 : reader.GetDouble("Discount"),
                                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                                };
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        #endregion
    }
}





