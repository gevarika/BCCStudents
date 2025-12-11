using System;

namespace BCCStudents.Domain.Entities
{
    public class SyncStateRecord
    {
        public string TableName { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public int LastSyncedId { get; set; }
    }
}



