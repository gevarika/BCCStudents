using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BCCStudents.Domain.Interfaces;
using ClosedXML.Excel;

namespace BCCStudents.TestHelpers
{
    /// <summary>
    /// Helper class to test import process step by step
    /// </summary>
    public class ImportTestHelper
    {
        private readonly IImportService _importService;
        private readonly IGroupRepository _groupRepository;
        private readonly ISubGroupRepository _subGroupRepository;
        
        public ImportTestHelper(IImportService importService, IGroupRepository groupRepository, ISubGroupRepository subGroupRepository)
        {
            _importService = importService;
            _groupRepository = groupRepository;
            _subGroupRepository = subGroupRepository;
        }
        
        /// <summary>
        /// Test Excel file reading and validation
        /// </summary>
        public ImportTestResult TestExcelFile(string filePath)
        {
            var result = new ImportTestResult();
            var logMessages = new List<string>();
            
            try
            {
                logMessages.Add($"Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ: {filePath}");
                
                if (!File.Exists(filePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"áƒ¤áƒáƒ˜áƒšáƒ˜ áƒáƒ  áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡: {filePath}";
                    return result;
                }
                
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheets = workbook.Worksheets.ToList();
                    logMessages.Add($"áƒœáƒáƒžáƒáƒ•áƒœáƒ˜áƒ {worksheets.Count} worksheet");
                    
                    foreach (var worksheet in worksheets)
                    {
                        var sheetName = worksheet.Name;
                        var rows = worksheet.RowsUsed().Skip(1).ToList();
                        var headers = worksheet.Row(1).CellsUsed().ToDictionary(
                            c => c.Value.ToString().Trim(),
                            c => c.Address.ColumnNumber - 1
                        );
                        
                        logMessages.Add($"Worksheet '{sheetName}': {rows.Count} áƒ›áƒ¬áƒ™áƒ áƒ˜áƒ•áƒ˜, {headers.Count} áƒ¡áƒ•áƒ”áƒ¢áƒ˜");
                        logMessages.Add($"  áƒ¡áƒ•áƒ”áƒ¢áƒ”áƒ‘áƒ˜: {string.Join(", ", headers.Keys)}");
                        
                        // Test first few rows
                        var sampleRows = rows.Take(3).ToList();
                        for (int i = 0; i < sampleRows.Count; i++)
                        {
                            var row = sampleRows[i];
                            var rowData = new List<string>();
                            
                            foreach (var header in headers.Keys)
                            {
                                var colIdx = headers[header];
                                var value = row.Cell(colIdx + 1).GetString();
                                rowData.Add($"{header}: '{value}'");
                            }
                            
                            logMessages.Add($"  áƒ›áƒ¬áƒ™áƒ áƒ˜áƒ•áƒ˜ {i + 1}: {string.Join(" | ", rowData)}");
                        }
                    }
                }
                
                result.Success = true;
                result.LogMessages = logMessages;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.LogMessages = logMessages;
                logMessages.Add($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                logMessages.Add($"Stack Trace: {ex.StackTrace}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Test database connectivity and basic operations
        /// </summary>
        public ImportTestResult TestDatabaseConnectivity()
        {
            var result = new ImportTestResult();
            var logMessages = new List<string>();
            
            try
            {
                logMessages.Add("áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ");
                
                // Test group repository
                var groups = _groupRepository.GetAllGroups();
                logMessages.Add($"áƒœáƒáƒžáƒáƒ•áƒœáƒ˜áƒ {groups.Count} áƒ¯áƒ’áƒ£áƒ¤áƒ˜");
                
                foreach (var group in groups.Take(5))
                {
                    logMessages.Add($"  áƒ¯áƒ’áƒ£áƒ¤áƒ˜: {group.Name} (ID: {group.Id}, áƒ¤áƒáƒ¡áƒ˜: {group.Price})");
                    
                    // Test sub groups for this group
                    var subGroups = _subGroupRepository.GetSubGroupsByGroupId(group.Id);
                    logMessages.Add($"    áƒ¥áƒ•áƒ”áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜: {subGroups.Count}");
                    
                    foreach (var subGroup in subGroups.Take(3))
                    {
                        logMessages.Add($"      {subGroup.Name} (ID: {subGroup.Id}, áƒ¤áƒáƒ¡áƒ˜: {subGroup.TuitionFee})");
                    }
                }
                
                result.Success = true;
                result.LogMessages = logMessages;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.LogMessages = logMessages;
                logMessages.Add($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                logMessages.Add($"Stack Trace: {ex.StackTrace}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Test import process with minimal data
        /// </summary>
        public ImportTestResult TestImportProcess(string filePath, Dictionary<string, int> sheetToGroupIdMap)
        {
            var result = new ImportTestResult();
            var logMessages = new List<string>();
            
            try
            {
                logMessages.Add("áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ");
                
                // Create minimal column mappings
                var columnMappings = new Dictionary<string, Dictionary<string, int>>();
                using (var workbook = new XLWorkbook(filePath))
                {
                    foreach (var sheetName in sheetToGroupIdMap.Keys)
                    {
                        var worksheet = workbook.Worksheet(sheetName);
                        var headers = worksheet.Row(1).CellsUsed().ToDictionary(
                            c => c.Value.ToString().Trim(),
                            c => c.Address.ColumnNumber - 1
                        );
                        
                        var mapping = new Dictionary<string, int>();
                        var requiredFields = new[] { "FirstName", "LastName", "Age", "ParentName", "PhoneNumber", "Id_Numb", "Address" };
                        
                        foreach (var field in requiredFields)
                        {
                            var matchedHeader = headers.Keys.FirstOrDefault(h => 
                                string.Equals(h.Replace(" ", ""), field.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
                            if (matchedHeader != null)
                            {
                                mapping.Add(field, headers[matchedHeader]);
                                logMessages.Add($"  {sheetName}: {field} -> {matchedHeader}");
                            }
                        }
                        
                        columnMappings.Add(sheetName, mapping);
                        logMessages.Add($"  {sheetName}: áƒ›áƒáƒžáƒáƒ•áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ {mapping.Count} áƒ•áƒ”áƒšáƒ˜áƒ¡ áƒ›áƒ”áƒžáƒ˜áƒœáƒ’áƒ˜");
                    }
                }
                
                // Test with progress reporting
                var progress = new Progress<(int current, int total, string worksheet)>(p =>
                {
                    logMessages.Add($"áƒžáƒ áƒáƒ’áƒ áƒ”áƒ¡áƒ˜: {p.current}/{p.total} - {p.worksheet}");
                });
                
                logMessages.Add("áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒ¡áƒ”áƒ áƒ•áƒ˜áƒ¡áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ«áƒáƒ®áƒ”áƒ‘áƒ");
                var importResult = _importService.ImportStudentsAsync(filePath, columnMappings, sheetToGroupIdMap, true, progress).Result;
                
                logMessages.Add($"áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ˜: Success={importResult.Success}");
                if (importResult.Success)
                {
                    logMessages.Add($"  áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜: {importResult.ImportedCount}");
                    logMessages.Add($"  áƒ“áƒ£áƒ‘áƒšáƒ˜áƒ™áƒáƒ¢áƒ”áƒ‘áƒ˜: {importResult.Dublicates}");
                }
                else
                {
                    logMessages.Add($"  áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {importResult.ErrorMessage}");
                }
                
                result.Success = true;
                result.LogMessages = logMessages;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.LogMessages = logMessages;
                logMessages.Add($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                logMessages.Add($"Stack Trace: {ex.StackTrace}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Save test results to file
        /// </summary>
        public void SaveTestResults(ImportTestResult testResult, string fileName = null)
        {
            if (fileName == null)
            {
                fileName = $"ImportTest_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            }
            
            var content = $"Import Test Results - {DateTime.Now}\n";
            content += $"Success: {testResult.Success}\n";
            
            if (!testResult.Success)
            {
                content += $"Error: {testResult.ErrorMessage}\n";
            }
            
            content += "\nLog Messages:\n";
            content += string.Join("\n", testResult.LogMessages);
            
            File.WriteAllText(fileName, content);
        }
    }
    
    /// <summary>
    /// Result of import test
    /// </summary>
    public class ImportTestResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> LogMessages { get; set; } = new List<string>();
    }
}

