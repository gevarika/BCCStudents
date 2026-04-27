using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Security.Cryptography;

namespace BCCStudents.Application.Services.AutoFileDetection
{
    /// <summary>
    /// ავტომატური ფაილის აღმოჩენის სერვისი
    /// მონიტორებს წინასწარ განსაზღვრულ საქაღალდეში ახალი გადასახდების ფაილებისთვის
    /// </summary>
    public class AutoFileDetectionService
    {
        private readonly IFileTrackingRepository _fileTrackingRepository;
        private readonly AutoFileDetectionConfig _config;

        public AutoFileDetectionService(IFileTrackingRepository fileTrackingRepository, AutoFileDetectionConfig config)
        {
            _fileTrackingRepository = fileTrackingRepository;
            _config = config;
        }

        /// <summary>
        /// აღმოაჩენს ახალ გადასახდების ფაილებს საქაღალდეში
        /// მხოლოდ ერთ ფაილს აბრუნებს (პირველი ნაპოვნი)
        /// </summary>
        /// <returns>ახალი ფაილების სია (მაქსიმუმ 1 ელემენტი)</returns>
        public async Task<List<DetectedFile>> DetectNewFilesAsync()
        {
            var newFiles = new List<DetectedFile>();

            // შევამოწმოთ, არის თუ არა ფუნქცია ჩართული
            if (!_config.Enabled)
            {
                return newFiles;
            }

            if (!Directory.Exists(_config.WatchFolderPath))
            {
                return newFiles;
            }

            try
            {
                // მოვძებნოთ ფაილები შესაბამისი ნიმუშით
                var files = Directory.GetFiles(_config.WatchFolderPath, _config.FileNamePattern, SearchOption.TopDirectoryOnly);

                foreach (var filePath in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(filePath);

                        // შევამოწმოთ მხარდაჭერილი გაფართოება
                        if (!_config.SupportedExtensions.Contains(fileInfo.Extension.ToLower()))
                            continue;

                        // გამოვთვალოთ ფაილის hash
                        var fileHash = await CalculateFileHashAsync(filePath);

                        // შევამოწმოთ უკვე იმპორტირებულია თუ არა
                        if (!await _fileTrackingRepository.IsFileAlreadyImportedAsync(fileHash))
                        {
                            newFiles.Add(new DetectedFile
                            {
                                FilePath = filePath,
                                FileName = fileInfo.Name,
                                FileSize = fileInfo.Length,
                                FileHash = fileHash,
                                CreatedAt = fileInfo.CreationTime,
                                ModifiedAt = fileInfo.LastWriteTime
                            });

                            // მხოლოდ ერთი ფაილი აღმოაჩენს
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // ლოგირება შეცდომისას
                        Console.WriteLine($"შეცდომა ფაილის შემოწმებისას {filePath}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა საქაღალდის შემოწმებისას {_config.WatchFolderPath}: {ex.Message}");
            }

            return newFiles;
        }

        /// <summary>
        /// გამოითვლის ფაილის MD5 hash-ს
        /// </summary>
        /// <param name="filePath">ფაილის გზა</param>
        /// <returns>ფაილის hash</returns>
        private async Task<string> CalculateFileHashAsync(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = await Task.Run(() => md5.ComputeHash(stream));
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// ლოგირებს იმპორტირებულ ფაილს
        /// </summary>
        /// <param name="filePath">ფაილის გზა</param>
        /// <param name="importedBy">ვინ გააკეთა იმპორტი</param>
        public async Task LogImportedFileAsync(string filePath, string importedBy)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var fileHash = await CalculateFileHashAsync(filePath);
                var computerName = Environment.MachineName;

                await _fileTrackingRepository.LogImportedFileAsync(new ImportedFileInfo
                {
                    FilePath = filePath,
                    FileName = fileInfo.Name,
                    FileHash = fileHash,
                    FileSize = fileInfo.Length,
                    ImportedAt = DateTime.Now,
                    ImportedBy = importedBy,
                    ComputerName = computerName
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ფაილის ლოგირებისას {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// შეამოწმებს არის თუ არა ფაილი უკვე იმპორტირებული
        /// </summary>
        /// <param name="filePath">ფაილის გზა</param>
        /// <returns>true თუ ფაილი უკვე იმპორტირებულია</returns>
        public async Task<bool> IsFileAlreadyImportedAsync(string filePath)
        {
            try
            {
                var fileHash = await CalculateFileHashAsync(filePath);
                return await _fileTrackingRepository.IsFileAlreadyImportedAsync(fileHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ფაილის შემოწმებისას {filePath}: {ex.Message}");
                return false;
            }
        }
    }
}

