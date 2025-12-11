using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    public interface IUpStreamSyncRepository
    {
        Task<long> EnqueueChangeAsync(SyncChangePayload payload, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SyncOutboxItem>> GetPendingItemsAsync(int limit = 50, CancellationToken cancellationToken = default);
        Task MarkAsSuccessAsync(long outboxId, CancellationToken cancellationToken = default);
        Task MarkAsFailedAsync(long outboxId, string errorMessage, bool giveUp, CancellationToken cancellationToken = default);
    }
}




