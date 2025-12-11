using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    public class TableSyncResult
    {
        public string TableName { get; set; }
        public bool Success { get; set; } = true;
        public int RecordsSynced { get; set; }
        public int ConflictsResolved { get; set; }
        public string Error { get; set; }

        public static TableSyncResult NoChanges(string tableName)
        {
            return new TableSyncResult
            {
                TableName = tableName,
                Success = true,
                RecordsSynced = 0,
                ConflictsResolved = 0
            };
        }

        public static TableSyncResult Failed(string tableName, string error)
        {
            return new TableSyncResult
            {
                TableName = tableName,
                Success = false,
                Error = error
            };
        }
    }
}


