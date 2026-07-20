using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Sync.UpStream
{
    public sealed class UpStreamSyncWatermarkAdvancer : IUpStreamSyncWatermarkAdvancer
    {
        private readonly IDownStreamSyncRepository _downStreamSyncRepository;

        public UpStreamSyncWatermarkAdvancer(IDownStreamSyncRepository downStreamSyncRepository)
        {
            _downStreamSyncRepository = downStreamSyncRepository
                ?? throw new ArgumentNullException(nameof(downStreamSyncRepository));
        }

        public async Task AdvanceAfterSuccessfulUpStreamAsync(SyncChangePayload payload, CancellationToken cancellationToken = default)
        {
            if (payload == null || payload.Operation == SyncOperationType.Delete)
            {
                return;
            }

            if (!SyncLastWriteWinHelper.TryResolveWatermark(payload.TableName, payload.Data, out var syncedAt, out var syncedId))
            {
                return;
            }

            var state = await _downStreamSyncRepository
                .GetSyncStateAsync(payload.TableName, cancellationToken)
                .ConfigureAwait(false);

            if (state != null && !SyncLastWriteWinHelper.IsAhead(syncedAt, syncedId, state.LastSyncedAt, state.LastSyncedId))
            {
                return;
            }

            await _downStreamSyncRepository
                .UpdateSyncStateAsync(payload.TableName, syncedAt, syncedId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
