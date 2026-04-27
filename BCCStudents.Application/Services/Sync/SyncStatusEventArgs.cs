namespace BCCStudents.Application.Services.Sync
{
    /// <summary>
    /// Event arguments for sync status updates
    /// </summary>
    public class SyncStatusEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public int RecordsSynced { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string SyncType { get; set; } // "DownStream" or "UpStream"
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}


