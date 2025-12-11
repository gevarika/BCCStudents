using System.Collections.Generic;
using System.Threading.Tasks;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Domain.Interfaces
{
    /// <summary>
    /// ფაილების ტრეკინგის რეპოზიტორიის ინტერფეისი
    /// </summary>
    public interface IFileTrackingRepository
    {
        /// <summary>
        /// შეამოწმებს არის თუ არა ფაილი უკვე იმპორტირებული hash-ის მიხედვით
        /// </summary>
        /// <param name="fileHash">ფაილის hash</param>
        /// <returns>true თუ ფაილი უკვე იმპორტირებულია</returns>
        Task<bool> IsFileAlreadyImportedAsync(string fileHash);

        /// <summary>
        /// ლოგირებს იმპორტირებულ ფაილს
        /// </summary>
        /// <param name="fileInfo">ფაილის ინფორმაცია</param>
        Task LogImportedFileAsync(ImportedFileInfo fileInfo);

        /// <summary>
        /// აბრუნებს იმპორტირებული ფაილების სიას
        /// </summary>
        /// <returns>იმპორტირებული ფაილების სია</returns>
        Task<List<ImportedFileInfo>> GetImportedFilesAsync();

        /// <summary>
        /// აბრუნებს კონკრეტული კომპიუტერის იმპორტირებული ფაილების სიას
        /// </summary>
        /// <param name="computerName">კომპიუტერის სახელი</param>
        /// <returns>იმპორტირებული ფაილების სია</returns>
        Task<List<ImportedFileInfo>> GetImportedFilesByComputerAsync(string computerName);
    }
}
