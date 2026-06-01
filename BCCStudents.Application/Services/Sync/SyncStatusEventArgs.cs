using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Sync
{
    /// <summary>
    /// Event arguments for sync status updates
    /// </summary>
    public class SyncStatusEventArgs : EventArgs
    {
        private static readonly string[] PendingRegistrationTables =
        {
            "PendingStudents",
            "PendingStudentGroups",
            "PendingStudentSubGroups"
        };

        public bool Success { get; set; }
        public int RecordsSynced { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string SyncType { get; set; } // "DownStream" or "UpStream"
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public IReadOnlyList<TableSyncResult> TableResults { get; set; } = Array.Empty<TableSyncResult>();

        public bool HasPendingRegistrationChanges()
        {
            if (TableResults == null || TableResults.Count == 0)
            {
                return false;
            }

            return TableResults.Any(t =>
                t.RecordsSynced > 0 &&
                PendingRegistrationTables.Contains(t.TableName, StringComparer.OrdinalIgnoreCase));
        }
    }
}


