namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// სერვერთან კავშირის შეცდომის აღწერა: UserMessage — UI, LogDetail — ლოგი.
    /// </summary>
    public sealed class ConnectionFailureInfo
    {
        public ConnectionFailureCategory Category { get; init; }
        public string UserMessage { get; init; } = string.Empty;
        public string LogDetail { get; init; } = string.Empty;
        public int? MySqlErrorNumber { get; init; }
        public string? ExceptionType { get; init; }
    }
}
