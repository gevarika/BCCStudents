using BCCStudents.Domain.Enums;

namespace BCCStudents.Application.Interfaces
{
    public interface ILogStorageSettings
    {
        LogStorageTarget CurrentTarget { get; }
        bool IsLocal { get; }
        bool IsServer { get; }
        event EventHandler TargetChanged;
        void Initialize(LogStorageTarget target);
        void SetTarget(LogStorageTarget target);
    }
}
