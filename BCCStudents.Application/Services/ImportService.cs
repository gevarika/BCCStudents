using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using static BCCStudents.Domain.Entities.UserSession;
using BCCStudents.Application.Services.Sync.UpStream;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Application.Interfaces;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// სტუდენტების იმპორტის სერვისი Excel ფაილებიდან
    /// </summary>
    public class ImportService : IImportService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IStudentSubGroupRepository _studentSubGroupRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly StudentCodeGenerator _studentCodeGenerator;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        public ImportService(
            IStudentRepository studentRepository,
            IGroupRepository groupRepository,
            ISubGroupRepository subGroupRepository,
            IStudentGroupRepository studentGroupRepository,
            IStudentSubGroupRepository studentSubGroupRepository,
            IPaymentRepository paymentRepository,
            StudentCodeGenerator studentCodeGenerator,
            IDatabaseConnectionProvider connectionProvider,
            IUpStreamChangeTracker upStreamChangeTracker)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _subGroupRepository = subGroupRepository;
            _studentGroupRepository = studentGroupRepository;
            _studentSubGroupRepository = studentSubGroupRepository;
            _paymentRepository = paymentRepository;
            _studentCodeGenerator = studentCodeGenerator;
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _upStreamChangeTracker = upStreamChangeTracker;
        }

        #region ==================== Public Methods ====================

        /// <summary>
        /// Excel ფაილიდან სტუდენტების იმპორტი
        /// </summary>
        /// <param name="filePath">Excel ფაილის გზა</param>
        /// <param name="columnMappings">სვეტების მეპინგი (worksheet -> field -> column index)</param>
        /// <param name="sheetToGroupIdMap">Worksheet-ების მეპინგი ჯგუფებზე (worksheet name -> group id)</param>
        /// <param name="isActive">სტუდენტების აქტიური სტატუსი</param>
        /// <param name="progress">პროგრესის რეპორტირება</param>
        /// <param name="externalLogMessage">გარე ლოგირების callback</param>
        /// <returns>იმპორტის შედეგი</returns>
        public async Task<ImportResult> ImportStudentsAsync(
            string filePath,
            Dictionary<string, Dictionary<string, int>> columnMappings,
            Dictionary<string, int> sheetToGroupIdMap,
            bool isActive,
            IProgress<(int current, int total, string worksheet)> progress,
            Action<string> externalLogMessage = null)
        {
            var studentsToImport = new List<Student>();
            int totalRecords = 0;
            int currentProgress = 0;
            string currentWorksheet = string.Empty;
            int importedCount = 0;
            int multiGroupCount = 0;
            int duplicateCount = 0;

            var logMessages = new List<string>();
            void LogMessage(string message)
            {
                var logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
                logMessages.Add(logEntry);
                externalLogMessage?.Invoke(message);
            }

            try
            {
                LogMessage("იმპორტის პროცესი დაწყებულია");

                // ==================== ფაზა 1: Excel-ის წაკითხვა ====================
                LogMessage("Excel ფაილის კითხვა დაწყებულია");

                using (var workbook = new XLWorkbook(filePath))
                {
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var sheetName = worksheet.Name;

                        // შეამოწმე არის თუ არა worksheet მეპინგში
                        if (!sheetToGroupIdMap.ContainsKey(sheetName) || !columnMappings.ContainsKey(sheetName))
                        {
                            LogMessage($"Worksheet '{sheetName}' გამოტოვებულია (არ არის მეპინგში)");
                            continue;
                        }

                        currentWorksheet = sheetName;
                        var groupId = sheetToGroupIdMap[sheetName];
                        var mapping = columnMappings[sheetName];

                        LogMessage($"Worksheet '{sheetName}' დამუშავება დაწყებულია (Group ID: {groupId})");

                        // ქვეჯგუფების ამოღება ერთჯერადად
                        var subGroups = _subGroupRepository.GetSubGroupsByGroupId(groupId);

                        // მონაცემთა მწკრივების ამოღება (პირველი მწკრივი = header)
                        // გამოვიყენოთ RangeUsed() რომ ვნახოთ რა დიაპაზონია გამოყენებული
                        var usedRange = worksheet.RangeUsed();
                        var rows = new List<IXLRow>();
                        
                        if (usedRange != null)
                        {
                            // ყველა მწკრივის ამოღება გამოყენებული დიაპაზონიდან (პირველი მწკრივი = header, ამიტომ Skip(1))
                            int lastRowNumber = usedRange.LastRow().RowNumber();
                            for (int rowNum = 2; rowNum <= lastRowNumber; rowNum++)
                            {
                                var row = worksheet.Row(rowNum);
                                // შევამოწმოთ არის თუ არა მწკრივი ცარიელი (ყველა უჯრა ცარიელია)
                                if (row.CellsUsed().Any())
                                {
                                    rows.Add(row);
                                }
                            }
                        }
                        else
                        {
                            // თუ RangeUsed() null-ია, გამოვიყენოთ RowsUsed() როგორც fallback
                            rows = worksheet.RowsUsed().Skip(1).ToList();
                        }
                        
                        totalRecords += rows.Count;

                        int excelProcessedCount = 0;
                        int excelSkippedCount = 0;
                        int excelErrorCount = 0;

                        foreach (var row in rows)
                        {
                            try
                            {
                                var student = new Student();

                                // Excel-ის მონაცემების წაკითხვა
                                foreach (var field in mapping.Keys)
                                {
                                    int colIdx = mapping[field];
                                    string value = row.Cell(colIdx + 1).GetString();

                                    switch (field)
                                    {
                                        case "StudentCode":
                                            student.StudentCode = string.IsNullOrWhiteSpace(value)
                                                ? _studentCodeGenerator.GenerateStudentCode()
                                                : value;
                                            break;
                                        case "FirstName":
                                            student.FirstName = value;
                                            break;
                                        case "LastName":
                                            student.LastName = value;
                                            break;
                                        case "Age":
                                            student.Age = int.TryParse(value, out int age) ? age : 0;
                                            break;
                                        case "ParentName":
                                            student.ParentName = value;
                                            break;
                                        case "PhoneNumber":
                                            student.PhoneNumber = value;
                                            break;
                                        case "Id_Numb":
                                            student.Id_Numb = long.TryParse(value, out long idn) ? idn : 0;
                                            break;
                                        case "Address":
                                            student.Address = value;
                                            break;
                                        case "RegistrationDate":
                                            student.RegistrationDate = DateTime.TryParse(value, out DateTime dtr)
                                                ? dtr
                                                : DateTime.Now;
                                            break;
                                        case "DateOfPayment":
                                        case "PaymentDate":
                                            if (!string.IsNullOrWhiteSpace(value))
                                            {
                                                DateTime dt;
                                                if (DateTime.TryParseExact(value,
                                                        new[] { "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" },
                                                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                                    || DateTime.TryParse(value, out dt))
                                                {
                                                    student.DateOfPayment = dt;
                                                }
                                                else if (double.TryParse(value, NumberStyles.Any,
                                                             CultureInfo.InvariantCulture, out double oa))
                                                {
                                                    student.DateOfPayment = DateTime.FromOADate(oa);
                                                }
                                            }
                                            break;
                                        case "Discount":
                                            student.Discount = double.TryParse(value, out double d) ? d : 0;
                                            break;
                                        case "TuitionFee":
                                        case "Price":
                                            student.TuitionFee = decimal.TryParse(value, out decimal tuitionFee)
                                                ? tuitionFee
                                                : 0;
                                            break;
                                        case "SubGroup":
                                            int subGroupId = 0;
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                subGroupId = subGroups.Any() ? subGroups.First().Id : 0;
                                            }
                                            else if (int.TryParse(value, out int sgNum))
                                            {
                                                var match = subGroups.FirstOrDefault(sg => sg.Name == $"კლასი {sgNum}");
                                                if (match != null)
                                                    subGroupId = match.Id;
                                                else if (subGroups.Count >= sgNum && sgNum > 0)
                                                    subGroupId = subGroups.OrderBy(sg => sg.Id).ElementAt(sgNum - 1).Id;
                                                else
                                                    subGroupId = subGroups.Any() ? subGroups.First().Id : 0;
                                            }
                                            else
                                            {
                                                var match = subGroups.FirstOrDefault(sg => sg.Name == value);
                                                subGroupId = match != null
                                                    ? match.Id
                                                    : (subGroups.Any() ? subGroups.First().Id : 0);
                                            }
                                            student.SubGroup = subGroupId;
                                            break;
                                    }
                                }

                                // ვალიდაცია: FirstName და LastName აუცილებელია
                                if (string.IsNullOrWhiteSpace(student.FirstName) ||
                                    string.IsNullOrWhiteSpace(student.LastName))
                                {
                                    excelSkippedCount++;
                                    LogMessage($"გამოტოვებულია მწკრივი {row.RowNumber()}: FirstName ან LastName ცარიელია (FirstName='{student.FirstName}', LastName='{student.LastName}')");
                                    continue;
                                }

                                // დამატებითი ველების დაყენება
                                student.Status = isActive;
                                if (!student.DateOfPayment.HasValue)
                                {
                                    var studyStartDate = StudyStartDateManager.GetStudyStartDate();
                                    if (studyStartDate.HasValue)
                                    {
                                        student.DateOfPayment = studyStartDate.Value.AddMonths(1);
                                    }
                                }
                                if (student.RegistrationDate == default(DateTime))
                                    student.RegistrationDate = DateTime.Now;
                                student.User_Id = UserSession.Id;
                                student.GroupId = groupId;

                                // თუ TuitionFee არ არის, აიღე Group-ის Price
                                if (student.TuitionFee == 0)
                                {
                                    var group = _groupRepository.GetGroupById(groupId);
                                    if (group != null)
                                    {
                                        student.TuitionFee = group.Price;
                                    }
                                }

                                studentsToImport.Add(student);
                                excelProcessedCount++;
                                currentProgress++;
                                progress?.Report((currentProgress, totalRecords * 2, currentWorksheet));
                            }
                            catch (Exception ex)
                            {
                                excelErrorCount++;
                                LogMessage($"შეცდომა მწკრივზე {row.RowNumber()}: {ex.Message}");
                                continue;
                            }
                        }

                        LogMessage(
                            $"Worksheet '{sheetName}': სულ {rows.Count} მწკრივი, დამუშავებული {excelProcessedCount}, გამოტოვებული {excelSkippedCount}, შეცდომები {excelErrorCount}");
                    }
                }

                LogMessage(
                    $"Excel ფაილის კითხვა დასრულდა: სულ {totalRecords} მწკრივი, სიაში დამატებული {studentsToImport.Count} სტუდენტი");

                // ==================== ფაზა 2: ბაზაში ჩასმა ====================
                LogMessage("ბაზის ფაზა დაწყებულია");

                var existingStudents = _studentRepository.GetStudentsForImport();
                var existingStudentGroups = _studentGroupRepository.GetAllActiveStudentGroupPairs();

                // მკაცრი დუბლიკატის ფილტრი: FirstName, LastName, Id_Numb, Address
                Func<string, string> norm = s => (s ?? string.Empty).Trim().ToLowerInvariant();
                var strictToId = existingStudents
                    .GroupBy(s => (norm(s.FirstName), norm(s.LastName), s.IdNumb, norm(s.Address)))
                    .ToDictionary(g => g.Key, g => g.First().Id);
                var studentGroupSet = new HashSet<(int StudentId, int GroupId)>(existingStudentGroups);

                // ბაზაში არსებული StudentCode-ების HashSet
                var existingCodes = new HashSet<string>(existingStudents
                    .Where(s => !string.IsNullOrWhiteSpace(s.StudentCode))
                    .Select(s => s.StudentCode.Trim()),
                    StringComparer.OrdinalIgnoreCase);

                int totalStudents = studentsToImport.Count;
                int dbProgress = 0;
                var pendingCodes = new HashSet<string>();
                var importedStudentIds = new List<int>(); // დამატებული სტუდენტების ID-ები სერვერზე ატვირთვისთვის
                var importedStudentGroupIds = new List<int>(); // დამატებული StudentGroups-ის ID-ები
                var importedStudentSubGroupIds = new List<int>(); // დამატებული StudentSubGroups-ის ID-ები
                var updatedGroupIds = new HashSet<int>(); // განახლებული Groups-ის ID-ები
                var updatedSubGroupIds = new HashSet<int>(); // განახლებული SubGroups-ის ID-ები

                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // existingCodes აღარ გვჭირდება, რადგან GenerateStudentCode() თვითონ ამოწმებს ბაზაში
                        for (int i = 0; i < studentsToImport.Count; i++)
                        {
                            try
                            {
                                var student = studentsToImport[i];
                                var code = student.StudentCode?.Trim();

                                // StudentCode-ის გენერაცია თუ არ არის ან დუბლიკატია იმპორტში ან ბაზაში
                                if (string.IsNullOrWhiteSpace(student.StudentCode))
                                {
                                    // გენერაცია უნიკალური StudentCode, რომელიც არ არსებობს ბაზაში ან იმპორტში
                                    student.StudentCode = _studentCodeGenerator.GenerateUniqueStudentCode(existingCodes, pendingCodes);
                                    // დამატება pendingCodes-ში, რომ არ განმეორდეს იმპორტში
                                    pendingCodes.Add(student.StudentCode);
                                }
                                else
                                {
                                    var studentCode = student.StudentCode.Trim();
                                    
                                    // შემოწმება: არის თუ არა StudentCode დუბლიკატი იმპორტში ან ბაზაში
                                    if (!pendingCodes.Add(studentCode) || existingCodes.Contains(studentCode))
                                    {
                                        // თუ StudentCode დუბლიკატია იმპორტში ან ბაზაში, გენერირება ახალი უნიკალური
                                        LogMessage($"დუბლიკატი StudentCode: {studentCode} (სტუდენტი: {student.FirstName} {student.LastName}) - გენერირდება ახალი უნიკალური");
                                        student.StudentCode = _studentCodeGenerator.GenerateUniqueStudentCode(existingCodes, pendingCodes);
                                        // დამატება pendingCodes-ში
                                        pendingCodes.Add(student.StudentCode);
                                    }
                                    else
                                    {
                                        // StudentCode უნიკალურია, დამატება pendingCodes-ში
                                        pendingCodes.Add(studentCode);
                                    }
                                }

                                int? studentId = null;
                                var strictKey = (norm(student.FirstName), norm(student.LastName), student.Id_Numb,
                                    norm(student.Address));
                                if (strictToId.TryGetValue(strictKey, out int foundStrict))
                                {
                                    studentId = foundStrict;
                                }

                                if (studentId == null)
                                {
                                    // ახალი სტუდენტის დამატება
                                    studentId = _studentRepository.InsertStudent(student, connection, transaction);
                                    strictToId[strictKey] = studentId.Value;

                                    // StudentGroups-ში დამატება
                                    var sg = new StudentGroups
                                    {
                                        StudentId = studentId.Value,
                                        GroupId = student.GroupId,
                                        Status = isActive,
                                        DateOfPayment = student.DateOfPayment ?? DateTime.Today.AddMonths(1),
                                        PaymentStatus = "Pending",
                                        Price = student.TuitionFee,
                                        Discount = student.Discount
                                    };
                                    var studentGroupId = _studentGroupRepository.InsertStudentGroup(sg, connection, transaction);
                                    studentGroupSet.Add((studentId.Value, student.GroupId));
                                    importedStudentGroupIds.Add(studentGroupId); // დამატება სიაში სერვერზე ატვირთვისთვის
                                    if (!updatedGroupIds.Contains(student.GroupId))
                                    {
                                        updatedGroupIds.Add(student.GroupId); // ჯგუფის StudentCount განახლდა
                                    }
                                    _groupRepository.RecalculateStudentCount(student.GroupId, connection, transaction);
                                    LogMessage($"Group ID {student.GroupId} StudentCount განახლდა ლოკალურ ბაზაში");

                                    // StudentSubGroups-ში დამატება
                                    var subGroup = _subGroupRepository.GetSubGroupById(student.SubGroup);
                                    if (subGroup != null)
                                    {
                                        var ssg = new StudentSubGroups
                                        {
                                            StudentId = studentId.Value,
                                            GroupId = subGroup.GroupId,
                                            SubGroupId = subGroup.Id,
                                            PaymentStatus = "Pending",
                                            DateOfPayment = student.DateOfPayment ?? DateTime.Today.AddMonths(1),
                                            Price = student.TuitionFee,
                                            Status = isActive
                                        };
                                        var studentSubGroupId = _studentSubGroupRepository.InsertStudentSubGroup(ssg, connection, transaction);
                                        importedStudentSubGroupIds.Add(studentSubGroupId); // დამატება სიაში სერვერზე ატვირთვისთვის
                                        if (!updatedSubGroupIds.Contains(subGroup.Id))
                                        {
                                            updatedSubGroupIds.Add(subGroup.Id); // ქვეჯგუფის StudentCount განახლდა
                                        }
                                        _subGroupRepository.IncrementSubGroupCount(subGroup.Id, connection, transaction);
                                        LogMessage($"SubGroup ID {subGroup.Id} StudentCount განახლდა ლოკალურ ბაზაში");
                                    }
                                    importedCount++;
                                    importedStudentIds.Add(studentId.Value); // დამატება სიაში სერვერზე ატვირთვისთვის
                                }
                                else
                                {
                                    // არსებული სტუდენტის დამატება ჯგუფში
                                    if (!studentGroupSet.Contains((studentId.Value, student.GroupId)))
                                    {
                                        var sg = new StudentGroups
                                        {
                                            StudentId = studentId.Value,
                                            GroupId = student.GroupId,
                                            Status = isActive,
                                            DateOfPayment = student.DateOfPayment ?? DateTime.Today.AddMonths(1),
                                            PaymentStatus = "Pending",
                                            Price = student.TuitionFee,
                                            Discount = student.Discount
                                        };
                                        var studentGroupId = _studentGroupRepository.InsertStudentGroup(sg, connection, transaction);
                                        studentGroupSet.Add((studentId.Value, student.GroupId));
                                        importedStudentGroupIds.Add(studentGroupId); // დამატება სიაში სერვერზე ატვირთვისთვის
                                        if (!updatedGroupIds.Contains(student.GroupId))
                                        {
                                            updatedGroupIds.Add(student.GroupId); // ჯგუფის StudentCount განახლდა
                                        }
                                        _groupRepository.RecalculateStudentCount(student.GroupId, connection,
                                            transaction);
                                        LogMessage($"Group ID {student.GroupId} StudentCount განახლდა ლოკალურ ბაზაში");

                                        var subGroup = _subGroupRepository.GetSubGroupById(student.SubGroup);
                                        if (subGroup != null)
                                        {
                                            var ssg = new StudentSubGroups
                                            {
                                                StudentId = studentId.Value,
                                                GroupId = subGroup.GroupId,
                                                SubGroupId = subGroup.Id,
                                                PaymentStatus = "Pending",
                                                DateOfPayment = student.DateOfPayment ?? DateTime.Today.AddMonths(1),
                                                Price = student.TuitionFee,
                                                Status = isActive
                                            };
                                            var studentSubGroupId = _studentSubGroupRepository.InsertStudentSubGroup(ssg, connection,
                                                transaction);
                                            importedStudentSubGroupIds.Add(studentSubGroupId); // დამატება სიაში სერვერზე ატვირთვისთვის
                                            if (!updatedSubGroupIds.Contains(subGroup.Id))
                                            {
                                                updatedSubGroupIds.Add(subGroup.Id); // ქვეჯგუფის StudentCount განახლდა
                                            }
                                            _subGroupRepository.IncrementSubGroupCount(subGroup.Id, connection,
                                                transaction);
                                            LogMessage($"SubGroup ID {subGroup.Id} StudentCount განახლდა ლოკალურ ბაზაში");
                                        }
                                        importedCount++;
                                        multiGroupCount++;
                                    }
                                    else
                                    {
                                        duplicateCount++;
                                    }
                                }

                                dbProgress++;
                                progress?.Report((totalRecords + dbProgress, totalRecords * 2, currentWorksheet));
                            }
                            catch (Exception ex)
                            {
                                duplicateCount++;
                                LogMessage($"შეცდომა ბაზაში ჩასმისას სტუდენტისთვის '{studentsToImport[i].FirstName} {studentsToImport[i].LastName}': {ex.Message}");
                                if (ex.InnerException != null)
                                {
                                    LogMessage($"  Inner Exception: {ex.InnerException.Message}");
                                }
                                dbProgress++;
                                progress?.Report((totalRecords + dbProgress, totalRecords * 2, currentWorksheet));
                                continue;
                            }
                        }

                        transaction.Commit();
                        LogMessage($"Transaction committed successfully");
                    }
                }

                // სერვერზე ატვირთვა დამატებული სტუდენტებისთვის
                if (importedStudentIds.Count > 0)
                {
                    LogMessage($"სერვერზე ატვირთვა დაწყებულია: {importedStudentIds.Count} სტუდენტი");
                    int syncSuccessCount = 0;
                    int syncErrorCount = 0;

                    foreach (var studentId in importedStudentIds)
                    {
                        try
                        {
                            // სტუდენტის სრული მონაცემების მიღება
                            var student = _studentRepository.GetStudentById(studentId);
                            if (student != null)
                            {
                                // სერვერზე ატვირთვა
                                await _upStreamChangeTracker.TrackStudentChangeAsync(studentId, SyncOperationType.Insert, student);
                                syncSuccessCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            syncErrorCount++;
                            LogMessage($"შეცდომა სტუდენტის სერვერზე ატვირთვისას (ID: {studentId}): {ex.Message}");
                        }
                    }

                    LogMessage($"Students სერვერზე ატვირთვა დასრულდა: წარმატებული {syncSuccessCount}, შეცდომები {syncErrorCount}");
                }

                // სერვერზე ატვირთვა StudentGroups-ისთვის
                if (importedStudentGroupIds.Count > 0)
                {
                    LogMessage($"StudentGroups სერვერზე ატვირთვა დაწყებულია: {importedStudentGroupIds.Count} ჩანაწერი");
                    int syncSuccessCount = 0;
                    int syncErrorCount = 0;

                    foreach (var studentGroupId in importedStudentGroupIds)
                    {
                        try
                        {
                            // StudentGroup-ის მონაცემების მიღება
                            var studentGroup = _studentGroupRepository.GetById(studentGroupId);
                            if (studentGroup != null)
                            {
                                // სერვერზე ატვირთვა
                                await _upStreamChangeTracker.TrackStudentGroupChangeAsync(studentGroupId, SyncOperationType.Insert, studentGroup);
                                syncSuccessCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            syncErrorCount++;
                            LogMessage($"შეცდომა StudentGroup-ის სერვერზე ატვირთვისას (ID: {studentGroupId}): {ex.Message}");
                        }
                    }

                    LogMessage($"StudentGroups სერვერზე ატვირთვა დასრულდა: წარმატებული {syncSuccessCount}, შეცდომები {syncErrorCount}");
                }

                // სერვერზე ატვირთვა StudentSubGroups-ისთვის
                if (importedStudentSubGroupIds.Count > 0)
                {
                    LogMessage($"StudentSubGroups სერვერზე ატვირთვა დაწყებულია: {importedStudentSubGroupIds.Count} ჩანაწერი");
                    int syncSuccessCount = 0;
                    int syncErrorCount = 0;

                    foreach (var studentSubGroupId in importedStudentSubGroupIds)
                    {
                        try
                        {
                            // StudentSubGroup-ის მონაცემების მიღება
                            var studentSubGroup = _studentSubGroupRepository.GetById(studentSubGroupId);
                            if (studentSubGroup != null)
                            {
                                // სერვერზე ატვირთვა
                                await _upStreamChangeTracker.TrackStudentSubGroupChangeAsync(studentSubGroupId, SyncOperationType.Insert, studentSubGroup);
                                syncSuccessCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            syncErrorCount++;
                            LogMessage($"შეცდომა StudentSubGroup-ის სერვერზე ატვირთვისას (ID: {studentSubGroupId}): {ex.Message}");
                        }
                    }

                    LogMessage($"StudentSubGroups სერვერზე ატვირთვა დასრულდა: წარმატებული {syncSuccessCount}, შეცდომები {syncErrorCount}");
                }

                // სერვერზე ატვირთვა Groups-ისთვის (StudentCount-ის განახლება)
                if (updatedGroupIds.Count > 0)
                {
                    LogMessage($"Groups სერვერზე ატვირთვა დაწყებულია: {updatedGroupIds.Count} ჯგუფი");
                    int syncSuccessCount = 0;
                    int syncErrorCount = 0;

                    foreach (var groupId in updatedGroupIds)
                    {
                        try
                        {
                            // ჯგუფის სრული მონაცემების მიღება
                            var group = _groupRepository.GetGroupById(groupId);
                            if (group != null)
                            {
                                // სერვერზე ატვირთვა (UPDATE - StudentCount განახლდა)
                                await _upStreamChangeTracker.TrackGroupChangeAsync(groupId, SyncOperationType.Update, group);
                                syncSuccessCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            syncErrorCount++;
                            LogMessage($"შეცდომა Group-ის სერვერზე ატვირთვისას (ID: {groupId}): {ex.Message}");
                        }
                    }

                    LogMessage($"Groups სერვერზე ატვირთვა დასრულდა: წარმატებული {syncSuccessCount}, შეცდომები {syncErrorCount}");
                }

                // სერვერზე ატვირთვა SubGroups-ისთვის (StudentCount-ის განახლება)
                if (updatedSubGroupIds.Count > 0)
                {
                    LogMessage($"SubGroups სერვერზე ატვირთვა დაწყებულია: {updatedSubGroupIds.Count} ქვეჯგუფი");
                    int syncSuccessCount = 0;
                    int syncErrorCount = 0;

                    foreach (var subGroupId in updatedSubGroupIds)
                    {
                        try
                        {
                            // ქვეჯგუფის სრული მონაცემების მიღება
                            var subGroup = _subGroupRepository.GetSubGroupById(subGroupId);
                            if (subGroup != null)
                            {
                                // სერვერზე ატვირთვა (UPDATE - StudentCount განახლდა)
                                await _upStreamChangeTracker.TrackSubGroupChangeAsync(subGroupId, SyncOperationType.Update, subGroup);
                                syncSuccessCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            syncErrorCount++;
                            LogMessage($"შეცდომა SubGroup-ის სერვერზე ატვირთვისას (ID: {subGroupId}): {ex.Message}");
                        }
                    }

                    LogMessage($"SubGroups სერვერზე ატვირთვა დასრულდა: წარმატებული {syncSuccessCount}, შეცდომები {syncErrorCount}");
                }

                progress?.Report((totalRecords * 2, totalRecords * 2, currentWorksheet));

                LogMessage($"=== იმპორტის შეჯამება ===");
                LogMessage($"Excel-იდან სულ მწკრივები: {totalRecords}");
                LogMessage($"სიაში დამატებული სტუდენტები: {studentsToImport.Count}");
                LogMessage($"ბაზაში იმპორტირებული: {importedCount}");
                LogMessage($"დუბლიკატები: {duplicateCount}");
                LogMessage($"მრავალჯგუფიანი: {multiGroupCount}");

                return ImportResult.CreateSuccess(importedCount, duplicateCount);
            }
            catch (Exception ex)
            {
                LogMessage($"შეცდომა იმპორტისას: {ex.Message}");
                LogMessage($"Stack Trace: {ex.StackTrace}");

                // ლოგის შენახვა ფაილში
                try
                {
                    var logDirectory = Path.Combine(System.Windows.Forms.Application.StartupPath, "ImportLogs");
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }

                    var logFileName = Path.Combine(logDirectory,
                        $"ImportServiceLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    var logContent = string.Join("\n", logMessages);
                    File.WriteAllText(logFileName, logContent);
                }
                catch
                {
                    // იგნორირება
                }

                throw;
            }
        }

        #endregion

        #region ==================== Other Methods ====================

        public bool UpdateStudentSubGroupPaymentStatus(int studentId, int groupId, int subGroupId, string status)
        {
            return _subGroupRepository.UpdateStudentSubGroupPaymentStatus(studentId, groupId, subGroupId, status);
        }

        public List<Payment> GetFailedPayments()
        {
            return _paymentRepository.GetFailedPayments().Select(fp => new Payment
            {
                PaymentDate = fp.PaymentDate,
                Amount = fp.Amount,
                PersonalId = fp.PersonalId,
                Description = fp.Description,
                FailureReason = fp.Reason,
                CreatedAt = fp.CreatedAt,
                PaymentStatus = "Failed",
                RequiresReview = true
            }).ToList();
        }

        public void DeleteFailedPayment(DateTime paymentDate, decimal amount, long? personalId, string description)
        {
            _paymentRepository.DeleteFailedPayment(paymentDate, amount, personalId, description);
        }

        #endregion
    }
}




