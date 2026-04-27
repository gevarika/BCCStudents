namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// აღმოჩენილი ფაილის ინფორმაცია
    /// </summary>
    public class DetectedFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    /// <summary>
    /// იმპორტირებული ფაილის ინფორმაცია
    /// </summary>
    public class ImportedFileInfo
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public long FileSize { get; set; }
        public DateTime ImportedAt { get; set; }
        public string ImportedBy { get; set; }
        public string ComputerName { get; set; }
    }
}
/// <summary>
/// ფაილების ტრეკინგის რეპოზიტორიის ინტერფეისი
/// </summary>
