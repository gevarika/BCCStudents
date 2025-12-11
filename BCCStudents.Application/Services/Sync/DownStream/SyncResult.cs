using System.Collections.Generic;
using System.Linq;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Services.Sync.DownStream
{
    public class SyncResult
    {
        private readonly List<TableSyncResult> _tables = new List<TableSyncResult>();
        private readonly List<string> _errors = new List<string>();

        public bool Success => _errors.Count == 0 && _tables.All(t => t.Success);
        public IReadOnlyList<TableSyncResult> Tables => _tables;
        public IReadOnlyList<string> Errors => _errors;

        public void AddTableResult(TableSyncResult result)
        {
            if (result != null)
            {
                _tables.Add(result);
            }
        }

        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                _errors.Add(error);
            }
        }
    }
}


