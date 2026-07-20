using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Enums;

namespace BCCStudents.Application.Services.Logging
{
    public sealed class LogStorageSettingsService : ILogStorageSettings
    {
        private readonly object _gate = new();
        private LogStorageTarget _currentTarget = LogStorageTarget.Local;

        public LogStorageTarget CurrentTarget
        {
            get
            {
                lock (_gate)
                    return _currentTarget;
            }
        }

        public bool IsLocal => CurrentTarget == LogStorageTarget.Local;
        public bool IsServer => CurrentTarget == LogStorageTarget.Server;

        public event EventHandler TargetChanged;

        public void Initialize(LogStorageTarget target)
        {
            ApplyTarget(target, raiseEvent: false);
        }

        public void SetTarget(LogStorageTarget target)
        {
            ApplyTarget(target, raiseEvent: true);
        }

        private void ApplyTarget(LogStorageTarget target, bool raiseEvent)
        {
            LogStorageTarget previous;
            lock (_gate)
            {
                previous = _currentTarget;
                _currentTarget = target;
            }

            if (raiseEvent && previous != target)
                TargetChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
