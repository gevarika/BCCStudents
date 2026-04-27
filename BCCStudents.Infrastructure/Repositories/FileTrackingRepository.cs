using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace BCCStudents.Infrastructure.Repositories
{
    /// <summary>
    /// ფაილების ტრეკინგის რეპოზიტორიის იმპლემენტაცია
    /// მუშაობს სერვერის ბაზასთან
    /// </summary>
    public class FileTrackingRepository : IFileTrackingRepository
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public FileTrackingRepository(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        /// <summary>
        /// შეამოწმებს არის თუ არა ფაილი უკვე იმპორტირებული hash-ის მიხედვით
        /// </summary>
        public async Task<bool> IsFileAlreadyImportedAsync(string fileHash)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
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
        /// ლოგირებს იმპორტირებულ ფაილს
        /// </summary>
        public async Task LogImportedFileAsync(ImportedFileInfo fileInfo)
        {
            using (var connection = _connectionProvider.GetLocalConnection())
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
        /// აბრუნებს იმპორტირებული ფაილების სიას
        /// </summary>
        public async Task<List<ImportedFileInfo>> GetImportedFilesAsync()
        {
            var files = new List<ImportedFileInfo>();

            using (var connection = _connectionProvider.GetLocalConnection())
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
        /// აბრუნებს კონკრეტული კომპიუტერის იმპორტირებული ფაილების სიას
        /// </summary>
        public async Task<List<ImportedFileInfo>> GetImportedFilesByComputerAsync(string computerName)
        {
            var files = new List<ImportedFileInfo>();

            using (var connection = _connectionProvider.GetLocalConnection())
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

