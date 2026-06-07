using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IApplicationLogRepository
    {
        Task InsertAsync(ApplicationLogEntry entry, CancellationToken cancellationToken = default);
        Task InsertBatchAsync(IReadOnlyList<ApplicationLogEntry> entries, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationLogEntry>> GetFilteredAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            bool isAdmin,
            bool canViewSystemLogs,
            int currentUserId,
            IReadOnlyList<string> allowedPermissionScopes,
            IReadOnlyList<string> allowedCategories,
            int limit,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationLogEntry>> GetUnsyncedBatchAsync(int limit, CancellationToken cancellationToken = default);
        Task MarkSyncedAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default);
        Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken cancellationToken = default);
        Task<int> InsertFromServerAsync(IReadOnlyList<ApplicationLogEntry> entries, CancellationToken cancellationToken = default);
        Task<int> DeleteAllAsync(CancellationToken cancellationToken = default);
        Task<int> DeleteByIdsAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default);
        Task<int> DeleteFilteredAsync(
            DateTime? from,
            DateTime? to,
            string sourceType,
            string category,
            string level,
            string username,
            string operation,
            string searchText,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetLogGuidsByIdsAsync(IReadOnlyList<long> ids, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetLogGuidsFilteredAsync(
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
