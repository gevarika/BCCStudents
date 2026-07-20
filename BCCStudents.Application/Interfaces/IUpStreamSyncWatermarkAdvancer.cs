using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// UpStream-ის წარმატების შემდეგ DownStream SyncState watermark-ის monotonic განახლება.
    /// </summary>
    public interface IUpStreamSyncWatermarkAdvancer
    {
        Task AdvanceAfterSuccessfulUpStreamAsync(SyncChangePayload payload, CancellationToken cancellationToken = default);
    }
}
