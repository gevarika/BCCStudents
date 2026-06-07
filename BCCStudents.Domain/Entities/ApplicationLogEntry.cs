namespace BCCStudents.Domain.Entities
{
    public class ApplicationLogEntry
    {
        public long Id { get; set; }
        public string LogGuid { get; set; }
        public string SourceType { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Operation { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string MachineName { get; set; }
        public string PermissionScope { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string Exception { get; set; }
        public string SourceContext { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SyncedToServerAt { get; set; }
        public string Origin { get; set; } = "Local";
    }
}
