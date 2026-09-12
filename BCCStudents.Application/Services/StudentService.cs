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
                            }
                        }
                        _loggerRepository.LogStudentAction("Register", "Success", $"დარეგისტრირდა სტუდენტი: {student.FirstName} {student.LastName}", userId.ToString());
                        ret = studentId;
                        if (allSuccess)
                        {
                            transaction.Commit();
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
            // 1. წავშალოთ ჯგუფებთან კავშირი
            _studentRepository.RemoveStudentFromGroups(studentId);

            // 2. წავშალოთ სტუდენტი
            _studentRepository.DeleteStudent(studentId, userId);

            // 3. ჩავწეროთ ლოგი
            _loggerRepository.LogStudentAction("Delete", "Success", $"წაიშალა სტუდენტი ID: {studentId}", userId.ToString());
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
        }



        public void UpdateStudentGroupFields(StudentGroups original, StudentGroups updated)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. Student group update operation is blocked.");
            _studentRepository.UpdateStudentGroupFields(original, updated);
        }

        public bool UpdateStudentStatus(int studentId, int groupId, bool status)
        {
            return _studentRepository.UpdateStudentStatus(studentId, groupId, status);
        }

        /// <summary>
        /// Adds a student to a group with the specified status (default: 'Active').
        /// </summary>
        public void AddStudentToGroup(int studentId, int groupId, bool status = true, MySqlConnection externalConnection = null, MySqlTransaction externalTransaction = null)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. AddStudentToGroup operation is blocked.");
            if (IsStudentInGroup(studentId, groupId))
                return;

            var groupPrice = _groupRepository.GetGroupPrice(groupId);
            var discountPercent = ResolveStudentDiscountPercent(studentId, groupId);
            var finalPrice = ApplyDiscountToPrice(groupPrice, discountPercent);

            var studentGroup = new StudentGroups
            {
                StudentId = studentId,
                GroupId = groupId,
                PaymentStatus = "Pending",
                DateOfPayment = DateTime.Today.AddMonths(1),
                Price = finalPrice,
                Discount = discountPercent,
                Status = status
            };
            _studentGroupsService.AddStudentGroup(studentGroup, externalConnection, externalTransaction);
            _groupRepository.RecalculateStudentCount(groupId, externalConnection, externalTransaction);
        }

        // Overload used by UI when აქვს სრულ ველებს
        public void AddStudentToGroup(int studentId, int groupId, bool status, DateTime? dateOfPayment, string paymentStatus, decimal price, double discount)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. AddStudentToGroup operation is blocked.");
            if (IsStudentInGroup(studentId, groupId))
                return;

            var groupPrice = _groupRepository.GetGroupPrice(groupId);
            var discountPercent = discount > 0 ? discount : ResolveStudentDiscountPercent(studentId, groupId);
            var finalPrice = ApplyDiscountToPrice(groupPrice, discountPercent);

            var studentGroup = new StudentGroups
            {
                StudentId = studentId,
                GroupId = groupId,
                PaymentStatus = paymentStatus,
                DateOfPayment = dateOfPayment ?? DateTime.Today.AddMonths(1),
                Price = finalPrice,
                Discount = discountPercent,
                Status = status
            };
            _studentGroupsService.AddStudentGroup(studentGroup);
            _groupRepository.RecalculateStudentCount(groupId, null, null);
        }

        public void AddStudentToSubGroup(int studentId, int groupId, int subGroupId, string paymentStatus, DateTime? dateOfPayment, decimal price, double discount, bool status)
        {
            if (!_appStatus.IsDatabaseOnline)
                throw new InvalidOperationException("Database is offline. AddStudentToSubGroup operation is blocked.");

            var discountPercent = discount > 0 ? discount : ResolveStudentDiscountPercent(studentId, groupId);
            var finalPrice = ApplyDiscountToPrice(price, discountPercent);

            var studentSubGroup = new StudentSubGroups
            {
                StudentId = studentId,
                GroupId = groupId,
                SubGroupId = subGroupId,
                PaymentStatus = paymentStatus,
                DateOfPayment = dateOfPayment ?? DateTime.Today.AddMonths(1),
                Price = finalPrice,
                Discount = discountPercent,
                Status = status
            };
            int result = _studentSubGroupRepository.InsertStudentSubGroup(studentSubGroup);
            if (result > 0)
            {
                _subGroupRepository.IncrementSubGroupCount(subGroupId, null, null);
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
                }

                _subGroupRepository.IncrementSubGroupCount(newSubGroupId, null, null);
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
            // 1. ვიღებთ SubGroupIds-ს StudentCount-ის განახლებისთვის
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

            // 2. Soft delete: StudentGroups და StudentSubGroups (ლოკალურ ბაზაში Status=0, IsDeleted=1)
            _studentRepository.RemoveStudentFromGroup(studentId, groupId);

            // 3. Local recounts after soft-delete: Groups
            try
            {
                _groupRepository.RecalculateStudentCount(groupId, null, null);
            }
            catch { }

            // 4. Local recounts after soft-delete: SubGroups
            foreach (var subGroupId in subGroupIds)
            {
                try
                {
                    _subGroupRepository.DecreaseStudentCount(subGroupId);
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

        #region Discount resolution

        /// <summary>
        /// ფასდაკლების პროცენტის გამოთვლა აქტიური ჯგუფებიდან ან ისტორიიდან (ჯგუფის ცვლილებისას).
        /// </summary>
        private double ResolveStudentDiscountPercent(int studentId, int groupId)
        {
            var activeGroups = _studentGroupsService.GetActiveByStudentId(studentId);
            var activeMax = activeGroups
                .Where(g => g.Discount > 0)
                .Select(g => g.Discount)
                .DefaultIfEmpty(0)
                .Max();
            if (activeMax > 0)
                return activeMax;

            var latestForGroup = _studentGroupsService.GetLatestByStudentAndGroup(studentId, groupId);
            if (latestForGroup != null && latestForGroup.Discount > 0)
                return latestForGroup.Discount;

            var allGroups = _studentGroupsService.GetByStudentId(studentId);
            return allGroups
                .Where(g => g.Discount > 0)
                .Select(g => g.Discount)
                .DefaultIfEmpty(0)
                .Max();
        }

        private static decimal ApplyDiscountToPrice(decimal basePrice, double discountPercent)
        {
            if (discountPercent <= 0)
                return basePrice;
            return new DiscountCalculator(basePrice, (decimal)discountPercent).GetFinalAmount();
        }

        #endregion
    }
}





