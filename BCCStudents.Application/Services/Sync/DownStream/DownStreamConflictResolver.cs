using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    public class DownStreamConflictResolver : IDownStreamConflictResolver
    {
        public Task<IReadOnlyList<SyncConflict>> DetectConflictsAsync<T>(string tableName, IReadOnlyList<T> serverData, CancellationToken cancellationToken = default)
        {
            // საწყისი ვერსიისთვის არ ვატარებთ კონფლიქტის შემოწმებას (ServerWins).
            return Task.FromResult<IReadOnlyList<SyncConflict>>(new List<SyncConflict>());
        }

        public Task<bool> ResolveConflictAsync(SyncConflict conflict, object serverEntity, CancellationToken cancellationToken = default)
        {
            // ServerWins – უბრალოდ ვაბრუნებთ true-ს, რადგან სერვერის მონაცემებს ვიყენებთ.
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


