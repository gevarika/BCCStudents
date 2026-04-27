using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IDownStreamSyncService
    {
        Task<SyncResult> SyncFromServerAsync(CancellationToken cancellationToken = default);
        Task<TableSyncResult> SyncTableAsync(string tableName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetTablesToSyncAsync();
    }
}




