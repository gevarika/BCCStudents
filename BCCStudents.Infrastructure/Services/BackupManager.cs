using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Configuration;
using BCCStudents.Domain.Interfaces;

using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Infrastructure.Services
{
    public class BackupManager
    {
        private readonly IConfigurationService _config;

        public BackupManager(IConfigurationService config)
        {
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        // áƒªáƒ•áƒšáƒ˜áƒšáƒ"áƒ'áƒ˜áƒ¡ áƒ"áƒ áƒáƒ¨áƒ
        public bool DbChangedSinceLastBackup 
        { 
            get => _config.DbChangedSinceLastBackup;
            set => _config.DbChangedSinceLastBackup = value;
        }

        // áƒ‘áƒáƒšáƒ backup-áƒ˜áƒ¡ áƒ“áƒ áƒ (áƒ¡áƒ£áƒ áƒ•áƒ˜áƒšáƒ˜áƒ¡áƒáƒ›áƒ”áƒ‘áƒ )
        public DateTime LastBackupTime 
        { 
            get => _config.LastBackupTime;
            set => _config.LastBackupTime = value;
        }

        // áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¢áƒáƒ˜áƒ›áƒ”áƒ áƒ˜
        private System.Timers.Timer _backupTimer;
        
        // áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ™áƒáƒœáƒ¤áƒ˜áƒ’áƒ£áƒ áƒáƒªáƒ˜áƒ
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

        // MySQL áƒ™áƒáƒœáƒ¤áƒ˜áƒ’áƒ£áƒ áƒáƒªáƒ˜áƒ
        private string _mySqlDumpPath;
        public string MySqlDumpPath
        {
            get
            {
                // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ¯áƒ”áƒ  áƒ™áƒ˜áƒ“áƒ”áƒ• áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜
                // áƒ”áƒ¡ áƒ›áƒœáƒ˜áƒ¨áƒ•áƒœáƒ”áƒšáƒáƒ•áƒáƒœáƒ˜áƒ áƒ áƒáƒ“áƒ’áƒáƒœ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ¨áƒ”áƒ˜áƒ«áƒšáƒ”áƒ‘áƒ áƒ¬áƒáƒ˜áƒ¨áƒáƒšáƒáƒ¡ áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ˜áƒ¡ áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡
                if (string.IsNullOrEmpty(_mySqlDumpPath) || !File.Exists(_mySqlDumpPath))
                {
                    // áƒžáƒ˜áƒ áƒ•áƒ”áƒš áƒ áƒ˜áƒ’áƒ¨áƒ˜ áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ˜áƒ¡ áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜
                    string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string localDumpPath = Path.Combine(exeDir, "mysqldump.exe");
                    
                    if (File.Exists(localDumpPath))
                    {
                        _mySqlDumpPath = localDumpPath;
                    }
                    else
                    {
                        // áƒ—áƒ£ áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ˜áƒ¡ áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜ áƒáƒ  áƒáƒ áƒ˜áƒ¡, áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¡áƒ¢áƒáƒœáƒ“áƒáƒ áƒ¢áƒ£áƒš áƒ‘áƒ˜áƒšáƒ˜áƒ™áƒ”áƒ‘áƒ¨áƒ˜
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
        public string ServerHost 
        { 
            get => _config.ServerHost;
            set => _config.ServerHost = value;
        }
        public string DatabaseName 
        { 
            get => _config.DatabaseName;
            set => _config.DatabaseName = value;
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

        // --- áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ ---
        public void InitializePeriodicBackup()
        {
            try
            {
                Console.WriteLine($"InitializePeriodicBackup - BackupDirectory: '{BackupDirectory}'");
                Console.WriteLine($"InitializePeriodicBackup - Application.StartupPath: '{System.Windows.Forms.Application.StartupPath}'");
                
                // áƒ¨áƒ”áƒ•áƒ¥áƒ›áƒœáƒáƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ“áƒ˜áƒ áƒ”áƒ¥áƒ¢áƒáƒ áƒ˜áƒ
                if (!Directory.Exists(BackupDirectory))
                {
                    Console.WriteLine($"Creating backup directory: {BackupDirectory}");
                    Directory.CreateDirectory(BackupDirectory);
                }

                // áƒ¨áƒ”áƒ•áƒ¥áƒ›áƒœáƒáƒ— áƒ¢áƒáƒ˜áƒ›áƒ”áƒ áƒ˜ áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
                _backupTimer = new System.Timers.Timer(BackupIntervalMinutes * 60 * 1000); // áƒ¬áƒ£áƒ—áƒ”áƒ‘áƒ¨áƒ˜
                _backupTimer.Elapsed += OnBackupTimerElapsed;
                _backupTimer.AutoReset = true;
                _backupTimer.Enabled = AutoBackupEnabled;

                // áƒ’áƒáƒ•áƒ£áƒ¨áƒ•áƒáƒ— áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ—áƒ£ áƒ¯áƒ”áƒ  áƒáƒ  áƒ’áƒáƒ™áƒ”áƒ—áƒ”áƒ‘áƒ£áƒšáƒ
                if (LastBackupTime == DateTime.MinValue)
                {
                    CreatePeriodicBackup();
                }

                Console.WriteLine($"áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ. áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜: {BackupIntervalMinutes} áƒ¬áƒ£áƒ—áƒ˜");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
            }
        }

        // --- áƒ¢áƒáƒ˜áƒ›áƒ”áƒ áƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ˜ ---
        private void OnBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (AutoBackupEnabled)
            {
                CreatePeriodicBackup();
            }
        }

        // --- áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ ---
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

                // áƒ¨áƒ”áƒ•áƒ¥áƒ›áƒœáƒáƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜
                bool success = CreateMySQLBackup(backupFilePath);
                
                if (success)
                {
                    _config.LastBackupTime = DateTime.Now;
                    _config.DbChangedSinceLastBackup = false;
                    
                    // áƒ’áƒáƒ•áƒ¬áƒ›áƒ˜áƒœáƒ“áƒáƒ— áƒ«áƒ•áƒ”áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜
                    CleanupOldBackups();
                    
                    Console.WriteLine($"áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¨áƒ”áƒ˜áƒ¥áƒ›áƒœáƒ: {backupFileName}");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
                return false;
            }
        }

        // --- MySQL áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ ---
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
                    string errorMessage = "mysqldump.exe áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ!\n\n" +
                        "áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒ©áƒáƒáƒ›áƒáƒ¢áƒ”áƒ— mysqldump.exe áƒ“áƒ áƒ¡áƒáƒ­áƒ˜áƒ áƒ DLL-áƒ”áƒ‘áƒ˜ áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ˜áƒ¡ áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜ áƒáƒœ áƒ“áƒáƒáƒ§áƒ”áƒœáƒ”áƒ— MySQL Server.\n\n" +
                        "áƒ«áƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ‘áƒ˜áƒšáƒ˜áƒ™áƒ”áƒ‘áƒ˜:\n" +
                        "- áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ˜áƒ¡ áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ˜\n" +
                        "- C:\\Program Files\\MySQL\\MySQL Server 8.0\\bin\\\n" +
                        "- C:\\Program Files\\MySQL\\MySQL Server 5.7\\bin\\\n" +
                        "- C:\\xampp\\mysql\\bin\\";
                    
                    MessageBox.Show(errorMessage, "áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        
                        // áƒ•áƒªáƒáƒ“áƒáƒ— áƒ£áƒ¤áƒ áƒ áƒ›áƒáƒ áƒ¢áƒ˜áƒ•áƒ˜ áƒ•áƒáƒ áƒ˜áƒáƒœáƒ¢áƒ˜
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
                            StandardOutputEncoding = Encoding.UTF8 // áƒ”áƒ¡ áƒáƒ£áƒªáƒ˜áƒšáƒ”áƒ‘áƒ”áƒšáƒ˜áƒ!
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
                                // áƒ•áƒ¬áƒ”áƒ áƒ— áƒ“áƒ áƒáƒ”áƒ‘áƒ˜áƒ— áƒ¤áƒáƒ˜áƒšáƒ¨áƒ˜ UTF-8 BOM-áƒ˜áƒ—
                                File.WriteAllText(tempFilePath, simpleOutput, new UTF8Encoding(true));
                                var tempFileInfo = new FileInfo(tempFilePath);
                                if (tempFileInfo.Length > 0)
                                {
                                    Console.WriteLine($"Simple backup successful. Size: {tempFileInfo.Length} bytes");
                                    
                                    // áƒ•áƒ›áƒáƒ«áƒ áƒáƒ•áƒ— áƒ¤áƒáƒ˜áƒšáƒ¡ áƒ¡áƒáƒ‘áƒáƒšáƒáƒ áƒšáƒáƒ™áƒáƒªáƒ˜áƒáƒ–áƒ”
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
                                // áƒ•áƒ¬áƒ”áƒ áƒ— áƒ“áƒ áƒáƒ”áƒ‘áƒ˜áƒ— áƒ¤áƒáƒ˜áƒšáƒ¨áƒ˜ UTF-8 BOM-áƒ˜áƒ—
                                //File.WriteAllText(tempFilePath, simpleOutput, new UTF8Encoding(true));
                                //var tempFileInfo = new FileInfo(tempFilePath);
                                if (tempFileInfo.Length > 0)
                                {
                                    Console.WriteLine($"Simple backup successful. Size: {tempFileInfo.Length} bytes");

                                    // áƒ•áƒ›áƒáƒ«áƒ áƒáƒ•áƒ— áƒ¤áƒáƒ˜áƒšáƒ¡ áƒ¡áƒáƒ‘áƒáƒšáƒáƒ áƒšáƒáƒ™áƒáƒªáƒ˜áƒáƒ–áƒ”
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
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ MySQL áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
                return false;
            }
        }

        // --- áƒ«áƒ•áƒ”áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒ¬áƒ›áƒ”áƒœáƒ“áƒ ---
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
                            Console.WriteLine($"áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ {file.Name}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ«áƒ•áƒ”áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒ¬áƒ›áƒ”áƒœáƒ“áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
            }
        }

        // --- áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ ---
        public bool RestoreBackup(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    throw new FileNotFoundException("áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ");
                }
                string mysqlPath = MySqlDumpPath.Replace("mysqldump.exe", "mysql.exe");
                if (!File.Exists(mysqlPath))
                {
                    throw new FileNotFoundException("mysql.exe áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ");
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
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
                return false;
            }
        }

        // --- áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ¡áƒ˜áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ ---
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
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ¡áƒ˜áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
            }
            return backups;
        }

        // --- áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ–áƒáƒ›áƒ˜áƒ¡ áƒ¤áƒáƒ áƒ›áƒáƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ ---
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

        // --- áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ’áƒáƒ©áƒ”áƒ áƒ”áƒ‘áƒ ---
        public void StopPeriodicBackup()
        {
            if (_backupTimer != null)
            {
                _backupTimer.Stop();
                _backupTimer.Dispose();
                _backupTimer = null;
            }
        }

        // --- Manual Backup (SaveFileDialog-áƒ˜áƒ—) ---
        public void ManualBackup(string dbFilePath)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
                saveFileDialog.Title = "áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ¡áƒáƒ áƒ”áƒ–áƒ”áƒ áƒ•áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒáƒ“áƒ’áƒ˜áƒšáƒ˜";
                saveFileDialog.FileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmm}.db";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(dbFilePath, saveFileDialog.FileName, true);
                    MessageBox.Show("áƒ¡áƒáƒ áƒ”áƒ–áƒ”áƒ áƒ•áƒ áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ“áƒ!", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _config.LastBackupTime = DateTime.Now;
                    _config.DbChangedSinceLastBackup = false;
                }
            }
        }

        // --- Manual Restore (OpenFileDialog-áƒ˜áƒ—) ---
        public void ManualRestore(string dbFilePath)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";
                openFileDialog.Title = "áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ¡áƒáƒ áƒ”áƒ–áƒ”áƒ áƒ•áƒ áƒ¤áƒáƒ˜áƒšáƒ˜";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string tempBackup = $"SchoolManagement_before_restore_{DateTime.Now:yyyyMMdd_HHmm}.db";
                    if (File.Exists(dbFilePath))
                        File.Copy(dbFilePath, tempBackup, true);

                    File.Copy(openFileDialog.FileName, dbFilePath, true);
                    MessageBox.Show("áƒ‘áƒáƒ–áƒ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ˜áƒšáƒ˜áƒ!", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /*public static void CreateBackup(string connectionString, string backupPath)
        {
            var mysqldumpPath = @"C:\Program Files\MySQL\MySQL Server 9.3\bin\mysqldump.exe";
            string file = Path.Combine(backupPath, $"backup-{DateTime.Now:yyyy-MM-dd-HH-mm}.sql");
            string cmd = $"\"{mysqldumpPath}\" -ubccenter_schoolAdmin25 -p'@wh0X0kpPHey' -hbccenter.ge bccenter_SchoolManagement_Test > \"{file}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", $"/C {cmd}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            Process.Start(startInfo);
        }*/
        public bool CreateBackup(string backupFilePath)
        {
            try
            {
                // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ mysqldump.exe áƒ®áƒ”áƒšáƒ›áƒ˜áƒ¡áƒáƒ¬áƒ•áƒ“áƒáƒ›áƒ˜ áƒ“áƒ áƒ—áƒáƒ•áƒ¡áƒ”áƒ‘áƒáƒ“áƒ˜
                if (string.IsNullOrEmpty(MySqlDumpPath) || !File.Exists(MySqlDumpPath))
                {
                    Console.WriteLine("mysqldump.exe áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ. áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— .NET áƒ›áƒ”áƒ—áƒáƒ“áƒ˜áƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒáƒ¡...");
                    return CreateBackupUsingDotNet(backupFilePath);
                }

                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— mysqldump.exe-áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒáƒ¡
                try
                {
                    // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ“áƒ áƒ•áƒ¥áƒ›áƒœáƒ˜áƒ— áƒ“áƒ˜áƒ áƒ”áƒ¥áƒ¢áƒáƒ áƒ˜áƒáƒ¡ áƒ—áƒ£ áƒ¡áƒáƒ­áƒ˜áƒ áƒáƒ
                    string backupDir = Path.GetDirectoryName(backupFilePath);
                    if (!Directory.Exists(backupDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(backupDir);
                        }
                        catch (Exception dirEx)
                        {
                            Console.WriteLine($"áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ“áƒ˜áƒ áƒ”áƒ¥áƒ¢áƒáƒ áƒ˜áƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ: {dirEx.Message}");
                            // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— Temp áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒáƒ¡
                            backupFilePath = Path.Combine(Path.GetTempPath(), $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                            Console.WriteLine($"áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¨áƒ”áƒ˜áƒ¥áƒ›áƒœáƒ”áƒ‘áƒ Temp áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜: {backupFilePath}");
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

                        // áƒžáƒ˜áƒ áƒ“áƒáƒžáƒ˜áƒ  STDOUT-áƒ˜áƒ“áƒáƒœ áƒ©áƒáƒ¬áƒ”áƒ áƒ UTF-8â€“áƒ¨áƒ˜
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
                            Console.WriteLine($"mysqldump.exe áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {error}");
                            Console.WriteLine("áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— .NET áƒ›áƒ”áƒ—áƒáƒ“áƒ˜áƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒáƒ¡...");
                            return CreateBackupUsingDotNet(backupFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"mysqldump.exe áƒ’áƒáƒ›áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                    Console.WriteLine("áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— .NET áƒ›áƒ”áƒ—áƒáƒ“áƒ˜áƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒáƒ¡...");
                    return CreateBackupUsingDotNet(backupFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡áƒáƒ¡ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                return false;
            }
        }

        // áƒáƒ®áƒáƒšáƒ˜ áƒ›áƒ”áƒ—áƒáƒ“áƒ˜ .NET-áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ˜áƒ—
        private bool CreateBackupUsingDotNet(string backupFilePath)
        {
            try
            {
                // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ“áƒ áƒ•áƒ¥áƒ›áƒœáƒ˜áƒ— áƒ“áƒ˜áƒ áƒ”áƒ¥áƒ¢áƒáƒ áƒ˜áƒáƒ¡ áƒ—áƒ£ áƒ¡áƒáƒ­áƒ˜áƒ áƒáƒ
                string backupDir = Path.GetDirectoryName(backupFilePath);
                if (!Directory.Exists(backupDir))
                {
                    try
                    {
                        Directory.CreateDirectory(backupDir);
                    }
                    catch (Exception dirEx)
                    {
                        Console.WriteLine($"áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ“áƒ˜áƒ áƒ”áƒ¥áƒ¢áƒáƒ áƒ˜áƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ: {dirEx.Message}");
                        // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— Temp áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒáƒ¡
                        backupFilePath = Path.Combine(Path.GetTempPath(), $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                        Console.WriteLine($"áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¨áƒ”áƒ˜áƒ¥áƒ›áƒœáƒ”áƒ‘áƒ Temp áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ¨áƒ˜: {backupFilePath}");
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

                            // áƒ›áƒáƒœáƒáƒªáƒ”áƒ›áƒ”áƒ‘áƒ˜áƒ¡ áƒ”áƒ¥áƒ¡áƒžáƒáƒ áƒ¢áƒ˜
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

                Console.WriteLine($"áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ¨áƒ”áƒ˜áƒ¥áƒ›áƒœáƒ .NET áƒ›áƒ”áƒ—áƒáƒ“áƒ˜áƒ—: {backupFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($".NET áƒ›áƒ”áƒ—áƒáƒ“áƒ˜áƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡áƒáƒ¡ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                return false;
            }
        }

        /*private static void CheckBackupFileEncoding(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    return;
                }

                var fileInfo = new FileInfo(backupFilePath);
                Console.WriteLine($"Checking encoding for backup file: {fileInfo.Name}");
                Console.WriteLine($"File size: {fileInfo.Length} bytes");

                // áƒ•áƒ™áƒ˜áƒ—áƒ®áƒ£áƒšáƒáƒ‘áƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒžáƒ˜áƒ áƒ•áƒ”áƒš 1000 áƒ‘áƒáƒ˜áƒ¢áƒ¡
                byte[] firstBytes = File.ReadAllBytes(backupFilePath).Take(1000).ToArray();
                
                // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— BOM-áƒ¡
                bool hasBOM = false;
                if (firstBytes.Length >= 3)
                {
                    hasBOM = (firstBytes[0] == 0xEF && firstBytes[1] == 0xBB && firstBytes[2] == 0xBF);
                    Console.WriteLine($"UTF-8 BOM detected: {hasBOM}");
                }

                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¡áƒ®áƒ•áƒáƒ“áƒáƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
                var encodings = new[]
                {
                    new UTF8Encoding(false), // UTF-8 without BOM
                    new UTF8Encoding(true),  // UTF-8 with BOM
                    Encoding.GetEncoding("ISO-8859-1"),
                    Encoding.GetEncoding("Windows-1252"),
                    Encoding.GetEncoding("ISO-8859-5"),
                    Encoding.Default
                };

                foreach (var encoding in encodings)
                {
                    try
                    {
                        string testText = encoding.GetString(firstBytes);
                        
                        // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ¡
                        var georgianChars = testText.Where(c => c >= '\u10A0' && c <= '\u10FF').Take(5).ToArray();
                        bool hasGeorgian = georgianChars.Length > 0;
                        
                        // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒš áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ¡
                        bool hasEncodedGeorgian = testText.Contains("Ã¡Æ’") || testText.Contains("Ã¡Æ’");
                        
                        Console.WriteLine($"{encoding.EncodingName}: Georgian={hasGeorgian}, Encoded={hasEncodedGeorgian}");
                        
                        if (hasGeorgian)
                        {
                            Console.WriteLine($"Georgian characters found: {string.Join(", ", georgianChars)}");
                        }
                        
                        if (hasEncodedGeorgian)
                        {
                            Console.WriteLine("WARNING: Encoded Georgian text detected - encoding issue!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error testing {encoding.EncodingName}: {ex.Message}");
                    }
                }

                // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ›áƒ—áƒ”áƒšáƒ˜ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒœáƒ˜áƒ›áƒ£áƒ¨áƒ¡
                try
                {
                    string fullContent = File.ReadAllText(backupFilePath, new UTF8Encoding(false));
                    var allGeorgianChars = fullContent.Where(c => c >= '\u10A0' && c <= '\u10FF').Take(10).ToArray();
                    int georgianCount = fullContent.Count(c => c >= '\u10A0' && c <= '\u10FF');
                    int encodedCount = fullContent.Split(new[] { "Ã¡Æ’" }, StringSplitOptions.None).Length - 1;
                    
                    Console.WriteLine($"Full file analysis:");
                    Console.WriteLine($"  Total Georgian characters: {georgianCount}");
                    Console.WriteLine($"  Encoded Georgian sequences: {encodedCount}");
                    Console.WriteLine($"  Sample Georgian chars: {string.Join(", ", allGeorgianChars)}");
                    
                    if (georgianCount > 0 && encodedCount == 0)
                    {
                        Console.WriteLine("âœ“ Georgian text appears to be properly encoded");
                    }
                    else if (encodedCount > 0)
                    {
                        Console.WriteLine("âš  WARNING: Encoded Georgian text detected in backup file");
                    }
                    else
                    {
                        Console.WriteLine("â„¹ No Georgian text detected in backup file");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing full file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckBackupFileEncoding: {ex.Message}");
            }
        }*/

        // --- áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ áƒáƒžáƒ¢áƒ˜áƒ›áƒ˜áƒ–áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ ---
        /*public static bool CreateGeorgianOptimizedBackup(string backupFilePath)
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
                    throw new FileNotFoundException("mysqldump.exe áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ");
                }
                string tempFileName = $"temp_backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
                string arguments = $"--host={ServerHost} --user={Username} --password={Password} " +
                                 $"--default-character-set=utf8mb4 --set-charset --hex-blob " +
                                 $"--single-transaction --routines --triggers " +
                                 $"--no-tablespaces --skip-lock-tables " +
                                 $"--add-drop-database --add-drop-table " +
                                 $"--result-file=\"{tempFilePath}\" {DatabaseName}";
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
                                CheckBackupFileEncoding(backupFilePath);
                                return true;
                            }
                            catch (Exception moveEx)
                            {
                                backupFilePath = tempFilePath;
                                CheckBackupFileEncoding(backupFilePath);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.WriteLine($"Georgian backup Error: {error}");
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒáƒžáƒ¢áƒ˜áƒ›áƒ˜áƒ–áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}");
                return false;
            }
        }*/

        // --- áƒáƒ áƒ¡áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒ¡áƒ¬áƒáƒ áƒ”áƒ‘áƒ ---
        /*public static bool FixBackupEncoding(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    return false;
                }
                // ... (áƒ¨áƒ”áƒ˜áƒœáƒáƒ áƒ©áƒ£áƒœáƒ” áƒ›áƒ®áƒáƒšáƒáƒ“ áƒ¡áƒáƒ­áƒ˜áƒ áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒ¡áƒ¬áƒáƒ áƒ”áƒ‘áƒ áƒáƒœ áƒ’áƒáƒáƒ›áƒáƒ áƒ¢áƒ˜áƒ•áƒ”)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fixing backup encoding: {ex.Message}");
                return false;
            }
        }*/
    }

    // áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ™áƒšáƒáƒ¡áƒ˜
    public class BackupInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreationTime { get; set; }
        public long FileSize { get; set; }
        public string FileSizeFormatted { get; set; }
    }
}






