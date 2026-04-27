using BCCStudents.Application.Services.Sync.DownStream;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// DownStream კონფლიქტების გადამწყვეტის ინტერფეისი
    /// </summary>
    public interface IDownStreamConflictResolver
    {
        /// <summary>
        /// ამოიცნობს კონფლიქტებს
        /// </summary>
        Task<IReadOnlyList<SyncConflict>> DetectConflictsAsync<T>(string tableName, IReadOnlyList<T> serverData, CancellationToken cancellationToken = default);

        /// <summary>
        /// წყვეტს კონფლიქტს
        /// </summary>
        Task<bool> ResolveConflictAsync(SyncConflict conflict, object serverEntity, CancellationToken cancellationToken = default);
    }
}

