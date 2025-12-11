using System;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    public class SyncStateRecord
    {
        public string TableName { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public int LastSyncedId { get; set; }
    }
}


