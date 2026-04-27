using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Timers;

namespace BCCStudents.Infrastructure.Services
{
    public class BackupService : IBackupService
    {
        private readonly IConfigurationService _config;

        public BackupService(IConfigurationService config)
        {
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        // ცვლილების ფლაგი
        public bool DbChangedSinceLastBackup
        {
            get => _config.DbChangedSinceLastBackup;
            set => _config.DbChangedSinceLastBackup = value;
        }

        // ბოლო backup-ის დრო (სურვილისამებრ)
        public DateTime LastBackupTime
        {
            get => _config.LastBackupTime;
            set => _config.LastBackupTime = value;
        }

        // პერიოდული ბექაპის ტაიმერი
        private System.Timers.Timer _backupTimer;

        // ბექაპის კონფიგურაცია
        public string BackupDirectory
        {
            get => _config.BackupDirectory;
            set => _config.BackupDirectory = value;
        }
        public int BackupIntervalMinutes
        {
            get => _config.BackupIntervalMinutes;
            set => _config.BackupIntervalMinutes = value;
        }
        public int BackupIntervalHours
        {
            get => _config.BackupIntervalHours;
            set => _config.BackupIntervalHours = value;
        }
        public int MaxBackupFiles
        {
            get => _config.MaxBackupFiles;
            set => _config.MaxBackupFiles = value;
        }
        public bool AutoBackupEnabled { get; set; } = true;

        // MySQL კონფიგურაცია
        private string _mySqlDumpPath;
        public string MySqlDumpPath
        {
            get
            {
                // ვამოწმებთ არის თუ არა ფაილი ჯერ კიდევ არსებულად
                // ეს მნიშვნელოვანია რადგან ფაილი შეიძლება წაიშალოს პროგრამის მუშაობის დროს
                if (string.IsNullOrEmpty(_mySqlDumpPath) || !File.Exists(_mySqlDumpPath))
                {
                    // პირველ რიგში ვცდილობთ პროგრამის ფოლდერში
                    string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string localDumpPath = Path.Combine(exeDir, "mysqldump.exe");

                    if (File.Exists(localDumpPath))
                    {
                        _mySqlDumpPath = localDumpPath;
                    }
                    else
                    {
                        // თუ პროგრამის ფოლდერში არ არის, ვცდილობთ სტანდარტულ ბილიკებში
                        string[] possiblePaths = {
                            @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
                            @"C:\Program Files\MySQL\MySQL Server 5.7\bin\mysqldump.exe",
                            @"C:\Program Files\MySQL\MySQL Server 9.3\bin\mysqldump.exe",
                            @"C:\xampp\mysql\bin\mysqldump.exe"
                        };

                        foreach (string path in possiblePaths)
                        {
                            if (File.Exists(path))
                            {
                                _mySqlDumpPath = path;
                                break;
                            }
                        }
                    }
                }
                return _mySqlDumpPath;
            }
            set => _mySqlDumpPath = value;
        }
        // სერვერის ჰოსტი/ბაზის სახელი უკვე შენახულია სრული connection string-ით Settings-ში.
        // აქ ვკითხულობთ ServerMySqlConnectionString(_Test)-დან და ვშლით MySqlConnectionStringBuilder-ით.
        private string GetActiveServerConnectionString()
        {
            bool isTest = _config.IsTestDb;
            string prod = _config.ServerMySqlConnectionString;
            string test = _config.ServerMySqlConnectionString_Test;

            var conn = isTest
                ? (string.IsNullOrWhiteSpace(test) ? prod : test)
                : prod;

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("ServerMySqlConnectionString( _Test ) is not configured in Settings.");

            return conn;
        }

        private MySql.Data.MySqlClient.MySqlConnectionStringBuilder GetServerConnectionBuilder()
        {
            return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(GetActiveServerConnectionString());
        }

        public string ServerHost
        {
            get => GetServerConnectionBuilder().Server;
        }

        public string DatabaseName
        {
            get => GetServerConnectionBuilder().Database;
        }
        public string Username
        {
            get => _config.BackupUsername;
            set => _config.BackupUsername = value;
        }
        public string Password
        {
            get => _config.BackupPassword;
            set => _config.BackupPassword = value;
        }

        // --- პერიოდული ბექაპის ინიციალიზაცია ---
        public void InitializePeriodicBackup()
        {
            try
            {
                Console.WriteLine($"InitializePeriodicBackup - BackupDirectory: '{BackupDirectory}'");
                Console.WriteLine($"InitializePeriodicBackup - Application.StartupPath: '{System.Windows.Forms.Application.StartupPath}'");

                // შევქმნათ ბექაპის დირექტორია
                if (!Directory.Exists(BackupDirectory))
                {
                    Console.WriteLine($"Creating backup directory: {BackupDirectory}");
                    Directory.CreateDirectory(BackupDirectory);
                }

                // შევქმნათ ტაიმერი პერიოდული ბექაპისთვის
                _backupTimer = new System.Timers.Timer(BackupIntervalMinutes * 60 * 1000); // წუთებში
                _backupTimer.Elapsed += OnBackupTimerElapsed;
                _backupTimer.AutoReset = true;
                _backupTimer.Enabled = AutoBackupEnabled;

                // გავუშვათ პირველი ბექაპი თუ ჯერ არ გაკეთებულა
                if (LastBackupTime == DateTime.MinValue)
                {
                    CreatePeriodicBackup();
                }

                Console.WriteLine($"პერიოდული ბექაპი ინიციალიზებულია. ინტერვალი: {BackupIntervalMinutes} წუთი");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა პერიოდული ბექაპის ინიციალიზაციის დროს: {ex.Message}");
            }
        }

        // --- ტაიმერის ივენტი ---
        private void OnBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (AutoBackupEnabled)
            {
                CreatePeriodicBackup();
            }
        }

