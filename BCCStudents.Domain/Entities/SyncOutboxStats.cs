namespace BCCStudents.Domain.Entities
{
    public sealed class SyncOutboxStats
    {
        public int PendingCount { get; set; }
        public int DeadLetterCount { get; set; }
    }
}
