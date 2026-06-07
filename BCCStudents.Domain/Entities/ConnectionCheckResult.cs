namespace BCCStudents.Domain.Entities
{
    public sealed class ConnectionCheckResult
    {
        public bool IsConnected { get; init; }
        public ConnectionFailureInfo? Failure { get; init; }

        public static ConnectionCheckResult Ok() => new() { IsConnected = true };

        public static ConnectionCheckResult Failed(ConnectionFailureInfo failure) =>
            new() { IsConnected = false, Failure = failure };
    }
}
