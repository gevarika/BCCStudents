using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IUpStreamSyncService
    {
        Task<bool> TrySyncImmediatelyAsync(SyncChangePayload payload, CancellationToken cancellationToken = default);
    }
}




