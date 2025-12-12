using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Application.Services.AutoFileDetection
{
    /// <summary>
    /// áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒáƒ¦áƒ›áƒáƒ©áƒ”áƒœáƒ˜áƒ¡ áƒ¡áƒ”áƒ áƒ•áƒ˜áƒ¡áƒ˜
    /// áƒ›áƒáƒœáƒ˜áƒ¢áƒáƒ áƒ”áƒ‘áƒ¡ áƒ¬áƒ˜áƒœáƒáƒ¡áƒ¬áƒáƒ  áƒ’áƒáƒœáƒ¡áƒáƒ–áƒ¦áƒ•áƒ áƒ£áƒš áƒ¡áƒáƒ¥áƒáƒ¦áƒáƒšáƒ“áƒ”áƒ¡ áƒáƒ®áƒáƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
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
        /// áƒáƒ¦áƒ›áƒáƒáƒ©áƒ”áƒœáƒ¡ áƒáƒ®áƒáƒš áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ¡ áƒ¡áƒáƒ¥áƒáƒ¦áƒáƒšáƒ“áƒ”áƒ¨áƒ˜
        /// áƒ›áƒ®áƒáƒšáƒáƒ“ áƒ”áƒ áƒ—áƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒáƒ¦áƒ›áƒáƒáƒ©áƒ”áƒœáƒ¡ (áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜ áƒœáƒáƒžáƒáƒ•áƒœáƒ˜)
        /// </summary>
        /// <returns>áƒáƒ®áƒáƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¡áƒ˜áƒ (áƒ›áƒáƒ¥áƒ¡áƒ˜áƒ›áƒ£áƒ› 1 áƒ”áƒšáƒ”áƒ›áƒ”áƒœáƒ¢áƒ˜)</returns>
        public async Task<List<DetectedFile>> DetectNewFilesAsync()
        {
            var newFiles = new List<DetectedFile>();

            // áƒ¨áƒ”áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒáƒ— áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¤áƒ£áƒœáƒ¥áƒªáƒ˜áƒ áƒ©áƒáƒ áƒ—áƒ£áƒšáƒ˜
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
                // áƒ›áƒáƒ•áƒ«áƒ”áƒ‘áƒœáƒáƒ— áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜ áƒ¨áƒ”áƒ¡áƒáƒ‘áƒáƒ›áƒ˜áƒ¡áƒ˜ áƒœáƒ˜áƒ›áƒ£áƒ¨áƒ˜áƒ—
                var files = Directory.GetFiles(_config.WatchFolderPath, _config.FileNamePattern, SearchOption.TopDirectoryOnly);

                foreach (var filePath in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(filePath);

                        // áƒ¨áƒ”áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ¡ áƒ›áƒ®áƒáƒ áƒ“áƒáƒ­áƒ”áƒ áƒ˜áƒš áƒ’áƒáƒ¤áƒáƒ áƒ—áƒáƒ”áƒ‘áƒáƒ¡
                        if (!_config.SupportedExtensions.Contains(fileInfo.Extension.ToLower()))
                            continue;

                        // áƒ’áƒáƒ›áƒáƒ•áƒ—áƒ•áƒáƒšáƒáƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ hash
                        var fileHash = await CalculateFileHashAsync(filePath);

                        // áƒ¨áƒ”áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒáƒ— áƒ£áƒ™áƒ•áƒ” áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ áƒ—áƒ£ áƒáƒ áƒ
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
                            
                            // áƒ›áƒ®áƒáƒšáƒáƒ“ áƒ”áƒ áƒ—áƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒáƒ¦áƒ›áƒáƒáƒ©áƒ”áƒœáƒ¡
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ˜áƒ¡áƒ
                        Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡ {filePath}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¡áƒáƒ¥áƒáƒ¦áƒáƒšáƒ“áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡ {_config.WatchFolderPath}: {ex.Message}");
            }

            return newFiles;
        }

        /// <summary>
        /// áƒ’áƒáƒ›áƒáƒ—áƒ•áƒšáƒ˜áƒ¡ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ MD5 hash-áƒ¡
        /// </summary>
        /// <param name="filePath">áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒ–áƒ</param>
        /// <returns>áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ hash</returns>
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
        /// áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ¡ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒš áƒ¤áƒáƒ˜áƒšáƒ¡
        /// </summary>
        /// <param name="filePath">áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒ–áƒ</param>
        /// <param name="importedBy">áƒ•áƒ˜áƒœ áƒ’áƒáƒáƒ™áƒ”áƒ—áƒ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜</param>
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
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡ {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// áƒ¨áƒ”áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ¡ áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ£áƒ™áƒ•áƒ” áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜
        /// </summary>
        /// <param name="filePath">áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒ–áƒ</param>
        /// <returns>true áƒ—áƒ£ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ£áƒ™áƒ•áƒ” áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ</returns>
        public async Task<bool> IsFileAlreadyImportedAsync(string filePath)
        {
            try
            {
                var fileHash = await CalculateFileHashAsync(filePath);
                return await _fileTrackingRepository.IsFileAlreadyImportedAsync(fileHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡ {filePath}: {ex.Message}");
                return false;
            }
        }
    }
}