        // --- პერიოდული ბექაპის შექმნა ---
        public bool CreatePeriodicBackup()
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    Directory.CreateDirectory(BackupDirectory);
                }

                string backupFileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
                string backupFilePath = Path.Combine(BackupDirectory, backupFileName);

                // შევქმნათ ბექაპი
                bool success = CreateMySQLBackup(backupFilePath);

                if (success)
                {
                    _config.LastBackupTime = DateTime.Now;
                    _config.DbChangedSinceLastBackup = false;

                    // გავწმინდოთ ძველი ბექაპები
                    CleanupOldBackups();

                    Console.WriteLine($"პერიოდული ბექაპი შეიქმნა: {backupFileName}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა პერიოდული ბექაპის შექმნის დროს: {ex.Message}");
                return false;
            }
        }

        // --- MySQL ბექაპის შექმნა ---
        public bool CreateMySQLBackup(string backupFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(ServerHost) || string.IsNullOrEmpty(DatabaseName) ||
                    string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    return false;
                }
                if (!File.Exists(MySqlDumpPath))
                {
                    string errorMessage = "mysqldump.exe ვერ მოიძებნა!\n\n" +
                        "გთხოვთ, ჩაამატეთ mysqldump.exe და საჭირო DLL-ები პროგრამის ფოლდერში ან დააყენეთ MySQL Server.\n\n" +
                        "ძებნის ბილიკები:\n" +
                        "- პროგრამის ფოლდერი\n" +
                        "- C:\\Program Files\\MySQL\\MySQL Server 8.0\\bin\\\n" +
                        "- C:\\Program Files\\MySQL\\MySQL Server 5.7\\bin\\\n" +
                        "- C:\\xampp\\mysql\\bin\\";

                    MessageBox.Show(errorMessage, "ბექაპის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                string tempFileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
                string arguments = $"--host={ServerHost} --user={Username} --password={Password} --default-character-set=utf8mb4 --set-charset=utf8mb4 --hex-blob --single-transaction --routines --triggers --no-tablespaces --skip-lock-tables --result-file=\"{tempFilePath}\" {DatabaseName}";
                string backupDir = Path.GetDirectoryName(backupFilePath);
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                var startInfo = new ProcessStartInfo
                {
                    FileName = MySqlDumpPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        var tempFileInfo = new FileInfo(tempFilePath);
                        if (tempFileInfo.Length > 0)
                        {
                            try
                            {
                                if (File.Exists(backupFilePath))
                                {
                                    File.Delete(backupFilePath);
                                }
                                File.Move(tempFilePath, backupFilePath);
                                if (!File.Exists(backupFilePath))
                                {
                                    return false;
                                }
                                byte[] fileBytes = File.ReadAllBytes(backupFilePath);
                                string content = Encoding.UTF8.GetString(fileBytes);
                                int georgianChars = content.Count(c => c >= '\u10A0' && c <= '\u10FF');
                                if (georgianChars > 0)
                                {
                                    string tempGeorgianPath = Path.Combine(Path.GetTempPath(), $"georgian_{Path.GetFileName(backupFilePath)}");
                                    using (var writer = new StreamWriter(tempGeorgianPath, false, Encoding.UTF8))
                                    {
                                        writer.Write(content);
                                    }
                                    if (File.Exists(backupFilePath))
                                    {
                                        File.Delete(backupFilePath);
                                    }
                                    File.Move(tempGeorgianPath, backupFilePath);
                                }
                                return true;
                            }
                            catch (Exception moveEx)
                            {
                                backupFilePath = tempFilePath;
                                //CheckBackupFileEncoding(backupFilePath);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.WriteLine($"Error details: {error}");
                        }

                        // ვცადოთ უფრო მარტივი ვარიანტი
                        Console.WriteLine("Trying simpler backup command...");
                        string simpleArguments = $"--host={ServerHost} --user={Username} --password={Password} --default-character-set=utf8mb4 --set-charset=utf8mb4 {DatabaseName}";


                        var simpleStartInfo = new ProcessStartInfo
                        {
                            FileName = MySqlDumpPath,
                            Arguments = simpleArguments,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            StandardOutputEncoding = Encoding.UTF8 // ეს აუცილებელია!
                        };
                        /*using (var simpleProcess = new Process { StartInfo = simpleStartInfo })
                        {
                            simpleProcess.Start();
                            
                            string simpleOutput = simpleProcess.StandardOutput.ReadToEnd();
                            string simpleError = simpleProcess.StandardError.ReadToEnd();
                            
                            simpleProcess.WaitForExit();
                            
                            Console.WriteLine($"Simple process Exit Code: {simpleProcess.ExitCode}");
                            Console.WriteLine($"Simple process Error: {simpleError}");
                            
                            if (simpleProcess.ExitCode == 0 && !string.IsNullOrEmpty(simpleOutput))
                            {
                                // ვწერთ დროებით ფაილში UTF-8 BOM-ით
                                File.WriteAllText(tempFilePath, simpleOutput, new UTF8Encoding(true));
                                var tempFileInfo = new FileInfo(tempFilePath);
                                if (tempFileInfo.Length > 0)
                                {
                                    Console.WriteLine($"Simple backup successful. Size: {tempFileInfo.Length} bytes");
                                    
                                    // ვგადაგვაქვს ფაილი საბოლოო ლოკაციაზე
                                    try
                                    {
                                        if (File.Exists(backupFilePath))
                                        {
                                            File.Delete(backupFilePath);
                                        }
                                        
                                        File.Move(tempFilePath, backupFilePath);
                                        Console.WriteLine($"Simple backup moved to final location: {backupFilePath}");
                                        
                                        CheckBackupFileEncoding(backupFilePath);
                                        return true;
                                    }
                                    catch (Exception moveEx)
                                    {
                                        Console.WriteLine($"Error moving simple backup file: {moveEx.Message}");
                                        backupFilePath = tempFilePath;
                                        CheckBackupFileEncoding(backupFilePath);
                                        return true;
                                    }
                                }
                            }*/
                        using (var simpleProcess = new Process { StartInfo = simpleStartInfo })
                        {
                            simpleProcess.Start();

                            using (var writer = new StreamWriter(tempFilePath, false, new UTF8Encoding(true)))
                            {
                                while (!simpleProcess.StandardOutput.EndOfStream)
                                {
                                    writer.WriteLine(simpleProcess.StandardOutput.ReadLine());
                                }
                            }

                            string simpleError = simpleProcess.StandardError.ReadToEnd();
                            simpleProcess.WaitForExit();

                            Console.WriteLine($"Simple process Exit Code: {simpleProcess.ExitCode}");
                            Console.WriteLine($"Simple process Error: {simpleError}");

                            var tempFileInfo = new FileInfo(tempFilePath);
                            if (simpleProcess.ExitCode == 0 && tempFileInfo.Length > 0)
                            {
                                Console.WriteLine($"Simple backup successful. Size: {tempFileInfo.Length} bytes");
                                // ვწერთ დროებით ფაილში UTF-8 BOM-ით
                                //File.WriteAllText(tempFilePath, simpleOutput, new UTF8Encoding(true));
                                //var tempFileInfo = new FileInfo(tempFilePath);
                                if (tempFileInfo.Length > 0)
                                {
                                    Console.WriteLine($"Simple backup successful. Size: {tempFileInfo.Length} bytes");

                                    // ვგადაგვაქვს ფაილი საბოლოო ლოკაციაზე
                                    try
                                    {
                                        if (File.Exists(backupFilePath))
                                        {
                                            File.Delete(backupFilePath);
                                        }

                                        File.Move(tempFilePath, backupFilePath);
                                        Console.WriteLine($"Simple backup moved to final location: {backupFilePath}");

                                        //CheckBackupFileEncoding(backupFilePath);
                                        return true;
                                    }
                                    catch (Exception moveEx)
                                    {
                                        Console.WriteLine($"Error moving simple backup file: {moveEx.Message}");
                                        backupFilePath = tempFilePath;
                                        //CheckBackupFileEncoding(backupFilePath);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა MySQL ბექაპის შექმნის დროს: {ex.Message}");
                return false;
            }
        }

        // --- ძველი ბექაპების გაწმენდა ---
        public void CleanupOldBackups()
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                    return;
                var backupFiles = Directory.GetFiles(BackupDirectory, "backup_*.sql")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();
                if (backupFiles.Count > MaxBackupFiles)
                {
                    var filesToDelete = backupFiles.Skip(MaxBackupFiles);
                    foreach (var file in filesToDelete)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ვერ მოხერხდა ფაილის წაშლა {file.Name}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ძველი ბექაპების გაწმენდის დროს: {ex.Message}");
            }
        }

        // --- ბექაპის აღდგენა ---
        public bool RestoreBackup(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    throw new FileNotFoundException("ბექაპის ფაილი ვერ მოიძებნა");
                }
                string mysqlPath = MySqlDumpPath.Replace("mysqldump.exe", "mysql.exe");
                if (!File.Exists(mysqlPath))
                {
                    throw new FileNotFoundException("mysql.exe ვერ მოიძებნა");
                }
                string arguments = $"--host={ServerHost} --user={Username} --password={Password} {DatabaseName} < \"{backupFilePath}\"";
                var startInfo = new ProcessStartInfo
                {
                    FileName = mysqlPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ბექაპის აღდგენის დროს: {ex.Message}");
                return false;
            }
        }

        // --- ბექაპების სიის მიღება ---
        public List<BackupInfo> GetBackupList()
        {
            var backups = new List<BackupInfo>();
            try
            {
                if (!Directory.Exists(BackupDirectory))
                    return backups;
                var backupFiles = Directory.GetFiles(BackupDirectory, "backup_*.sql")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime);
                foreach (var file in backupFiles)
                {
                    backups.Add(new BackupInfo
                    {
                        FileName = file.Name,
                        FilePath = file.FullName,
                        CreationTime = file.CreationTime,
                        FileSize = file.Length,
                        FileSizeFormatted = FormatFileSize(file.Length)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ბექაპების სიის მიღების დროს: {ex.Message}");
            }
            return backups;
        }

        // --- ფაილის ზომის ფორმატირება ---
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        // --- პერიოდული ბექაპის გაჩერება ---
        public void StopPeriodicBackup()
        {
            if (_backupTimer != null)
            {
                _backupTimer.Stop();
                _backupTimer.Dispose();
                _backupTimer = null;
            }
        }

        // --- Manual Backup (SaveFileDialog-ით) ---
        public void ManualBackup(string dbFilePath)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
                saveFileDialog.Title = "აირჩიეთ სარეზერვო ფაილის ადგილი";
                saveFileDialog.FileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmm}.db";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(dbFilePath, saveFileDialog.FileName, true);
                    MessageBox.Show("სარეზერვო კოპირება შესრულდა!", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _config.LastBackupTime = DateTime.Now;
                    _config.DbChangedSinceLastBackup = false;
                }
            }
        }

        // --- Manual Restore (OpenFileDialog-ით) ---
        public void ManualRestore(string dbFilePath)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
                openFileDialog.Title = "აირჩიეთ სარეზერვო ფაილი";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string tempBackup = $"SchoolManagement_before_restore_{DateTime.Now:yyyyMMdd_HHmm}.db";
                    if (File.Exists(dbFilePath))
                        File.Copy(dbFilePath, tempBackup, true);

                    File.Copy(openFileDialog.FileName, dbFilePath, true);
                    MessageBox.Show("ბაზა წარმატებით აღდგენილია!", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _config.DbChangedSinceLastBackup = false;
                }
            }
        }

        // --- AutoBackup (Directory must exist) ---
        public void AutoBackup(string dbFilePath, string backupDirectory)
        {
            if (!DbChangedSinceLastBackup)
                return;

            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            string autoBackupFile = Path.Combine(backupDirectory, $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmm}.db");
            File.Copy(dbFilePath, autoBackupFile, true);
            _config.LastBackupTime = DateTime.Now;
            _config.DbChangedSinceLastBackup = false;
        }

        public bool CreateBackup(string backupFilePath)
        {
            try
            {
                // ვამოწმებთ არის თუ არა mysqldump.exe ხელმისაწვდომი და თავსებადი
                if (string.IsNullOrEmpty(MySqlDumpPath) || !File.Exists(MySqlDumpPath))
                {
                    Console.WriteLine("mysqldump.exe ვერ მოიძებნა. ვცდილობთ .NET მეთოდით ბექაპის შექმნას...");
                    return CreateBackupUsingDotNet(backupFilePath);
                }

                // ვცდილობთ mysqldump.exe-ის გამოყენებას
                try
                {
                    // ვამოწმებთ და ვქმნით დირექტორიას თუ საჭიროა
                    string backupDir = Path.GetDirectoryName(backupFilePath);
                    if (!Directory.Exists(backupDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(backupDir);
                        }
                        catch (Exception dirEx)
                        {
                            Console.WriteLine($"ვერ მოხერხდა ბექაპის დირექტორიის შექმნა: {dirEx.Message}");
                            // ვცდილობთ Temp ფოლდერში შექმნას
                            backupFilePath = Path.Combine(Path.GetTempPath(), $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                            Console.WriteLine($"ბექაპი შეიქმნება Temp ფოლდერში: {backupFilePath}");
                        }
                    }

                    string arguments = $"--host={ServerHost} --user={Username} --password={Password} " +
                        $"--default-character-set=utf8mb4 --set-charset --hex-blob --single-transaction " +
                        $"--routines --triggers --no-tablespaces --skip-lock-tables {DatabaseName}";

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = MySqlDumpPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        CreateNoWindow = true
                    };

                    using (var process = new Process { StartInfo = startInfo })
                    {
                        process.Start();

                        // პირდაპირ STDOUT-იდან ჩაწერა UTF-8-ში
                        using (var writer = new StreamWriter(backupFilePath, false, Encoding.UTF8))
                        {
                            while (!process.StandardOutput.EndOfStream)
                            {
                                string line = process.StandardOutput.ReadLine();
                                writer.WriteLine(line);
                            }
                        }

                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0 && string.IsNullOrEmpty(error))
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"mysqldump.exe შეცდომა: {error}");
                            Console.WriteLine("ვცდილობთ .NET მეთოდით ბექაპის შექმნას...");
                            return CreateBackupUsingDotNet(backupFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"mysqldump.exe გამოყენებისას შეცდომა: {ex.Message}");
                    Console.WriteLine("ვცდილობთ .NET მეთოდით ბექაპის შექმნას...");
                    return CreateBackupUsingDotNet(backupFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ბექაპის შექმნისას შეცდომა: {ex.Message}");
                return false;
            }
        }

        // ახალი მეთოდი .NET-ის გამოყენებით
        private bool CreateBackupUsingDotNet(string backupFilePath)
        {
            try
            {
                // ვამოწმებთ და ვქმნით დირექტორიას თუ საჭიროა
                string backupDir = Path.GetDirectoryName(backupFilePath);
                if (!Directory.Exists(backupDir))
                {
                    try
                    {
                        Directory.CreateDirectory(backupDir);
                    }
                    catch (Exception dirEx)
                    {
                        Console.WriteLine($"ვერ მოხერხდა ბექაპის დირექტორიის შექმნა: {dirEx.Message}");
                        // ვცდილობთ Temp ფოლდერში შექმნას
                        backupFilePath = Path.Combine(Path.GetTempPath(), $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                        Console.WriteLine($"ბექაპი შეიქმნება Temp ფოლდერში: {backupFilePath}");
                    }
                }

                using (var connection = new MySql.Data.MySqlClient.MySqlConnection($"Server={ServerHost};Database={DatabaseName};Uid={Username};Pwd={Password};Charset=utf8mb4;"))
                {
                    connection.Open();

                    var tables = new List<string>();
                    using (var command = new MySql.Data.MySqlClient.MySqlCommand("SHOW TABLES", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(reader.GetString(0));
                            }
                        }
                    }

                    using (var writer = new StreamWriter(backupFilePath, false, Encoding.UTF8))
                    {
                        writer.WriteLine("-- MySQL dump created using .NET backup method");
                        writer.WriteLine($"-- Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine($"-- Database: {DatabaseName}");
                        writer.WriteLine();
                        writer.WriteLine("SET NAMES utf8mb4;");
                        writer.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
                        writer.WriteLine();

                        foreach (var tableName in tables)
                        {
                            writer.WriteLine($"-- Table structure for table `{tableName}`");
                            writer.WriteLine($"DROP TABLE IF EXISTS `{tableName}`;");

                            using (var createCommand = new MySql.Data.MySqlClient.MySqlCommand($"SHOW CREATE TABLE `{tableName}`", connection))
                            {
                                using (var reader = createCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        writer.WriteLine(reader.GetString(1) + ";");
                                    }
                                }
                            }
                            writer.WriteLine();

                            // მონაცემების ექსპორტი
                            writer.WriteLine($"-- Data for table `{tableName}`");
                            using (var dataCommand = new MySql.Data.MySqlClient.MySqlCommand($"SELECT * FROM `{tableName}`", connection))
                            {
                                using (var reader = dataCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var values = new List<string>();
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            if (reader.IsDBNull(i))
                                            {
                                                values.Add("NULL");
                                            }
                                            else
                                            {
                                                var value = reader.GetValue(i);
                                                if (value is string)
                                                {
                                                    values.Add($"'{value.ToString().Replace("'", "''")}'");
                                                }
                                                else if (value is DateTime)
                                                {
                                                    values.Add($"'{((DateTime)value):yyyy-MM-dd HH:mm:ss}'");
                                                }
                                                else
                                                {
                                                    values.Add(value.ToString());
                                                }
                                            }
                                        }
                                        writer.WriteLine($"INSERT INTO `{tableName}` VALUES ({string.Join(", ", values)});");
                                    }
                                }
                            }
                            writer.WriteLine();
                        }

                        writer.WriteLine("SET FOREIGN_KEY_CHECKS = 1;");
                    }
                }

                Console.WriteLine($"ბექაპი წარმატებით შეიქმნა .NET მეთოდით: {backupFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($".NET მეთოდით ბექაპის შექმნისას შეცდომა: {ex.Message}");
                return false;
            }
        }

    }
}






