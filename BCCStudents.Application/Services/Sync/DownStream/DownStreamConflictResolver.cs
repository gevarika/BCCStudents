using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    /// <summary>
    /// DownStream კონფლიქტის პოლიტიკა:
    /// - Students/Groups/SubGroups/StudentGroups/StudentSubGroups: upsert SQL-ში local UpdatedAt ≥ server → local wins.
    /// - Payments/Users/Pending*/Logs: upsert ყოველთვის server-ის მნიშვნებით overwrite-ს აკეთებს.
    /// DetectConflictsAsync ამ ეтапზე არ ამოწმებს local pending upstream ცვლილებებს.
    /// </summary>
    public class DownStreamConflictResolver : IDownStreamConflictResolver
    {
        public Task<IReadOnlyList<SyncConflict>> DetectConflictsAsync<T>(string tableName, IReadOnlyList<T> serverData, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SyncConflict>>(new List<SyncConflict>());
        }

        public Task<bool> ResolveConflictAsync(SyncConflict conflict, object serverEntity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }

    public class SyncConflict
    {
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string Reason { get; set; }
    }
}
