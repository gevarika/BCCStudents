using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using ClosedXML.Excel;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Threading.Tasks;
using System.Globalization;
using BCCStudents.Application.Services.Sync.UpStream;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services {
    public class ExcelPaymentImportService : IExcelPaymentImportService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBalanceRepository _balanceManager;
        private readonly ILoggerRepository _loggerRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly PaymentDescriptionAnalyzer _descriptionAnalyzer;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;
        private readonly List<FailedPayment> _failedPayments = new List<FailedPayment>();

        public ExcelPaymentImportService(
            IPaymentRepository paymentRepository, 
            IBalanceRepository balanceManager, 
            ILoggerRepository loggerRepository,
            IStudentRepository studentRepository,
            IGroupRepository groupRepository,
            IUpStreamChangeTracker upStreamChangeTracker)
        {
            _paymentRepository = paymentRepository;
            _balanceManager = balanceManager;
            _loggerRepository = loggerRepository;
            _studentRepository = studentRepository;
            _upStreamChangeTracker = upStreamChangeTracker;
            _descriptionAnalyzer = new PaymentDescriptionAnalyzer(studentRepository, groupRepository, loggerRepository);
            
            // გავასუფთაოთ ძველი ვერ წარმატებული გადახდები
            //_paymentRepository.ClearFailedPayments();
        }

        /// <summary>
        /// Excel ფაილიდან გადახდების იმპორტი
        /// </summary>
        /// <param name="filePath">Excel ფაილის გზა</param>
        /// <param name="progressCallback">პროგრესის კალბექი (current, total)</param>
        /// <returns>იმპორტის შედეგი</returns>
        /// <remarks>
        /// Excel სტრუქტურა:
        /// - სვეტი 1: თარიღი (PaymentDate)
        /// - სვეტი 3: თანხა (Amount)
        /// - სვეტი 5: პირადი ნომერი (PersonalId) - ოფციონალური
        /// - სვეტი 6: გადამხდელის სახელი (PayerName)
        /// - სვეტი 7: აღწერა (Description)
        /// </remarks>
        public async Task<ImportResult> ImportPaymentsFromExcel(string filePath, Action<int, int> progressCallback = null)
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // Skip header row
                var totalRows = worksheet.LastRowUsed().RowNumber() - 1;
                var processedRows = 0;

                var failedRows = new List<FailedRow>();

                foreach (var row in rows)
                {
                    try
                    {
                        // ==================== 1. მონაცემების ამოღება Excel-იდან ====================
                        DateTime paymentDate;
                        var dateStr = row.Cell(1).GetString()?.Trim() ?? "";
                        if (!DateTime.TryParse(dateStr, out paymentDate))
                        {
                            throw new FormatException($"არასწორი ფორმატი თარიღისთვის: {dateStr}");
                        }

                        decimal amount;
                        var amountStr = row.Cell(3).GetString()?.Trim().Replace("₾", "").Replace(" ", "") ?? "";
                        if (!decimal.TryParse(amountStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amount))
                        {
                            throw new FormatException($"არასწორი ფორმატი თანხისთვის: {amountStr}");
                        }

                        long? personalId = null;
                        var personalIdStr = row.Cell(5).GetString()?.Trim() ?? "";
                        if (!string.IsNullOrWhiteSpace(personalIdStr))
                        {
                            if (!long.TryParse(personalIdStr, out long parsedId))
                            {
                                throw new FormatException($"არასწორი ფორმატი პირადი ნომრისთვის: {personalIdStr}");
                            }
                            personalId = parsedId;
                        }

                        var payerName = row.Cell(6).GetString();
                        var description = row.Cell(7).GetString();

                        // ==================== 2. დუბლიკატების შემოწმება ====================
                        if (_paymentRepository.CheckImportedPaymentDuplicate(paymentDate, amount, personalId, description))
                        {
                            var failedPayment = new FailedPayment
                            {
                                RowNumber = row.RowNumber(),
                                PaymentDate = paymentDate,
                                Amount = amount,
                                PersonalId = personalId,
                                Description = description,
                                Reason = "ზუსტად იგივე გადახდა უკვე არსებობს სისტემაში (თარიღი, თანხა, აღწერა)",
                                CreatedAt = DateTime.Now
                            };
                            _failedPayments.Add(failedPayment);
                            var failedPaymentId = _paymentRepository.SaveFailedPayment(failedPayment);
                            
                            // ==================== სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId);
                                if (savedFailedPayment != null)
                                {
                                    await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId, SyncOperationType.Insert, savedFailedPayment);
                                    _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId}): {syncEx.Message}", "System");
                            }
                            
                            failedRows.Add(new FailedRow
                            {
                                RowNumber = row.RowNumber(),
                                Reason = failedPayment.Reason
                            });
                            continue; // გამოტოვება - დუბლიკატი
                        }

                        // ==================== 3. სტუდენტის იდენტიფიცირება ====================
                        var analysis = await _descriptionAnalyzer.AnalyzeDescription(description, personalId);

                        if (analysis.IsValid && !analysis.RequiresReview && analysis.StudentId.HasValue)
                        {
                            // ==================== 4. ბალანსზე დამატება ====================
                            // ⚠️ მნიშვნელოვანი: IncrementStudentBalance იყენებს Balance = Balance + @Amount
                            // ეს უზრუნველყოფს რომ არსებული ბალანსი არ დაიკარგება
                            var balanceUpdated = _studentRepository.IncrementStudentBalance(analysis.StudentId.Value, amount);
                            
                            if (!balanceUpdated)
                            {
                                _loggerRepository?.LogImportAction("ბალანსის განახლება", "Error", $"ბალანსის განახლება ვერ მოხერხდა: StudentId={analysis.StudentId.Value}, Amount={amount}", "System");
                            }
                            else
                            {
                                _loggerRepository?.LogImportAction("ბალანსის განახლება", "Success", $"ბალანსი განახლდა: StudentId={analysis.StudentId.Value}, Amount={amount}", "System");
                            }
                            
                            // ==================== 4.1. სერვერზე სინქრონიზაცია ====================
                            if (balanceUpdated)
                            {
                                try
                                {
                                    // მცირე დაყოვნება, რათა დავრწმუნდეთ რომ ბაზაში ცვლილება დაფიქსირდა
                                    await Task.Delay(100);
                                    
                                    var student = _studentRepository.GetStudentById(analysis.StudentId.Value);
                                    if (student != null)
                                    {
                                        _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Info", $"სტუდენტის სერვერზე სინქრონიზაცია: ID={analysis.StudentId.Value}, Balance={student.Balance}", "System");
                                        await _upStreamChangeTracker.TrackStudentChangeAsync(analysis.StudentId.Value, SyncOperationType.Update, student);
                                        _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Success", $"სტუდენტის სერვერზე სინქრონიზაცია დასრულდა: ID={analysis.StudentId.Value}", "System");
                                    }
                                    else
                                    {
                                        _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Error", $"სტუდენტი ვერ მოიძებნა სინქრონიზაციისთვის: ID={analysis.StudentId.Value}", "System");
                                    }
                                }
                                catch (Exception syncEx)
                                {
                                    // ლოგირება, მაგრამ არ ვაჩერებთ პროცესს
                                    _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Error", $"შეცდომა სტუდენტის სერვერზე სინქრონიზაციისას (ID: {analysis.StudentId.Value}): {syncEx.Message}", "System");
                                }
                            }
                            
                            // ==================== 5. იმპორტის ლოგირება ====================
                            var importedPaymentLogId = _paymentRepository.AddImportedPaymentLog(paymentDate, amount, personalId, description, filePath);
                            
                            // ==================== 5.1. სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var importedPaymentLog = _paymentRepository.GetImportedPaymentLogById(importedPaymentLogId);
                                if (importedPaymentLog != null)
                                {
                                    await _upStreamChangeTracker.TrackImportedPaymentLogChangeAsync(importedPaymentLogId, SyncOperationType.Insert, importedPaymentLog);
                                    _loggerRepository?.LogImportAction("ImportedPaymentsLog სერვერზე სინქრონიზაცია", "Success", $"ImportedPaymentLog ID={importedPaymentLogId} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("ImportedPaymentsLog სერვერზე სინქრონიზაცია", "Error", $"შეცდომა ImportedPaymentLog სერვერზე სინქრონიზაციისას (ID: {importedPaymentLogId}): {syncEx.Message}", "System");
                            }
                        }
                        else
                        {
                            // ==================== 6. ვერ იდენტიფიცირებული გადახდების შენახვა ====================
                            var failedPayment = new FailedPayment
                            {
                                RowNumber = row.RowNumber(),
                                PaymentDate = paymentDate,
                                Amount = amount,
                                PersonalId = personalId,
                                Description = description,
                                Reason = "ვერ მოხერხდა სტუდენტის იდენტიფიცირება",
                                CreatedAt = DateTime.Now
                            };
                            _failedPayments.Add(failedPayment);
                            var failedPaymentId2 = _paymentRepository.SaveFailedPayment(failedPayment);
                            
                            // ==================== სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId2);
                                if (savedFailedPayment != null)
                                {
                                    await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId2, SyncOperationType.Insert, savedFailedPayment);
                                    _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId2} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId2}): {syncEx.Message}", "System");
                            }
                            
                            failedRows.Add(new FailedRow
                            {
                                RowNumber = row.RowNumber(),
                                Reason = failedPayment.Reason
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        failedRows.Add(new FailedRow
                        {
                            RowNumber = row.RowNumber(),
                            Reason = ex.Message
                        });
                    }

                    processedRows++;
                    progressCallback?.Invoke(processedRows, totalRows);
                }

                return new ImportResult
                {
                    IsSuccess = true,
                    ImportedCount = processedRows - failedRows.Count,
                    FailedRows = failedRows
                };
            }
        }

        /// <summary>
        /// არჩეული გადახდების იმპორტი (UI-დან მომხმარებლის მიერ არჩეული)
        /// </summary>
        /// <param name="filePath">Excel ფაილის გზა</param>
        /// <param name="allRows">ყველა მწკრივი DataRow-ების სახით</param>
        /// <param name="progressCallback">პროგრესის კალბექი</param>
        /// <returns>იმპორტის შედეგი</returns>
        public async Task<ImportResult> ImportSelectedPayments(string filePath, IEnumerable<DataRow> allRows, Action<int, int> progressCallback = null)
        {
            var payments = new List<Payment>();
            var failedRows = new List<FailedRow>();
            var totalRows = allRows.Count();
            var result = new ImportResult();
            var processedCount = 0;
            int current = 0;

            foreach (var row in allRows)
            {
                try
                {
                    // ==================== 1. მონაცემების ამოღება ====================
                    bool isSelected = (bool)row["IsSelected"];
                    bool requiresReview = (bool)row["RequiresReview"];
                    var paymentDate = row["PaymentDate"] != DBNull.Value ? (DateTime)row["PaymentDate"] : DateTime.MinValue;
                    var amount = row["Amount"] != DBNull.Value ? (decimal)row["Amount"] : 0;
                    var personalId = row["PersonalId"] != DBNull.Value ? (long?)row["PersonalId"] : null;
                    var description = row["Description"]?.ToString();
                    var rowNumber = (int)row["RowNumber"];

                    // ==================== 2. არჩეული და დადასტურებული გადახდების დამუშავება ====================
                    if (isSelected && !requiresReview)
                    {
                        // ==================== 2.1. დუბლიკატების შემოწმება ====================
                        if (_paymentRepository.CheckImportedPaymentDuplicate(paymentDate, amount, personalId, description))
                        {
                            var failedPayment = new FailedPayment
                            {
                                RowNumber = rowNumber,
                                PaymentDate = paymentDate,
                                Amount = amount,
                                PersonalId = personalId,
                                Description = description,
                                Reason = "ზუსტად იგივე გადახდა უკვე არსებობს სისტემაში (თარიღი, თანხა, აღწერა)",
                                CreatedAt = DateTime.Now
                            };
                            _failedPayments.Add(failedPayment);
                            var failedPaymentId3 = _paymentRepository.SaveFailedPayment(failedPayment);
                            
                            // ==================== სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId3);
                                if (savedFailedPayment != null)
                                {
                                    await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId3, SyncOperationType.Insert, savedFailedPayment);
                                    _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId3} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId3}): {syncEx.Message}", "System");
                            }
                            
                            failedRows.Add(new FailedRow
                            {
                                RowNumber = rowNumber,
                                Reason = failedPayment.Reason
                            });
                            continue; // გამოტოვება - დუბლიკატი
                        }

                        // ==================== 2.2. სტუდენტის იდენტიფიცირება ====================
                        var analysis = await _descriptionAnalyzer.AnalyzeDescription(description, personalId);
                        if (analysis.IsValid && !analysis.RequiresReview && analysis.StudentId.HasValue)
                        {
                            // ==================== 2.3. ბალანსზე დამატება ====================
                            // ⚠️ მნიშვნელოვანი: IncrementStudentBalance იყენებს Balance = Balance + @Amount
                            // ეს უზრუნველყოფს რომ არსებული ბალანსი არ დაიკარგება
                            var balanceUpdated = _studentRepository.IncrementStudentBalance(analysis.StudentId.Value, amount);
                            
                            if (!balanceUpdated)
                            {
                                _loggerRepository?.LogImportAction("ბალანსის განახლება", "Error", $"ბალანსის განახლება ვერ მოხერხდა: StudentId={analysis.StudentId.Value}, Amount={amount}", "System");
                            }
                            else
                            {
                                _loggerRepository?.LogImportAction("ბალანსის განახლება", "Success", $"ბალანსი განახლდა: StudentId={analysis.StudentId.Value}, Amount={amount}", "System");
                            }
                            
                            // ==================== 2.3.1. სერვერზე სინქრონიზაცია ====================
                            if (balanceUpdated)
                            {
                                try
                                {
                                    // მცირე დაყოვნება, რათა დავრწმუნდეთ რომ ბაზაში ცვლილება დაფიქსირდა
                                    await Task.Delay(100);
                                    
                                    var student = _studentRepository.GetStudentById(analysis.StudentId.Value);
                                    if (student != null)
                                    {
                                        _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Info", $"სტუდენტის სერვერზე სინქრონიზაცია: ID={analysis.StudentId.Value}, Balance={student.Balance}", "System");
                                        await _upStreamChangeTracker.TrackStudentChangeAsync(analysis.StudentId.Value, SyncOperationType.Update, student);
                                        _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Success", $"სტუდენტის სერვერზე სინქრონიზაცია დასრულდა: ID={analysis.StudentId.Value}", "System");
                                    }
                                    else
                                    {
                                        _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Error", $"სტუდენტი ვერ მოიძებნა სინქრონიზაციისთვის: ID={analysis.StudentId.Value}", "System");
                                    }
                                }
                                catch (Exception syncEx)
                                {
                                    // ლოგირება, მაგრამ არ ვაჩერებთ პროცესს
                                    _loggerRepository?.LogImportAction("სერვერზე სინქრონიზაცია", "Error", $"შეცდომა სტუდენტის სერვერზე სინქრონიზაციისას (ID: {analysis.StudentId.Value}): {syncEx.Message}", "System");
                                }
                            }
                            
                            // ==================== 2.4. იმპორტის ლოგირება ====================
                            var importedPaymentLogId2 = _paymentRepository.AddImportedPaymentLog(paymentDate, amount, personalId, description, filePath);
                            
                            // ==================== 2.4.1. სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var importedPaymentLog = _paymentRepository.GetImportedPaymentLogById(importedPaymentLogId2);
                                if (importedPaymentLog != null)
                                {
                                    await _upStreamChangeTracker.TrackImportedPaymentLogChangeAsync(importedPaymentLogId2, SyncOperationType.Insert, importedPaymentLog);
                                    _loggerRepository?.LogImportAction("ImportedPaymentsLog სერვერზე სინქრონიზაცია", "Success", $"ImportedPaymentLog ID={importedPaymentLogId2} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("ImportedPaymentsLog სერვერზე სინქრონიზაცია", "Error", $"შეცდომა ImportedPaymentLog სერვერზე სინქრონიზაციისას (ID: {importedPaymentLogId2}): {syncEx.Message}", "System");
                            }
                            
                            processedCount++;
                        }
                        else
                        {
                            // ==================== 2.5. შეცდომა - ვერ იდენტიფიცირებული ====================
                            // არ უნდა მოხდეს, მაგრამ ლოგირება შეცდომად
                            var failedPayment = new FailedPayment
                            {
                                RowNumber = rowNumber,
                                PaymentDate = paymentDate,
                                Amount = amount,
                                PersonalId = personalId,
                                Description = description,
                                Reason = "ვერ მოხერხდა სტუდენტის იდენტიფიცირება იმპორტისას",
                                CreatedAt = DateTime.Now
                            };
                            _failedPayments.Add(failedPayment);
                            var failedPaymentId4 = _paymentRepository.SaveFailedPayment(failedPayment);
                            
                            // ==================== სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId4);
                                if (savedFailedPayment != null)
                                {
                                    await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId4, SyncOperationType.Insert, savedFailedPayment);
                                    _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId4} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId4}): {syncEx.Message}", "System");
                            }
                            
                            failedRows.Add(new FailedRow
                            {
                                RowNumber = rowNumber,
                                Reason = failedPayment.Reason
                            });
                        }
                    }
                    // ==================== 3. არჩეული მაგრამ გადასახედი გადახდები ====================
                    else if (isSelected && requiresReview)
                    {
                        // შეცდომა: ჩანაწერი არჩეულია იმპორტისთვის მაგრამ საჭიროებს გადასახედს
                        var failedPayment = new FailedPayment
                        {
                            RowNumber = rowNumber,
                            PaymentDate = paymentDate,
                            Amount = amount,
                            PersonalId = personalId,
                            Description = description,
                            Reason = "ჩანაწერი არჩეულია იმპორტისთვის მაგრამ საჭიროებს გადასახედს",
                            CreatedAt = DateTime.Now
                        };
                        _failedPayments.Add(failedPayment);
                        var failedPaymentId5 = _paymentRepository.SaveFailedPayment(failedPayment);
                        
                        // ==================== სერვერზე სინქრონიზაცია ====================
                        try
                        {
                            await Task.Delay(50);
                            var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId5);
                            if (savedFailedPayment != null)
                            {
                                await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId5, SyncOperationType.Insert, savedFailedPayment);
                                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId5} სერვერზე გაიგზავნა", "System");
                            }
                        }
                        catch (Exception syncEx)
                        {
                            _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId5}): {syncEx.Message}", "System");
                        }
                        
                        failedRows.Add(new FailedRow
                        {
                            RowNumber = rowNumber,
                            Reason = failedPayment.Reason
                        });
                    }
                    // ==================== 4. არაარჩეული გადახდების დამუშავება ====================
                    {
                        // დაუდასტურებელი გადახდების შენახვა (მხოლოდ თუ არ არის დუბლიკატი)
                        if (!_paymentRepository.CheckFailedPaymentDuplicate(paymentDate, amount, personalId, description))
                        {
                            var failedPayment = new FailedPayment
                            {
                                RowNumber = rowNumber,
                                PaymentDate = paymentDate,
                                Amount = amount,
                                PersonalId = personalId,
                                Description = description,
                                Reason = "დაუდასტურებელი და ვერ იდენტიფიცირებული გადახდა",
                                CreatedAt = DateTime.Now
                            };
                            _failedPayments.Add(failedPayment);
                            var failedPaymentId6 = _paymentRepository.SaveFailedPayment(failedPayment);
                            
                            // ==================== სერვერზე სინქრონიზაცია ====================
                            try
                            {
                                await Task.Delay(50);
                                var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId6);
                                if (savedFailedPayment != null)
                                {
                                    await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId6, SyncOperationType.Insert, savedFailedPayment);
                                    _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId6} სერვერზე გაიგზავნა", "System");
                                }
                            }
                            catch (Exception syncEx)
                            {
                                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId6}): {syncEx.Message}", "System");
                            }
                            
                            failedRows.Add(new FailedRow
                            {
                                RowNumber = rowNumber,
                                Reason = failedPayment.Reason
                            });
                        }
                    }
                    // else: skip (either not selected and not problematic, or selected but requires review)
                }
                catch (Exception ex)
                {
                    failedRows.Add(new FailedRow
                    {
                        RowNumber = row["RowNumber"] != DBNull.Value ? (int)row["RowNumber"] : -1,
                        Reason = ex.Message
                    });
                }
                current++;
                progressCallback?.Invoke(current, totalRows);
            }

            result.IsSuccess = true;
            result.ImportedCount = processedCount;
            result.FailedRows = failedRows;
            return result;
        }

        private async Task AddFailedPayment(int rowNumber, DateTime paymentDate, decimal amount, long? personalId, string description, string reason)
        {
            var failedPayment = new FailedPayment
            {
                RowNumber = rowNumber,
                PaymentDate = paymentDate,
                Amount = amount,
                PersonalId = personalId,
                Description = description,
                Reason = reason,
                CreatedAt = DateTime.Now
            };
            _failedPayments.Add(failedPayment);
            var failedPaymentId = _paymentRepository.SaveFailedPayment(failedPayment);
            
            // ==================== სერვერზე სინქრონიზაცია ====================
            try
            {
                await Task.Delay(50);
                var savedFailedPayment = _paymentRepository.GetFailedPaymentById(failedPaymentId);
                if (savedFailedPayment != null)
                {
                    await _upStreamChangeTracker.TrackFailedPaymentChangeAsync(failedPaymentId, SyncOperationType.Insert, savedFailedPayment);
                    _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Success", $"FailedPayment ID={failedPaymentId} სერვერზე გაიგზავნა", "System");
                }
            }
            catch (Exception syncEx)
            {
                _loggerRepository?.LogImportAction("FailedPayments სერვერზე სინქრონიზაცია", "Error", $"შეცდომა FailedPayment სერვერზე სინქრონიზაციისას (ID: {failedPaymentId}): {syncEx.Message}", "System");
            }
        }

        public IEnumerable<FailedPayment> GetFailedPayments()
        {
            return _paymentRepository.GetFailedPayments();
        }
    }
}





