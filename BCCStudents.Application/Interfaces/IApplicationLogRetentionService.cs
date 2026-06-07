namespace BCCStudents.Application.Interfaces
{
    public interface IApplicationLogRetentionService : IDisposable
    {
        void Start();
        void Stop();
        Task<int> RunCleanupAsync(CancellationToken cancellationToken = default);
    }
}
