using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Infrastructure.Repositories
{
    /// <summary>
    /// áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¢áƒ áƒ”áƒ™áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ áƒ”áƒžáƒáƒ–áƒ˜áƒ¢áƒáƒ áƒ˜áƒ˜áƒ¡ áƒ˜áƒ›áƒžáƒšáƒ”áƒ›áƒ”áƒœáƒ¢áƒáƒªáƒ˜áƒ
    /// áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ¡ áƒ¡áƒ”áƒ áƒ•áƒ”áƒ áƒ˜áƒ¡ áƒ‘áƒáƒ–áƒáƒ¡áƒ—áƒáƒœ
    /// </summary>
    public class FileTrackingRepository : IFileTrackingRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public FileTrackingRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// áƒ¨áƒ”áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ¡ áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ£áƒ™áƒ•áƒ” áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ hash-áƒ˜áƒ¡ áƒ›áƒ˜áƒ®áƒ”áƒ“áƒ•áƒ˜áƒ—
        /// </summary>
        public async Task<bool> IsFileAlreadyImportedAsync(string fileHash)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                await connection.OpenAsync();
                
                var query = @"SELECT COUNT(*) FROM ImportedFilesLog WHERE FileHash = @fileHash";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@fileHash", fileHash);
                    var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// áƒšáƒáƒ’áƒ˜áƒ áƒ”áƒ‘áƒ¡ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒš áƒ¤áƒáƒ˜áƒšáƒ¡
        /// </summary>
        public async Task LogImportedFileAsync(ImportedFileInfo fileInfo)
        {
            using (var connection = _dbHelper.GetLocalConnection())
            {
                await connection.OpenAsync();
                
                var query = @"INSERT INTO ImportedFilesLog 
                            (FilePath, FileName, FileHash, FileSize, ImportedAt, ImportedBy, ComputerName) 
                            VALUES (@filePath, @fileName, @fileHash, @fileSize, @importedAt, @importedBy, @computerName)";
                
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@filePath", fileInfo.FilePath);
                    cmd.Parameters.AddWithValue("@fileName", fileInfo.FileName);
                    cmd.Parameters.AddWithValue("@fileHash", fileInfo.FileHash);
                    cmd.Parameters.AddWithValue("@fileSize", fileInfo.FileSize);
                    cmd.Parameters.AddWithValue("@importedAt", fileInfo.ImportedAt);
                    cmd.Parameters.AddWithValue("@importedBy", fileInfo.ImportedBy);
                    cmd.Parameters.AddWithValue("@computerName", fileInfo.ComputerName);
                    
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ¡ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¡áƒ˜áƒáƒ¡
        /// </summary>
        public async Task<List<ImportedFileInfo>> GetImportedFilesAsync()
        {
            var files = new List<ImportedFileInfo>();
            
            using (var connection = _dbHelper.GetLocalConnection())
            {
                await connection.OpenAsync();
                
                var query = @"SELECT FilePath, FileName, FileHash, FileSize, ImportedAt, ImportedBy, ComputerName 
                            FROM ImportedFilesLog 
                            ORDER BY ImportedAt DESC";
                
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        files.Add(new ImportedFileInfo
                        {
                            FilePath = reader["FilePath"]?.ToString() ?? "",
                            FileName = reader["FileName"]?.ToString() ?? "",
                            FileHash = reader["FileHash"]?.ToString() ?? "",
                            FileSize = Convert.ToInt64(reader["FileSize"]),
                            ImportedAt = Convert.ToDateTime(reader["ImportedAt"]),
                            ImportedBy = reader["ImportedBy"]?.ToString() ?? "",
                            ComputerName = reader["ComputerName"]?.ToString() ?? ""
                        });
                    }
                }
            }
            
            return files;
        }

        /// <summary>
        /// áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ¡ áƒ™áƒáƒœáƒ™áƒ áƒ”áƒ¢áƒ£áƒšáƒ˜ áƒ™áƒáƒ›áƒžáƒ˜áƒ£áƒ¢áƒ”áƒ áƒ˜áƒ¡ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¡áƒ˜áƒáƒ¡
        /// </summary>
        public async Task<List<ImportedFileInfo>> GetImportedFilesByComputerAsync(string computerName)
        {
            var files = new List<ImportedFileInfo>();
            
            using (var connection = _dbHelper.GetLocalConnection())
            {
                await connection.OpenAsync();
                
                var query = @"SELECT FilePath, FileName, FileHash, FileSize, ImportedAt, ImportedBy, ComputerName 
                            FROM ImportedFilesLog 
                            WHERE ComputerName = @computerName
                            ORDER BY ImportedAt DESC";
                
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@computerName", computerName);
                    
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            files.Add(new ImportedFileInfo
                            {
                                FilePath = reader["FilePath"]?.ToString() ?? "",
                                FileName = reader["FileName"]?.ToString() ?? "",
                                FileHash = reader["FileHash"]?.ToString() ?? "",
                                FileSize = Convert.ToInt64(reader["FileSize"]),
                                ImportedAt = Convert.ToDateTime(reader["ImportedAt"]),
                                ImportedBy = reader["ImportedBy"]?.ToString() ?? "",
                                ComputerName = reader["ComputerName"]?.ToString() ?? ""
                            });
                        }
                    }
                }
            }
            
            return files;
        }
    }
}

