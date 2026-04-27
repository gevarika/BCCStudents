using BCCStudents.Domain.Entities;

namespace BCCStudents.Application.Interfaces
{
    public interface IBackupService
    {
        // პროპერტი ბაზის ცვლილების საკონტროლოდ
        bool DbChangedSinceLastBackup { get; set; }
        // მექანიკური ბექაპი (მაგ. ღილაკზე დაჭერისას)
        bool CreateBackup(string backupFilePath);

        // ძირითადი მეთოდები
        bool CreateMySQLBackup(string backupFilePath);
        bool RestoreBackup(string backupFilePath);

        // პერიოდული ბექაპის მართვა
        void InitializePeriodicBackup();
        void StopPeriodicBackup();

        // მონაცემების მიღება
        List<BackupInfo> GetBackupList();

        // ფაილების მართვა
        //void ManualBackup(string dbFilePath);
        //void ManualRestore(string dbFilePath);
    }
}
