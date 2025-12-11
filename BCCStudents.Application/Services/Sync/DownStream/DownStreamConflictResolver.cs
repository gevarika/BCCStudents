using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    public class DownStreamConflictResolver : IDownStreamConflictResolver
    {
        public Task<IReadOnlyList<SyncConflict>> DetectConflictsAsync<T>(string tableName, IReadOnlyList<T> serverData, CancellationToken cancellationToken = default)
        {
            // áƒ¡áƒáƒ¬áƒ§áƒ˜áƒ¡áƒ˜ áƒ•áƒ”áƒ áƒ¡áƒ˜áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ áƒáƒ  áƒ•áƒáƒ¢áƒáƒ áƒ”áƒ‘áƒ— áƒ™áƒáƒœáƒ¤áƒšáƒ˜áƒ¥áƒ¢áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒáƒ¡ (ServerWins).
            return Task.FromResult<IReadOnlyList<SyncConflict>>(new List<SyncConflict>());
        }

        public Task<bool> ResolveConflictAsync(SyncConflict conflict, object serverEntity, CancellationToken cancellationToken = default)
        {
            // ServerWins â€“ áƒ£áƒ‘áƒ áƒáƒšáƒáƒ“ áƒ•áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ— true-áƒ¡, áƒ áƒáƒ“áƒ’áƒáƒœ áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ˜áƒ¡ áƒ›áƒáƒœáƒáƒªáƒ”áƒ›áƒ”áƒ‘áƒ¡ áƒ•áƒ˜áƒ§áƒ”áƒœáƒ”áƒ‘áƒ—.
            return Task.FromResult(true);
        }
    }

    public class SyncConflict
    {
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string Reason { get; set; }
    }
}


