using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;

namespace BCCStudents.Application.Services
{
    /// <summary>
    /// სტუდენტების იმპორტის სერვისი - Refactored Version
    /// </summary>
    public class ImportServiceV2 : IImportService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupService _groupService;
        private readonly ISubGroupRepository _subGroupRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IStudentSubGroupRepository _studentSubGroupRepository;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IStudentCodeGenerator _studentCodeGenerator;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;
        private readonly ISystemConfigurationService _systemConfigService;

        public ImportServiceV2(
            IStudentRepository studentRepository,
            IGroupService groupService,
            ISubGroupRepository subGroupRepository,
            IStudentGroupRepository studentGroupRepository,
            IStudentSubGroupRepository studentSubGroupRepository,
            IDatabaseConnectionProvider connectionProvider,
            IStudentCodeGenerator studentCodeGenerator,
            IUpStreamChangeTracker upStreamChangeTracker,
            ISystemConfigurationService systemConfigService)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
            _subGroupRepository = subGroupRepository ?? throw new ArgumentNullException(nameof(subGroupRepository));
            _studentGroupRepository = studentGroupRepository ?? throw new ArgumentNullException(nameof(studentGroupRepository));
            _studentSubGroupRepository = studentSubGroupRepository ?? throw new ArgumentNullException(nameof(studentSubGroupRepository));
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _studentCodeGenerator = studentCodeGenerator ?? throw new ArgumentNullException(nameof(studentCodeGenerator));
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));
            _systemConfigService = systemConfigService ?? throw new ArgumentNullException(nameof(systemConfigService));
        }

        /// <summary>
        /// იღებს default გადახდის თარიღს სისტემური კონფიგურაციიდან
        /// თუ არ არის დაყენებული, აბრუნებს null
        /// </summary>
        private DateTime? GetDefaultPaymentDate()
        {
            return _systemConfigService.GetDefaultPaymentDate();
        }

        /// <summary>
        /// ამოწმებს დაყენებულია თუ არა default გადახდის თარიღი
        /// </summary>
        private bool IsDefaultPaymentDateConfigured()
        {
            return _systemConfigService.GetDefaultPaymentDate().HasValue;
        }

        public async Task<List<string>> GetExcelSheetsAsync(Stream fileStream)
        {
            return await Task.Run(() =>
            {
                try
                {
                    fileStream.Position = 0;
                    using (var workbook = new XLWorkbook(fileStream))
                    {
                        return workbook.Worksheets.Select(w => w.Name).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Excel ფაილის კითხვა ვერ მოხერხდა: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<string>> GetSheetHeadersAsync(Stream fileStream, string sheetName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    fileStream.Position = 0;
                    using (var workbook = new XLWorkbook(fileStream))
                    {
                        var worksheet = workbook.Worksheet(sheetName);
                        if (worksheet == null)
                            throw new ArgumentException($"Sheet '{sheetName}' არ მოიძებნა Excel ფაილში");

                        var headers = new List<string>();
                        foreach (var cell in worksheet.FirstRow().CellsUsed())
                        {
                            var headerName = cell.GetString();
                            if (!string.IsNullOrWhiteSpace(headerName))
                            {
                                headers.Add(headerName);
                            }
                        }
                        return headers;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Sheet '{sheetName}' header-ების კითხვა ვერ მოხერხდა: {ex.Message}", ex);
                }
            });
        }

        public async Task<DataTable> GetSheetPreviewAsync(Stream fileStream, string sheetName, int maxRows = 100)
        {
            return await Task.Run(() =>
            {
                try
                {
                    fileStream.Position = 0;
                    using (var workbook = new XLWorkbook(fileStream))
                    {
                        var worksheet = workbook.Worksheet(sheetName);
                        if (worksheet == null)
                            throw new ArgumentException($"Sheet '{sheetName}' არ მოიძებნა Excel ფაილში");

                        var dataTable = new DataTable();

                        // Header-ების წაკითხვა
                        var headerRow = worksheet.FirstRow();
                        var headerColumnIndexes = new Dictionary<int, int>(); // Excel column index -> DataTable column index
                        int dataTableColIndex = 0;

                        foreach (var headerCell in headerRow.CellsUsed())
                        {
                            var headerName = headerCell.GetString();
                            if (!string.IsNullOrWhiteSpace(headerName))
                            {
                                dataTable.Columns.Add(headerName);
                                headerColumnIndexes[headerCell.Address.ColumnNumber] = dataTableColIndex++;
                            }
                        }

                        // პრევიუ მწკრივების წაკითხვა
                        dataTable.BeginLoadData();
                        try
                        {
                            var rows = worksheet.RowsUsed().Skip(1).Take(maxRows);
                            foreach (var row in rows)
                            {
                                var dataRow = dataTable.NewRow();
                                foreach (var kvp in headerColumnIndexes)
                                {
                                    var excelColumnIndex = kvp.Key;
                                    var dataTableColumnIndex = kvp.Value;
                                    var cell = row.Cell(excelColumnIndex);
                                    dataRow[dataTableColumnIndex] = cell != null && !cell.IsEmpty() ? cell.GetString() : string.Empty;
                                }
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                        finally
                        {
                            dataTable.EndLoadData();
                        }

                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Sheet '{sheetName}' პრევიუს კითხვა ვერ მოხერხდა: {ex.Message}", ex);
                }
            });
        }

        public async Task<ImportResult> ImportStudentsAsync(
            Stream fileStream,
            ImportMappingConfiguration configuration,
            IProgress<(int current, int total, string status)> progress = null)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (!configuration.Validate())
                return ImportResult.CreateError("მეპინგის კონფიგურაცია არავალიდურია");

            // შემოწმება: დაყენებულია თუ არა default გადახდის თარიღი
            if (!IsDefaultPaymentDateConfigured())
            {
                return ImportResult.CreateError("გადახდის თარიღი არ არის დაყენებული. აუცილებელია დაყენება.");
            }

            var errors = new List<string>();
            var importedCount = 0;
            var duplicateCount = 0;
            var multiGroupCount = 0;

            try
            {
                fileStream.Position = 0;

                // Excel-ის წაკითხვა და სტუდენტების პრეპარირება
                var studentsToImport = new List<(Student student, int groupId)>();
                int totalRecords = 0;

                using (var workbook = new XLWorkbook(fileStream))
                {
                    foreach (var sheetMapping in configuration.SheetToGroupMapping)
                    {
                        var sheetName = sheetMapping.Key;
                        var groupId = sheetMapping.Value;

                        var worksheet = workbook.Worksheet(sheetName);
                        if (worksheet == null)
                        {
                            var errorMsg = $"Sheet '{sheetName}' არ მოიძებნა Excel ფაილში";
                            errors.Add(errorMsg);
                            continue;
                        }

                        progress?.Report((0, 100, $"Sheet '{sheetName}'-ის კითხვა..."));

                        // Header-ების ინდექსების მიღება
                        var headerColumnIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                        foreach (var cell in worksheet.FirstRow().CellsUsed())
                        {
                            var headerName = cell.GetString();
                            if (!string.IsNullOrWhiteSpace(headerName))
                            {
                                headerColumnIndexes[headerName] = cell.Address.ColumnNumber;
                            }
                        }

                        // მონაცემთა მწკრივების წაკითხვა
                        var rows = worksheet.RowsUsed().Skip(1).ToList();
                        totalRecords += rows.Count;

                        // Get column mapping for this specific sheet
                        var sheetColumnMapping = configuration.GetColumnMappingForSheet(sheetName);

                        foreach (var row in rows)
                        {
                            Student student = null;
                            try
                            {
                                student = MapRowToStudent(row, headerColumnIndexes, sheetColumnMapping, groupId, configuration.IsActive);

                                // ვალიდაცია: FirstName და LastName აუცილებელია
                                if (string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName))
                                {
                                    var errorMsg = $"FirstName ან LastName ცარიელია";
                                    errors.Add($"მწკრივი {row.RowNumber()}: {errorMsg}");

                                    // შევინახოთ failed import
                                    SaveFailedImport(sheetName, row.RowNumber(), student, groupId, errorMsg, null);
                                    continue;
                                }

                                studentsToImport.Add((student, groupId));
                            }
                            catch (Exception ex)
                            {
                                var errorMsg = ex.Message;
                                errors.Add($"მწკრივი {row.RowNumber()}: {errorMsg}");

                                // შევინახოთ failed import
                                SaveFailedImport(sheetName, row.RowNumber(), student, groupId, errorMsg, null);
                            }
                        }
                    }
                }

                progress?.Report((0, 100, $"ბაზაში ჩასმა: {studentsToImport.Count} სტუდენტი..."));

                // ბაზაში ჩასმა
                var result = await ImportStudentsToDatabaseAsync(studentsToImport, progress, totalRecords, errors);
                result.Errors = errors;

                if (errors.Any())
                {
                    if (string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        result.ErrorMessage = $"იმპორტი დასრულდა შეცდომებით. შეცდომების რაოდენობა: {errors.Count}";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return ImportResult.CreateError($"იმპორტის შეცდომა: {ex.Message}");
            }
        }

        private Student MapRowToStudent(
            IXLRow row,
            Dictionary<string, int> headerColumnIndexes,
            Dictionary<string, string> columnMapping,
            int groupId,
            bool isActive)
        {
            var student = new Student
            {
                GroupId = groupId,
                Status = isActive,
                RegistrationDate = DateTime.Now,
                User_Id = UserSession.Id,
                DateOfPayment = GetDefaultPaymentDate() ?? throw new InvalidOperationException("Default payment date is not configured") // Default value from system configuration
            };

            foreach (var mapping in columnMapping)
            {
                var excelColumnName = mapping.Key;
                var studentPropertyName = mapping.Value;

                if (!headerColumnIndexes.TryGetValue(excelColumnName, out int columnIndex))
                    continue;

                var cellValue = row.Cell(columnIndex).GetString();
                SetStudentProperty(student, studentPropertyName, cellValue);
            }

            // Set default TuitionFee from Group if not set
            if (student.TuitionFee == 0)
            {
                try
                {
                    var group = _groupService.GetGroupById(groupId);
                    if (group != null)
                    {
                        student.TuitionFee = group.Price;
                    }
                }
                catch
                {
                    // Ignore errors
                }
            }

            return student;
        }

        private void SetStudentProperty(Student student, string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(Student.StudentCode):
                    student.StudentCode = string.IsNullOrWhiteSpace(value)
                        ? _studentCodeGenerator.GenerateStudentCode()
                        : value;
                    break;
                case nameof(Student.FirstName):
                    student.FirstName = value;
                    break;
                case nameof(Student.LastName):
                    student.LastName = value;
                    break;
                case nameof(Student.Age):
                    student.Age = int.TryParse(value, out int age) ? age : 0;
                    break;
                case nameof(Student.ParentName):
                    student.ParentName = value;
                    break;
                case nameof(Student.PhoneNumber):
                    student.PhoneNumber = value;
                    break;
                case nameof(Student.Id_Numb):
                    student.Id_Numb = long.TryParse(value, out long idn) ? idn : 0;
                    break;
                case nameof(Student.Address):
                    student.Address = value;
                    break;
                case nameof(Student.TuitionFee):
                    student.TuitionFee = decimal.TryParse(value, out decimal fee) ? fee : 0;
                    break;
                case nameof(Student.Discount):
                    student.Discount = double.TryParse(value, out double discount) ? discount : 0;
                    break;
                case nameof(Student.DateOfPayment):
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
                case nameof(Student.RegistrationDate):
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (DateTime.TryParse(value, out DateTime regDate))
                        {
                            student.RegistrationDate = regDate;
                        }
                    }
                    break;
                case "SubGroup":
                    if (int.TryParse(value, out int subGroupId))
                    {
                        student.SubGroup = subGroupId;
                    }
                    break;
            }
        }

        private async Task<ImportResult> ImportStudentsToDatabaseAsync(
            List<(Student student, int groupId)> studentsToImport,
            IProgress<(int current, int total, string status)> progress,
            int totalRecords,
            List<string> errors)
        {
            return await Task.Run(() =>
            {
                var importedCount = 0;
                var duplicateCount = 0;
                var multiGroupCount = 0;

                try
                {
                    // არსებული სტუდენტების მიღება duplicate checking-ისთვის
                    var existingStudents = _studentRepository.GetStudentsForImport();
                    var existingStudentGroups = _studentGroupRepository.GetAllActiveStudentGroupPairs();

                    // მკაცრი დუბლიკატის ფილტრი: FirstName, LastName, Id_Numb, Address
                    Func<string, string> norm = s => (s ?? string.Empty).Trim().ToLowerInvariant();
                    var strictToId = existingStudents
                        .GroupBy(s => (norm(s.FirstName), norm(s.LastName), s.IdNumb, norm(s.Address)))
                        .ToDictionary(g => g.Key, g => g.First().Id);
                    var studentGroupSet = new HashSet<(int StudentId, int GroupId)>(existingStudentGroups);

                    // StudentCode-ების HashSet
                    var existingCodes = new HashSet<string>(existingStudents
                        .Where(s => !string.IsNullOrWhiteSpace(s.StudentCode))
                        .Select(s => s.StudentCode.Trim()),
                        StringComparer.OrdinalIgnoreCase);

                    var pendingCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var importedStudentIds = new List<int>();
                    var importedStudentGroupIds = new List<int>();
                    var importedStudentSubGroupIds = new List<int>();
                    var updatedGroupIds = new HashSet<int>();
                    var updatedSubGroupIds = new HashSet<int>();

                    using (var connection = _connectionProvider.GetLocalConnection())
                    {
                        connection.Open();

                        using (var transaction = connection.BeginTransaction())
                        {
                            int current = 0;
                            foreach (var (student, groupId) in studentsToImport)
                            {
                                try
                                {
                                    current++;
                                    progress?.Report((current, studentsToImport.Count,
                                        $"სტუდენტის დამატება: {student.FirstName} {student.LastName}"));

                                    // StudentCode-ის გენერაცია თუ დუბლიკატია
                                    if (string.IsNullOrWhiteSpace(student.StudentCode))
                                    {
                                        student.StudentCode = _studentCodeGenerator.GenerateUniqueStudentCode(existingCodes, pendingCodes);
                                        pendingCodes.Add(student.StudentCode);
                                    }
                                    else
                                    {
                                        var studentCode = student.StudentCode.Trim();
                                        if (!pendingCodes.Add(studentCode) || existingCodes.Contains(studentCode))
                                        {
                                            student.StudentCode = _studentCodeGenerator.GenerateUniqueStudentCode(existingCodes, pendingCodes);
                                            pendingCodes.Add(student.StudentCode);
                                        }
                                    }

                                    // Duplicate checking
                                    int? studentId = null;
                                    var strictKey = (norm(student.FirstName), norm(student.LastName), student.Id_Numb, norm(student.Address));
                                    if (strictToId.TryGetValue(strictKey, out int foundStrict))
                                    {
                                        studentId = foundStrict;
                                    }

                                    if (studentId == null)
                                    {
                                        // ახალი სტუდენტის დამატება
                                        studentId = _studentRepository.InsertStudent(student, connection, transaction);
                                        strictToId[strictKey] = studentId.Value;
                                        importedStudentIds.Add(studentId.Value);

                                        // StudentGroups-ში დამატება
                                        var sg = new StudentGroups
                                        {
                                            StudentId = studentId.Value,
                                            GroupId = groupId,
                                            Status = student.Status,
                                            DateOfPayment = student.DateOfPayment ?? GetDefaultPaymentDate() ?? throw new InvalidOperationException("Default payment date is not configured"),
                                            PaymentStatus = "Pending",
                                            Price = student.TuitionFee,
                                            Discount = student.Discount
                                        };
                                        var studentGroupId = _studentGroupRepository.InsertStudentGroup(sg, connection, transaction);
                                        studentGroupSet.Add((studentId.Value, groupId));
                                        importedStudentGroupIds.Add(studentGroupId);

                                        if (!updatedGroupIds.Contains(groupId))
                                        {
                                            updatedGroupIds.Add(groupId);
                                        }
                                        _groupService.RecalculateStudentCount(groupId, connection, transaction);

                                        // StudentSubGroups-ში დამატება
                                        SubGroup subGroup = null;

                                        if (student.SubGroup > 0)
                                        {
                                            // Excel-ში SubGroup column-ში რიცხვი (1, 2, 3...) არის SubGroup-ის ნომერი/ინდექსი, არა ID
                                            // ამიტომ ვპოულობთ Group-ის SubGroup-ების სიიდან ინდექსით
                                            var subGroups = _subGroupRepository.GetSubGroupsByGroupId(groupId);
                                            if (subGroups != null && subGroups.Count > 0)
                                            {
                                                // student.SubGroup-1 რადგან Excel-ში 1-დან იწყება, ხოლო სია 0-დან
                                                int subGroupIndex = student.SubGroup - 1;
                                                if (subGroupIndex >= 0 && subGroupIndex < subGroups.Count)
                                                {
                                                    subGroup = subGroups[subGroupIndex];
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // თუ SubGroup არ არის მითითებული ან 1-ზე ნაკლები, დავამატოთ პირველ SubGroup-ში
                                            subGroup = _subGroupRepository.GetFirstSubGroupByGroupId(groupId);
                                        }

                                        if (subGroup != null && subGroup.GroupId == groupId)
                                        {
                                            var ssg = new StudentSubGroups
                                            {
                                                StudentId = studentId.Value,
                                                GroupId = subGroup.GroupId,
                                                SubGroupId = subGroup.Id,
                                                PaymentStatus = "Pending",
                                                DateOfPayment = student.DateOfPayment ?? GetDefaultPaymentDate() ?? throw new InvalidOperationException("Default payment date is not configured"),
                                                Price = student.TuitionFee,
                                                Status = student.Status
                                            };
                                            var studentSubGroupId = _studentSubGroupRepository.InsertStudentSubGroup(ssg, connection, transaction);
                                            importedStudentSubGroupIds.Add(studentSubGroupId);

                                            if (!updatedSubGroupIds.Contains(subGroup.Id))
                                            {
                                                updatedSubGroupIds.Add(subGroup.Id);
                                            }
                                            _subGroupRepository.IncrementSubGroupCount(subGroup.Id, connection, transaction);
                                        }

                                        importedCount++;
                                    }
                                    else
                                    {
                                        // არსებული სტუდენტის დამატება ჯგუფში
                                        if (!studentGroupSet.Contains((studentId.Value, groupId)))
                                        {
                                            var sg = new StudentGroups
                                            {
                                                StudentId = studentId.Value,
                                                GroupId = groupId,
                                                Status = student.Status,
                                                DateOfPayment = student.DateOfPayment ?? GetDefaultPaymentDate() ?? throw new InvalidOperationException("Default payment date is not configured"),
                                                PaymentStatus = "Pending",
                                                Price = student.TuitionFee,
                                                Discount = student.Discount
                                            };
                                            var studentGroupId = _studentGroupRepository.InsertStudentGroup(sg, connection, transaction);
                                            studentGroupSet.Add((studentId.Value, groupId));
                                            importedStudentGroupIds.Add(studentGroupId);

                                            if (!updatedGroupIds.Contains(groupId))
                                            {
                                                updatedGroupIds.Add(groupId);
                                            }
                                            _groupService.RecalculateStudentCount(groupId, connection, transaction);

                                            // StudentSubGroups-ში დამატება
                                            SubGroup subGroup = null;

                                            if (student.SubGroup > 0)
                                            {
                                                // Excel-ში SubGroup column-ში რიცხვი (1, 2, 3...) არის SubGroup-ის ნომერი/ინდექსი, არა ID
                                                // ამიტომ ვპოულობთ Group-ის SubGroup-ების სიიდან ინდექსით
                                                var subGroups = _subGroupRepository.GetSubGroupsByGroupId(groupId);
                                                if (subGroups != null && subGroups.Count > 0)
                                                {
                                                    // student.SubGroup-1 რადგან Excel-ში 1-დან იწყება, ხოლო სია 0-დან
                                                    int subGroupIndex = student.SubGroup - 1;
                                                    if (subGroupIndex >= 0 && subGroupIndex < subGroups.Count)
                                                    {
                                                        subGroup = subGroups[subGroupIndex];
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // თუ SubGroup არ არის მითითებული ან 1-ზე ნაკლები, დავამატოთ პირველ SubGroup-ში
                                                subGroup = _subGroupRepository.GetFirstSubGroupByGroupId(groupId);
                                            }

                                            if (subGroup != null && subGroup.GroupId == groupId)
                                            {
                                                var ssg = new StudentSubGroups
                                                {
                                                    StudentId = studentId.Value,
                                                    GroupId = subGroup.GroupId,
                                                    SubGroupId = subGroup.Id,
                                                    PaymentStatus = "Pending",
                                                    DateOfPayment = student.DateOfPayment ?? GetDefaultPaymentDate() ?? throw new InvalidOperationException("Default payment date is not configured"),
                                                    Price = student.TuitionFee,
                                                    Status = student.Status
                                                };
                                                var studentSubGroupId = _studentSubGroupRepository.InsertStudentSubGroup(ssg, connection, transaction);
                                                importedStudentSubGroupIds.Add(studentSubGroupId);

                                                if (!updatedSubGroupIds.Contains(subGroup.Id))
                                                {
                                                    updatedSubGroupIds.Add(subGroup.Id);
                                                }
                                                _subGroupRepository.IncrementSubGroupCount(subGroup.Id, connection, transaction);
                                            }

                                            importedCount++;
                                            multiGroupCount++;
                                        }
                                        else
                                        {
                                            duplicateCount++;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    duplicateCount++;
                                    var errorMsg = ex.Message;
                                    errors.Add($"სტუდენტისთვის '{student.FirstName} {student.LastName}': {errorMsg}");

                                    // შევინახოთ failed import (თუ ვიცით sheetName და rowNumber)
                                    // აქ sheetName და rowNumber არ გვაქვს, მაგრამ შევინახოთ რაც გვაქვს
                                    SaveFailedImport(null, 0, student, groupId, errorMsg, null);
                                }
                            }

                            transaction.Commit();

                            // Sync to server
                            SyncToServerAsync(importedStudentIds, importedStudentGroupIds, importedStudentSubGroupIds, updatedGroupIds, updatedSubGroupIds).Wait();
                        }
                    }

                    return ImportResult.CreateSuccess(importedCount, duplicateCount);
                }
                catch (Exception ex)
                {
                    errors.Add($"ბაზაში ჩასმის შეცდომა: {ex.Message}");
                    return ImportResult.CreateError($"ბაზაში ჩასმის შეცდომა: {ex.Message}");
                }
            });
        }

        private async Task SyncToServerAsync(
            List<int> importedStudentIds,
            List<int> importedStudentGroupIds,
            List<int> importedStudentSubGroupIds,
            HashSet<int> updatedGroupIds,
            HashSet<int> updatedSubGroupIds)
        {
            await Task.Run(async () =>
            {
                // Students sync
                foreach (var studentId in importedStudentIds)
                {
                    try
                    {
                        var student = _studentRepository.GetStudentById(studentId);
                        if (student != null)
                        {
                            await _upStreamChangeTracker.TrackStudentChangeAsync(studentId, SyncOperationType.Insert, student);
                        }
                    }
                    catch
                    {
                        // Log error but continue
                    }
                }

                // StudentGroups sync
                foreach (var studentGroupId in importedStudentGroupIds)
                {
                    try
                    {
                        var studentGroup = _studentGroupRepository.GetById(studentGroupId);
                        if (studentGroup != null)
                        {
                            await _upStreamChangeTracker.TrackStudentGroupChangeAsync(studentGroupId, SyncOperationType.Insert, studentGroup);
                        }
                    }
                    catch
                    {
                        // Log error but continue
                    }
                }

                // StudentSubGroups sync
                foreach (var studentSubGroupId in importedStudentSubGroupIds)
                {
                    try
                    {
                        var studentSubGroup = _studentSubGroupRepository.GetById(studentSubGroupId);
                        if (studentSubGroup != null)
                        {
                            await _upStreamChangeTracker.TrackStudentSubGroupChangeAsync(studentSubGroupId, SyncOperationType.Insert, studentSubGroup);
                        }
                    }
                    catch
                    {
                        // Log error but continue
                    }
                }

                // Groups sync (StudentCount update)
                foreach (var groupId in updatedGroupIds)
                {
                    try
                    {
                        var group = _groupService.GetGroupById(groupId);
                        if (group != null)
                        {
                            await _upStreamChangeTracker.TrackGroupChangeAsync(groupId, SyncOperationType.Update, group);
                        }
                    }
                    catch
                    {
                        // Log error but continue
                    }
                }

                // SubGroups sync (StudentCount update)
                foreach (var subGroupId in updatedSubGroupIds)
                {
                    try
                    {
                        var subGroup = _subGroupRepository.GetSubGroupById(subGroupId);
                        if (subGroup != null)
                        {
                            await _upStreamChangeTracker.TrackSubGroupChangeAsync(subGroupId, SyncOperationType.Update, subGroup);
                        }
                    }
                    catch
                    {
                        // Log error but continue
                    }
                }
            });
        }

        /// <summary>
        /// წარუმატებელი იმპორტის შენახვა FailedStudentImports ცხრილში
        /// </summary>
        private void SaveFailedImport(string sheetName, int rowNumber, Student student, int? groupId, string errorMessage, string excelFileName)
        {
            try
            {
                using (var connection = _connectionProvider.GetLocalConnection())
                {
                    connection.Open();

                    var query = @"INSERT INTO FailedStudentImports 
                                (SheetName, RowNumber, FirstName, LastName, Age, ParentName, PhoneNumber, Id_Numb, Address,
                                 TuitionFee, Discount, DateOfPayment, RegistrationDate, StudentCode, SubGroup, GroupId, GroupName,
                                 ErrorMessage, ImportDate, User_Id, ExcelFileName)
                                VALUES 
                                (@SheetName, @RowNumber, @FirstName, @LastName, @Age, @ParentName, @PhoneNumber, @Id_Numb, @Address,
                                 @TuitionFee, @Discount, @DateOfPayment, @RegistrationDate, @StudentCode, @SubGroup, @GroupId, @GroupName,
                                 @ErrorMessage, @ImportDate, @User_Id, @ExcelFileName)";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@SheetName", (object)sheetName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RowNumber", rowNumber);
                        cmd.Parameters.AddWithValue("@FirstName", (object)student?.FirstName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LastName", (object)student?.LastName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Age", student?.Age ?? 0);
                        cmd.Parameters.AddWithValue("@ParentName", (object)student?.ParentName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PhoneNumber", (object)student?.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id_Numb", student?.Id_Numb ?? 0L);
                        cmd.Parameters.AddWithValue("@Address", (object)student?.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TuitionFee", student?.TuitionFee ?? 0m);
                        cmd.Parameters.AddWithValue("@Discount", student?.Discount ?? 0.0);
                        cmd.Parameters.AddWithValue("@DateOfPayment", student?.DateOfPayment.HasValue == true ? (object)student.DateOfPayment.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegistrationDate", student != null && student.RegistrationDate != default(DateTime) ? (object)student.RegistrationDate : DBNull.Value);
                        cmd.Parameters.AddWithValue("@StudentCode", (object)student?.StudentCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubGroup", student?.SubGroup ?? 0);
                        cmd.Parameters.AddWithValue("@GroupId", groupId.HasValue ? (object)groupId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@GroupName", (object)sheetName ?? DBNull.Value); // Excel sheet-ის სახელი როგორც GroupName
                        cmd.Parameters.AddWithValue("@ErrorMessage", errorMessage ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ImportDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@User_Id", UserSession.Id > 0 ? (object)UserSession.Id : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExcelFileName", (object)excelFileName ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // ლოგირება შეცდომის შენახვისას (არ ჩავწეროთ errors-ში, რადგან ეს დამატებითი ლოგიკაა)
                System.Diagnostics.Debug.WriteLine($"Failed to save failed import: {ex.Message}");
            }
        }
    }
}
