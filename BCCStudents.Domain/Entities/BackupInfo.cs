namespace BCCStudents.Domain.Entities
{
    public class BackupInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreationTime { get; set; }
        public long FileSize { get; set; }
        public string FileSizeFormatted { get; set; }
    }
}
