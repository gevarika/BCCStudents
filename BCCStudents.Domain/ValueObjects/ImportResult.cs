using System;
using System.Collections.Generic;

namespace BCCStudents.Domain.Entities
{
    public class ImportResult
    {
        public bool IsSuccess { get; set; }
        public int ImportedCount { get; set; }
        public int Dublicates { get; set; }
        public string ErrorMessage { get; set; }
        public List<FailedRow> FailedRows { get; set; } = new List<FailedRow>();

        // Existing 'Success' property might be causing confusion.
        // Let's rely on IsSuccess for clarity.
        public bool Success => IsSuccess;

        public static ImportResult CreateSuccess(int importedCount, int dublicateCount)
        {
            return new ImportResult { IsSuccess = true, ImportedCount = importedCount, Dublicates = dublicateCount };
        }

        public static ImportResult CreateError(string message)
        {
            return new ImportResult { IsSuccess = false, ErrorMessage = message };
        }
    }
} 
