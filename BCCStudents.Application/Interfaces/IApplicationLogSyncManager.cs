namespace BCCStudents.Application.Interfaces
{
    public interface IApplicationLogSyncManager : IDisposable
    {
        void Start();
        void Stop();
    }
}
