using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    public interface IApplicationLogSyncService
    {
        Task<int> SyncPendingToServerAsync(CancellationToken cancellationToken = default);
        Task InsertToServerAsync(ApplicationLogEntry entry, CancellationToken cancellationToken = default);
        Task InsertBatchToServerAsync(IReadOnlyList<ApplicationLogEntry> entries, CancellationToken cancellationToken = default);
        Task<int> DeleteByLogGuidsOnServerAsync(IReadOnlyList<string> logGuids, CancellationToken cancellationToken = default);
        Task<int> DeleteAllOnServerAsync(CancellationToken cancellationToken = default);
        Task<int> DeleteByIdsOnServerAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default);
        Task<int> DeleteFilteredOnServerAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            CancellationToken cancellationToken = default);
    }
}
