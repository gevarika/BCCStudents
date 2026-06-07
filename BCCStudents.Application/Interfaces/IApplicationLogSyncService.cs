namespace BCCStudents.Application.Interfaces
{
    public interface IApplicationLogSyncService
    {
        Task<int> SyncPendingToServerAsync(CancellationToken cancellationToken = default);
        Task<int> DeleteByLogGuidsOnServerAsync(IReadOnlyList<string> logGuids, CancellationToken cancellationToken = default);
        Task<int> DeleteAllOnServerAsync(CancellationToken cancellationToken = default);
    }
}
