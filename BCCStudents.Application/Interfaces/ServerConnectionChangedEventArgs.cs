using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    public sealed class ServerConnectionChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; init; }
        public ConnectionFailureInfo? Failure { get; init; }
    }
}
