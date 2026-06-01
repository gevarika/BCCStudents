using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    public interface IUpStreamSyncService
    {
        Task<UpStreamSyncResult> TrySyncImmediatelyAsync(SyncChangePayload payload, CancellationToken cancellationToken = default);
    }
}

